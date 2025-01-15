using System.ComponentModel.DataAnnotations;

namespace PicusPicks.Web.Models;

/// <summary>
/// Represents a user's statistics for the league table display
/// </summary>
public class LeagueTableStats
{
    /// <summary>
    /// The display name of the user
    /// </summary>
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The number of correct picks made by the user
    /// </summary>
    [Range(0, int.MaxValue)]
    public int CorrectPicks { get; set; }

    /// <summary>
    /// The total number of picks made by the user
    /// </summary>
    [Range(0, int.MaxValue)]
    public int TotalPicks { get; set; }

    /// <summary>
    /// The success rate as a percentage (0-100)
    /// Calculated as (CorrectPicks / TotalPicks) * 100
    /// Returns 0 if TotalPicks is 0
    /// </summary>
    public decimal SuccessRate => TotalPicks == 0 ? 0 : Math.Round((decimal)CorrectPicks / TotalPicks * 100, 2);
}
