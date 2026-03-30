using dotnet_core_api_w_postgres.Data;
using dotnet_core_api_w_postgres.Models;
using dotnet_core_api_w_postgres.Services;
using Microsoft.EntityFrameworkCore;

namespace newsletter_api.Tests.Services;

public class AnalyticsServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AnalyticsService _sut;

    // Pin dates relative to now so tests are always accurate regardless of when they run
    private static readonly DateTime _thisMonth = DateTime.Now;
    private static readonly DateTime _lastMonth = DateTime.Now.AddMonths(-1);
    private static readonly DateTime _twoMonthsAgo = DateTime.Now.AddMonths(-2);

    public AnalyticsServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new AnalyticsService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region GetAnalytics

    [Fact]
    public async Task GetAnalytics_ReturnsZeroForAll_WhenNoNewsLettersExist()
    {
        // Act
        var result = await _sut.GetAnalytics();

        // Assert
        Assert.Equal(0, result.TotalNewsLettersSent);
        Assert.Equal(0, result.TotalNewsLettersSentThisMonth);
        Assert.Equal(0, result.TotalNewsLettersSentLastMonth);
    }

    [Fact]
    public async Task GetAnalytics_ReturnsTotalNewsLettersSent()
    {
        // Arrange
        _context.NewsLetters.AddRange(
            CreateNewsletter(1, _thisMonth),
            CreateNewsletter(2, _lastMonth),
            CreateNewsletter(3, _twoMonthsAgo)
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAnalytics();

        // Assert
        Assert.Equal(3, result.TotalNewsLettersSent);
    }

    [Fact]
    public async Task GetAnalytics_ReturnsCorrectCountForThisMonth()
    {
        // Arrange
        _context.NewsLetters.AddRange(
            CreateNewsletter(1, _thisMonth),
            CreateNewsletter(2, _thisMonth),
            CreateNewsletter(3, _lastMonth)
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAnalytics();

        // Assert
        Assert.Equal(2, result.TotalNewsLettersSentThisMonth);
    }

    [Fact]
    public async Task GetAnalytics_ReturnsCorrectCountForLastMonth()
    {
        // Arrange
        _context.NewsLetters.AddRange(
            CreateNewsletter(1, _thisMonth),
            CreateNewsletter(2, _lastMonth),
            CreateNewsletter(3, _lastMonth),
            CreateNewsletter(4, _twoMonthsAgo)
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAnalytics();

        // Assert
        Assert.Equal(2, result.TotalNewsLettersSentLastMonth);
    }

    [Fact]
    public async Task GetAnalytics_DoesNotIncludeOlderNewsLetters_InMonthlyCount()
    {
        // Arrange
        _context.NewsLetters.AddRange(
            CreateNewsletter(1, _twoMonthsAgo),
            CreateNewsletter(2, _twoMonthsAgo)
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAnalytics();

        // Assert
        Assert.Equal(0, result.TotalNewsLettersSentThisMonth);
        Assert.Equal(0, result.TotalNewsLettersSentLastMonth);
        Assert.Equal(2, result.TotalNewsLettersSent);
    }

    [Fact]
    public async Task GetAnalytics_ReturnsZeroForThisMonth_WhenOnlyLastMonthExists()
    {
        // Arrange
        _context.NewsLetters.Add(CreateNewsletter(1, _lastMonth));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAnalytics();

        // Assert
        Assert.Equal(0, result.TotalNewsLettersSentThisMonth);
        Assert.Equal(1, result.TotalNewsLettersSentLastMonth);
    }

    #endregion

    #region GetNewsLetterTotalsByMonth

    [Fact]
    public async Task GetNewsLetterTotalsByMonth_ReturnsEmptyList_WhenNoNewsLettersExist()
    {
        // Act
        var result = await _sut.GetNewsLetterTotalsByMonth();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetNewsLetterTotalsByMonth_GroupsCorrectlyByMonth()
    {
        // Arrange
        _context.NewsLetters.AddRange(
            CreateNewsletter(1, _thisMonth),
            CreateNewsletter(2, _thisMonth),
            CreateNewsletter(3, _lastMonth)
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetNewsLetterTotalsByMonth();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetNewsLetterTotalsByMonth_CountsAreCorrectPerGroup()
    {
        // Arrange
        _context.NewsLetters.AddRange(
            CreateNewsletter(1, _thisMonth),
            CreateNewsletter(2, _thisMonth),
            CreateNewsletter(3, _thisMonth),
            CreateNewsletter(4, _lastMonth)
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetNewsLetterTotalsByMonth();

        var thisMonthResult = result.First(x => x.Month == $"{_thisMonth.Year}-{_thisMonth.Month}");
        var lastMonthResult = result.First(x => x.Month == $"{_lastMonth.Year}-{_lastMonth.Month}");

        // Assert
        Assert.Equal(3, thisMonthResult.Count);
        Assert.Equal(1, lastMonthResult.Count);
    }

    [Fact]
    public async Task GetNewsLetterTotalsByMonth_FormatsMonthCorrectly()
    {
        // Arrange
        _context.NewsLetters.Add(CreateNewsletter(1, _thisMonth));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetNewsLetterTotalsByMonth();

        // Assert
        Assert.Equal($"{_thisMonth.Year}-{_thisMonth.Month}", result[0].Month);
    }

    [Fact]
    public async Task GetNewsLetterTotalsByMonth_ReturnsResultsOrderedByMonth()
    {
        // Arrange
        _context.NewsLetters.AddRange(
            CreateNewsletter(1, _thisMonth),
            CreateNewsletter(2, _twoMonthsAgo),
            CreateNewsletter(3, _lastMonth)
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetNewsLetterTotalsByMonth();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal($"{_twoMonthsAgo.Year}-{_twoMonthsAgo.Month}", result[0].Month);
        Assert.Equal($"{_lastMonth.Year}-{_lastMonth.Month}", result[1].Month);
        Assert.Equal($"{_thisMonth.Year}-{_thisMonth.Month}", result[2].Month);
    }

    #endregion

    #region Helpers

    private static Newsletter CreateNewsletter(int id, DateTime sendDate) => new()
    {
        Id = id,
        Title = $"Newsletter {id}",
        Subject = "Test Subject",
        Overview = "Test overview for the newsletter that is long enough for any truncation checks.",
        SendDate = sendDate,
        AiProvider = "OpenAI",
        AiModel = "gpt-4o"
    };

    #endregion
}
