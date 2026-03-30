using dotnet_core_api_w_postgres.Data;
using dotnet_core_api_w_postgres.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace dotnet_core_api_w_postgres.Services;

public interface INewsLetterService
{
    Task<List<NewsLetterDto>> GetAllNewsLettersAsync();
    Task<NewsLetterDto> GetNewsLetterByIdAsync(int id);
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
            Overview = x.Overview.Substring(0, 150),
            Subject = x.Subject
        }).OrderByDescending(x=>x.SendDate).ToListAsync();
    }

    public async Task<NewsLetterDto> GetNewsLetterByIdAsync(int id)
    {
        var newsLetter = await context.NewsLetters.Select(x=> new NewsLetterDto
        {
            Id = x.Id,
            Title = x.Title,
            SendDate = x.SendDate,
            Overview = x.Overview,
            Subject = x.Subject,
            CodeSnippet = x.CodeSnippet
        }).FirstOrDefaultAsync(x => x.Id == id);
        return newsLetter ?? throw new Exception("NewsLetter not found");
    }
}
