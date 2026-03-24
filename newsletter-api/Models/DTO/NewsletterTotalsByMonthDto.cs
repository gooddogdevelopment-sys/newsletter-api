namespace dotnet_core_api_w_postgres.Models.DTO;

public class NewsletterTotalsByMonthDto
{
    public required string Month { get; set; }
    public required int Count { get; set; }
}