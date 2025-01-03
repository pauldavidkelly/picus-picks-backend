namespace Picus.Api.Models.DTOs;

/// <summary>
/// DTO for displaying pick information with visibility rules applied
/// </summary>
public class VisiblePickDto
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int? SelectedTeamId { get; set; }  // Null if pick isn't visible yet
    public bool HasPick { get; set; }         // True if user has made a pick
    public bool IsVisible { get; set; }       // True if pick details can be shown
}

/// <summary>
/// DTO for displaying pick status for a given week
/// </summary>
public class PicksStatusDto
{
    public int Week { get; set; }
    public int Season { get; set; }
    public int TotalGames { get; set; }
    public int PicksMade { get; set; }
    public bool IsComplete { get; set; }
    public List<int> GamesNeedingPicks { get; set; } = new();
}

/// <summary>
/// DTO for displaying all picks in a league for a given week
/// </summary>
public class LeaguePicksDto
{
    public int LeagueId { get; set; }
    public string LeagueName { get; set; } = string.Empty;
    public int Week { get; set; }
    public int Season { get; set; }
    public List<UserPicksDto> UserPicks { get; set; } = new();
}

/// <summary>
/// DTO for displaying a user's picks
/// </summary>
public class UserPicksDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<VisiblePickDto> Picks { get; set; } = new();
}

/// <summary>
/// DTO for submitting a new pick
/// </summary>
public class SubmitPickDto
{
    public int GameId { get; set; }
    public int SelectedTeamId { get; set; }
    public string? Notes { get; set; }
}
