namespace Picus.Api.Models;

public class User : BaseEntity
{
    public string Auth0Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = "Player";
    public int? LeagueId { get; set; }
    
    // Navigation properties
    public League? League { get; set; }
    public ICollection<Pick> Picks { get; set; } = new List<Pick>();
}
