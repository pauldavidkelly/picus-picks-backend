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
            var games = await _gameService.UpsertGamesForSeasonAsync(leagueId, 2024);
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

    /// <summary>
    /// Gets all games for a specific week and season
    /// </summary>
    /// <param name="week">The week number</param>
    /// <param name="season">The season year</param>
    /// <returns>List of games for the specified week and season</returns>
    [HttpGet("week/{week}/season/{season}")]
    [ProducesResponseType(typeof(IEnumerable<GameDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetGamesByWeekAndSeason(int week, int season)
    {
        var games = await _gameService.GetGamesByWeekAndSeasonAsync(week, season);
        return Ok(games);
    }

    /// <summary>
    /// Gets all games for a specific team in a season
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="season">The season year</param>
    /// <returns>List of games for the specified team and season</returns>
    [HttpGet("team/{teamId}/season/{season}")]
    [ProducesResponseType(typeof(IEnumerable<GameDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTeamGamesBySeason(int teamId, int season)
    {
        var games = await _gameService.GetTeamGamesBySeasonAsync(teamId, season);
        return Ok(games);
    }
}
