using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Picus.Api.Data;

namespace Picus.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly PicusDbContext _context;

        public TeamsController(PicusDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _context.Teams.ToListAsync();
            return Ok(new { Count = teams.Count, Teams = teams });
        }
    }
}
