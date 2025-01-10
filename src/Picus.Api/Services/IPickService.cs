using Picus.Api.Models;
using Picus.Api.Models.DTOs;

namespace Picus.Api.Services;

public interface IPickService
{
    /// <summary>
    /// Submit a pick for a game
    /// </summary>
    Task<Pick> SubmitPickAsync(int userId, SubmitPickDto pickDto);
    
    /// <summary>
    /// Get picks for a specific user for a given week and season
    /// </summary>
    Task<IEnumerable<Pick>> GetUserPicksByWeekAsync(int userId, int week, int season);
    
    /// <summary>
    /// Get all picks for a league for a given week and season
    /// </summary>
    Task<IEnumerable<Pick>> GetLeaguePicksByWeekAsync(int leagueId, int week, int season);
    
    /// <summary>
    /// Apply visibility rules to picks based on game deadlines
    /// </summary>
    Task<IEnumerable<VisiblePickDto>> ApplyPickVisibilityRulesAsync(IEnumerable<Pick> picks);
    
    /// <summary>
    /// Check if a user belongs to a league
    /// </summary>
    Task<bool> UserBelongsToLeagueAsync(int userId, int leagueId);
    
    /// <summary>
    /// Get pick status for a user for a given week
    /// </summary>
    Task<PicksStatusDto> GetPickStatusAsync(int userId, int week, int season);
}
