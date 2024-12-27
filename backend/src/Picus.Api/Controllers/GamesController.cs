using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Picus.Api.Services;

namespace Picus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Updates or inserts games for a specific league and season from the SportsDb API
    /// </summary>
    /// <param name="leagueId">The ID of the league to fetch games for</param>
    /// <param name="season">The season year to fetch games for</param>
    /// <returns>A list of games that were updated or inserted</returns>
    [HttpPost("upsert/{leagueId}/{season}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpsertGames(int leagueId, int season)
    {
        try
        {
            var games = await _gameService.UpsertGamesForSeasonAsync(leagueId, season);
            return Ok(games);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
