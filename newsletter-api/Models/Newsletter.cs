using System.ComponentModel.DataAnnotations;

namespace dotnet_core_api_w_postgres.Models;

public class Newsletter : BaseEntity
{
    [MaxLength(60)]
    public required string Title { get; init; }

    [MaxLength(3000)]
    public string? CodeSnippet { get; set; }

    [MaxLength(1000)]
    public required string Overview { get; set; }

    [MaxLength(100)]
    public required string Subject { get; set; }
    public required DateTime SendDate { get; set; }

    [MaxLength(50)]
    public required string AiProvider { get; set; }

    [MaxLength(50)]
    public required string AiModel { get; set; }
}
