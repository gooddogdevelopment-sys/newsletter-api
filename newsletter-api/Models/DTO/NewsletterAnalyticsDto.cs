namespace dotnet_core_api_w_postgres.Models.DTO;

public class NewsletterAnalyticsDto
{
    public required int TotalNewsLettersSent { get; set; }
    public required int TotalNewsLettersSentThisMonth { get; set; }
    public required int TotalNewsLettersSentLastMonth { get; set; }
}