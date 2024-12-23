namespace Picus.Api.Models;

public class Pick : BaseEntity
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int SelectedTeamId { get; set; }
    public bool? IsCorrect { get; set; }
    public int Points { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public Game Game { get; set; } = null!;
    public Team SelectedTeam { get; set; } = null!;
}
