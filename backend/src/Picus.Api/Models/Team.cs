using Picus.Api.Models.Enums;

namespace Picus.Api.Models;

public class Team : BaseEntity
{
    public string ESPNTeamId { get; set; } = string.Empty;
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
    
    // Navigation properties
    public ICollection<Game> HomeGames { get; set; } = new List<Game>();
    public ICollection<Game> AwayGames { get; set; } = new List<Game>();
    public ICollection<Pick> Picks { get; set; } = new List<Pick>();
}
