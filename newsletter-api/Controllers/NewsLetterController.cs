using dotnet_core_api_w_postgres.Models;
using dotnet_core_api_w_postgres.Models.DTO;
using dotnet_core_api_w_postgres.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_core_api_w_postgres.Controllers;

[ApiController]
[Route("[controller]")]
public class NewsLetterController(INewsLetterService service): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<NewsLetterDto>>> Index()
    {
        var newsletters = await service.GetAllNewsLettersAsync();
        return Ok(newsletters);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<NewsLetterDto>> GetNewsLetterById(int id)
    {
        var newsletter = await service.GetNewsLetterByIdAsync(id);
        return Ok(newsletter);
    }
}