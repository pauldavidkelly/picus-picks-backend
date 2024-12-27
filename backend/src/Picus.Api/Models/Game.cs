namespace Picus.Api.Models;

public class Game : BaseEntity
{
    public string ExternalGameId { get; set; } = string.Empty;
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public DateTime GameTime { get; set; }
    public DateTime PickDeadline { get; set; }
    public int Week { get; set; }
    public int Season { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsPlayoffs { get; set; }
    public string Location { get; set; } = string.Empty;
    public int? HomeTeamScore { get; set; }
    public int? AwayTeamScore { get; set; }
    public int? WinningTeamId { get; set; }
    
    // Navigation properties
    public Team HomeTeam { get; set; } = null!;
    public Team AwayTeam { get; set; } = null!;
    public Team? WinningTeam { get; set; }
    public ICollection<Pick> Picks { get; set; } = new List<Pick>();
}
