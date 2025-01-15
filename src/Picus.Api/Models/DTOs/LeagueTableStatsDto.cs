using System.ComponentModel.DataAnnotations;

namespace Picus.Api.Models.DTOs;

public class LeagueTableStatsDto
{
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int CorrectPicks { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalPicks { get; set; }

    public decimal SuccessRate => TotalPicks > 0 ? (decimal)CorrectPicks / TotalPicks * 100 : 0;
}
