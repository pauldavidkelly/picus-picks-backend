using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Picus.Api.Models.DTOs;
using Picus.Api.Services;
using System.Security.Claims;

namespace Picus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PicksController : ControllerBase
{
    private readonly IPickService _pickService;
    private readonly IUserService _userService;
    private readonly ILogger<PicksController> _logger;

    public PicksController(IPickService pickService, IUserService userService, ILogger<PicksController> logger)
    {
        _pickService = pickService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Submit a pick for a game
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SubmitPick([FromBody] SubmitPickDto pickDto)
    {
        try
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found");
            var user = await _userService.GetUserByAuth0IdAsync(auth0Id) ?? throw new InvalidOperationException("User not found");
            var pick = await _pickService.SubmitPickAsync(user.Id, pickDto);
            return Ok(pick);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid pick submission attempt");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid pick submission attempt");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting pick");
            return StatusCode(500, new { message = "An error occurred while submitting your pick" });
        }
    }

    /// <summary>
    /// Get picks for the current user for a specific week
    /// </summary>
    [HttpGet("my-picks/week/{week}/season/{season}")]
    public async Task<IActionResult> GetMyPicksForWeek(int week, int season)
    {
        try
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found");
            var user = await _userService.GetUserByAuth0IdAsync(auth0Id) ?? throw new InvalidOperationException("User not found");
            var picks = await _pickService.GetUserPicksByWeekAsync(user.Id, week, season);
            var status = await _pickService.GetPickStatusAsync(user.Id, week, season);
            
            return Ok(new { picks, status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user's picks");
            return StatusCode(500, new { message = "An error occurred while retrieving your picks" });
        }
    }

    /// <summary>
    /// Get all picks for a league for a specific week
    /// </summary>
    [HttpGet("league/{leagueId}/week/{week}/season/{season}")]
    public async Task<IActionResult> GetLeaguePicksForWeek(int leagueId, int week, int season)
    {
        try
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found");
            var user = await _userService.GetUserByAuth0IdAsync(auth0Id) ?? throw new InvalidOperationException("User not found");
            
            if (!await _pickService.UserBelongsToLeagueAsync(user.Id, leagueId))
            {
                return Forbid();
            }

            var picks = await _pickService.GetLeaguePicksByWeekAsync(leagueId, week, season);
            var visiblePicks = await _pickService.ApplyPickVisibilityRulesAsync(picks);
            
            return Ok(visiblePicks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving league picks");
            return StatusCode(500, new { message = "An error occurred while retrieving league picks" });
        }
    }

    /// <summary>
    /// Get pick status for current user for a specific week
    /// </summary>
    [HttpGet("status/week/{week}/season/{season}")]
    public async Task<IActionResult> GetPickStatus(int week, int season)
    {
        try
        {
            var auth0Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("User ID not found");
            var user = await _userService.GetUserByAuth0IdAsync(auth0Id) ?? throw new InvalidOperationException("User not found");
            var status = await _pickService.GetPickStatusAsync(user.Id, week, season);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pick status");
            return StatusCode(500, new { message = "An error occurred while retrieving pick status" });
        }
    }
}
