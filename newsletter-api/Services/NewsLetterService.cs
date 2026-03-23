using dotnet_core_api_w_postgres.Data;
using dotnet_core_api_w_postgres.Models;
using dotnet_core_api_w_postgres.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace dotnet_core_api_w_postgres.Services;

public interface INewsLetterService
{
    Task<List<NewsLetterDto>> GetAllNewsLettersAsync();   
}

public class NewsLetterService (AppDbContext context) : INewsLetterService
{
    public async Task<List<NewsLetterDto>> GetAllNewsLettersAsync()
    {
        return await context.NewsLetters.Select(x => new NewsLetterDto
        {
            Id = x.Id,
            Title = x.Title,
            SendDate = x.SendDate,
            Overview = x.Overview.Substring(0, 50),
            Subject = x.Subject
        }).ToListAsync();
    }
}