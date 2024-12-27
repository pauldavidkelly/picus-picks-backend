using Picus.Api.Models.Enums;
using System.Text.Json.Serialization;

namespace Picus.Api.Models;

public class Team : BaseEntity
{
    public string ExternalTeamId { get; set; } = string.Empty;
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
    [JsonIgnore]
    public ICollection<Game> HomeGames { get; set; } = new List<Game>();
    [JsonIgnore]
    public ICollection<Game> AwayGames { get; set; } = new List<Game>();
    [JsonIgnore]
    public ICollection<Pick> Picks { get; set; } = new List<Pick>();
}
