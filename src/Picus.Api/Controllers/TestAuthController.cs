using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Picus.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestAuthController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok(new { message = "You are authenticated!" });
    }
}
