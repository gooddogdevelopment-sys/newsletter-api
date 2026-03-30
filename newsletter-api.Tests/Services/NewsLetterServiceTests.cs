using dotnet_core_api_w_postgres.Data;
using dotnet_core_api_w_postgres.Models;
using dotnet_core_api_w_postgres.Services;
using Microsoft.EntityFrameworkCore;

namespace newsletter_api.Tests.Services;

public class NewsLetterServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly NewsLetterService _sut;

    public NewsLetterServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _sut = new NewsLetterService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region GetAllNewsLettersAsync

    [Fact]
    public async Task GetAllNewsLettersAsync_ReturnsAllNewsLetters()
    {
        // Arrange
        _context.NewsLetters.AddRange(CreateNewsletter(1, "First"), CreateNewsletter(2, "Second"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllNewsLettersAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAllNewsLettersAsync_ReturnsEmptyList_WhenNoNewsLettersExist()
    {
        // Act
        var result = await _sut.GetAllNewsLettersAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllNewsLettersAsync_ReturnsNewsLettersOrderedBySendDateDescending()
    {
        // Arrange
        _context.NewsLetters.AddRange(
            CreateNewsletter(1, "Oldest", sendDate: new DateTime(2024, 1, 1)),
            CreateNewsletter(2, "Newest", sendDate: new DateTime(2025, 1, 1)),
            CreateNewsletter(3, "Middle", sendDate: new DateTime(2024, 6, 1))
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllNewsLettersAsync();

        // Assert
        Assert.Equal("Newest", result[0].Title);
        Assert.Equal("Middle", result[1].Title);
        Assert.Equal("Oldest", result[2].Title);
    }

    [Fact]
    public async Task GetAllNewsLettersAsync_TruncatesOverviewTo150Characters()
    {
        // Arrange
        var longOverview = new string('A', 300);
        _context.NewsLetters.Add(CreateNewsletter(1, "Test", overview: longOverview));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllNewsLettersAsync();

        // Assert
        Assert.Equal(150, result[0].Overview.Length);
    }

    [Fact]
    public async Task GetAllNewsLettersAsync_MapsFieldsCorrectly()
    {
        // Arrange
        var sendDate = new DateTime(2025, 6, 15);
        _context.NewsLetters.Add(CreateNewsletter(1, "Mapped Title", subject: "Test Subject", sendDate: sendDate));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllNewsLettersAsync();
        var dto = result[0];

        // Assert
        Assert.Equal("Mapped Title", dto.Title);
        Assert.Equal("Test Subject", dto.Subject);
        Assert.Equal(sendDate, dto.SendDate);
    }

    #endregion

    #region GetNewsLetterByIdAsync

    [Fact]
    public async Task GetNewsLetterByIdAsync_ReturnsCorrectNewsLetter()
    {
        // Arrange
        _context.NewsLetters.AddRange(CreateNewsletter(1, "First"), CreateNewsletter(2, "Second"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetNewsLetterByIdAsync(1);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("First", result.Title);
    }

    [Fact]
    public async Task GetNewsLetterByIdAsync_ThrowsException_WhenNewsLetterNotFound()
    {
        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _sut.GetNewsLetterByIdAsync(999));
    }

    [Fact]
    public async Task GetNewsLetterByIdAsync_ThrowsExceptionWithCorrectMessage_WhenNewsLetterNotFound()
    {
        // Act
        var exception = await Assert.ThrowsAsync<Exception>(() => _sut.GetNewsLetterByIdAsync(999));

        // Assert
        Assert.Equal("NewsLetter not found", exception.Message);
    }

    [Fact]
    public async Task GetNewsLetterByIdAsync_IncludesFullOverview()
    {
        // Arrange
        var fullOverview = new string('A', 300);
        _context.NewsLetters.Add(CreateNewsletter(1, "Test", overview: fullOverview));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetNewsLetterByIdAsync(1);

        // Assert
        Assert.Equal(300, result.Overview.Length);
    }

    [Fact]
    public async Task GetNewsLetterByIdAsync_IncludesCodeSnippet()
    {
        // Arrange
        var newsletter = CreateNewsletter(1, "Test");
        newsletter.CodeSnippet = "var x = 1;";
        _context.NewsLetters.Add(newsletter);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetNewsLetterByIdAsync(1);

        // Assert
        Assert.Equal("var x = 1;", result.CodeSnippet);
    }

    [Fact]
    public async Task GetNewsLetterByIdAsync_CodeSnippetIsNull_WhenNotSet()
    {
        // Arrange
        _context.NewsLetters.Add(CreateNewsletter(1, "Test"));
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetNewsLetterByIdAsync(1);

        // Assert
        Assert.Null(result.CodeSnippet);
    }

    #endregion

    #region Helpers

    private static Newsletter CreateNewsletter(
        int id,
        string title,
        string subject = "Default Subject",
        string overview = "This is a default overview that is intentionally long enough to exceed the one hundred and fifty character truncation limit applied by the GetAllNewsLetters query.",
        DateTime? sendDate = null) => new()
    {
        Id = id,
        Title = title,
        Subject = subject,
        Overview = overview,
        SendDate = sendDate ?? DateTime.UtcNow,
        AiProvider = "OpenAI",
        AiModel = "gpt-4o"
    };

    #endregion
}
