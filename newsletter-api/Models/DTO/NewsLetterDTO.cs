namespace dotnet_core_api_w_postgres.Models.DTO;

public class NewsLetterDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required DateTime SendDate { get; set; }
    public required string Overview { get; set; }
    public required string Subject { get; set; }
    public string? CodeSnippet { get; set; }
}