namespace Picus.Api.Models;

public class Game : BaseEntity
{
    public string ESPNGameId { get; set; } = string.Empty;
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
    public DateTime GameTime { get; set; }
    public DateTime PickDeadline { get; set; }
    public string? FinalScore { get; set; }
    public int Week { get; set; }
    public int Season { get; set; }
    
    // Navigation properties
    public Team HomeTeam { get; set; } = null!;
    public Team AwayTeam { get; set; } = null!;
    public ICollection<Pick> Picks { get; set; } = new List<Pick>();
}
