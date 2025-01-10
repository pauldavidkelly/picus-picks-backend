using System.Text.Json.Serialization;
using Picus.Api.Models.Enums;

namespace Picus.Api.Models.DTOs;

public class GameDTO
{
    public int Id { get; set; }
    public string ExternalGameId { get; set; } = string.Empty;
    public DateTime GameTime { get; set; }
    public DateTime PickDeadline { get; set; }
    public int Week { get; set; }
    public int Season { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPlayoffs { get; set; }
    public string Location { get; set; } = string.Empty;
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    
    public TeamDTO HomeTeam { get; set; } = null!;
    public TeamDTO AwayTeam { get; set; } = null!;
    public TeamDTO? WinningTeam { get; set; }
}

public class TeamDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string BannerUrl { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public string TertiaryColor { get; set; } = string.Empty;
    public ConferenceType Conference { get; set; }
    public DivisionType Division { get; set; }
}
