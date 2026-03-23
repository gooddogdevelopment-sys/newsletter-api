using System.ComponentModel.DataAnnotations;

namespace dotnet_core_api_w_postgres.Models;

public class Newsletter : BaseEntity
{
    [MaxLength(60)]
    public required string Title { get; set; }
    public string? CodeSnippet { get; set; }
    public required string Overview { get; set; }
    public required string Subject { get; set; }
    public required DateTime SendDate { get; set; }
    public required string AiProvider { get; set; }
    public required string AiModel { get; set; }
}