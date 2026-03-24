using dotnet_core_api_w_postgres.Data;
using dotnet_core_api_w_postgres.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace dotnet_core_api_w_postgres.Services;

public interface IAnalyticsService
{
   Task<NewsletterAnalyticsDto> GetAnalytics();
   Task<List<NewsletterTotalsByMonthDto>> GetNewsLetterTotalsByMonth();
}

public class AnalyticsService (AppDbContext context) : IAnalyticsService
{
    public async Task<NewsletterAnalyticsDto> GetAnalytics()
    {
        return new NewsletterAnalyticsDto
        {
            TotalNewsLettersSent = await GetTotalNewsLettersSent(),
            TotalNewsLettersSentThisMonth = await GetCurrentMonthNewsLetterCount(),
            TotalNewsLettersSentLastMonth = await GetPriorMonthNewsLetterCount()
        };
    }
    
    private async Task<int> GetCurrentMonthNewsLetterCount()
    {
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var count = await context.NewsLetters.Where(n =>
            n.SendDate.Month == currentMonth && n.SendDate.Year == currentYear).CountAsync();
        return count;
    }

    private async Task<int> GetPriorMonthNewsLetterCount()
    {
        var priorMonth = DateTime.Now.AddMonths(-1);
        var count = await context.NewsLetters.Where(n =>
            n.SendDate.Month == priorMonth.Month && n.SendDate.Year == priorMonth.Year).CountAsync();
        return count;
    }

    private async Task<int> GetTotalNewsLettersSent()
    {
        var count = await context.NewsLetters.CountAsync();
        return count;
    }

    public async Task<List<NewsletterTotalsByMonthDto>> GetNewsLetterTotalsByMonth()
    {
        var result = await context.NewsLetters
            .GroupBy(n => new { n.SendDate.Year, n.SendDate.Month })
            .Select(g => new NewsletterTotalsByMonthDto
            {
                Month = g.Key.Year + "-" + g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(x => x.Month)
            .ToListAsync();
        return result;
    }
}