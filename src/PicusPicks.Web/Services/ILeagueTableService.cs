using PicusPicks.Web.Models;

namespace PicusPicks.Web.Services;

/// <summary>
/// Service for managing league table statistics
/// </summary>
public interface ILeagueTableService
{
    /// <summary>
    /// Gets the league table statistics for all users
    /// </summary>
    /// <returns>A list of user statistics ordered by success rate (descending)</returns>
    Task<IEnumerable<LeagueTableStats>> GetLeagueTableStatsAsync();
}
