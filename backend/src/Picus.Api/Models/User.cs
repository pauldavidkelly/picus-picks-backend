namespace Picus.Api.Models;

public class User : BaseEntity
{
    public string Auth0Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; } = true;
    public string Role { get; set; } = "Player";
    public int? LeagueId { get; set; }
    
    // Navigation properties
    public League? League { get; set; }
    public ICollection<Pick> Picks { get; set; } = new List<Pick>();
}
