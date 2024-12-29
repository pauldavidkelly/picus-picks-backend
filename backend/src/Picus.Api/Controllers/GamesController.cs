using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Picus.Api.Services;
using Picus.Api.Models.DTOs;

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
    [ProducesResponseType(typeof(List<GameDTO>), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Gets a specific game by its ID
    /// </summary>
    /// <param name="id">The ID of the game to retrieve</param>
    /// <returns>The game if found, or NotFound if not found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GameDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGameById(int id)
    {
        var game = await _gameService.GetGameByIdAsync(id);
        
        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }
}
