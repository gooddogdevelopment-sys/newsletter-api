using System.ComponentModel.DataAnnotations;

namespace dotnet_core_api_w_postgres.Models;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
}