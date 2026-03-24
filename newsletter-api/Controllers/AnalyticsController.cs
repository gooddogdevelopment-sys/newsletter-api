using dotnet_core_api_w_postgres.Models.DTO;
using dotnet_core_api_w_postgres.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_core_api_w_postgres.Controllers;

[ApiController]
[Route("[controller]")]
public class AnalyticsController (IAnalyticsService service) : ControllerBase
{
    [HttpGet("monthly-newsletters")]
    public async Task<ActionResult<NewsletterAnalyticsDto>> GetNewsLetterAnalytics()
    {
        var result = await service.GetAnalytics();
        return Ok(result);
    }
    
    [HttpGet("newsletter-counts-by-month")]
    public async Task<ActionResult<List<NewsletterTotalsByMonthDto>>> GetNewsLetterTotalsByMonth()
    {
        var result = await service.GetNewsLetterTotalsByMonth();
        return Ok(result);
    }
}