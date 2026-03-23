using Microsoft.AspNetCore.Mvc;

namespace dotnet_core_api_w_postgres.Controllers;

[ApiController]
[Route("[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public ActionResult Ping()
    {
        return Ok("Pong");
    }
}