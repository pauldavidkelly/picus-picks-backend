using System.Text.Json.Serialization;

namespace Picus.Api.Models.SportsDb
{
    public class Game
    {
        [JsonPropertyName("idEvent")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("strEvent")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("dateEvent")]
        public string Date { get; set; } = string.Empty;

        [JsonPropertyName("strTime")]
        public string Time { get; set; } = string.Empty;

        [JsonPropertyName("idHomeTeam")]
        public string HomeTeamId { get; set; } = string.Empty;

        [JsonPropertyName("idAwayTeam")]
        public string AwayTeamId { get; set; } = string.Empty;

        [JsonPropertyName("strHomeTeam")]
        public string HomeTeamName { get; set; } = string.Empty;

        [JsonPropertyName("strAwayTeam")]
        public string AwayTeamName { get; set; } = string.Empty;

        [JsonPropertyName("intHomeScore")]
        public string? HomeScore { get; set; }

        [JsonPropertyName("intAwayScore")]
        public string? AwayScore { get; set; }

        public string StrSport { get; set; }
        public string IntRound { get; set; }
        public DateTime StrTimestamp { get; set; }
        public string DateEventLocal { get; set; }
        public string StrTimeLocal { get; set; }
        public string StrHomeTeamBadge { get; set; }
        public string StrAwayTeamBadge { get; set; }
        public string StrVenue { get; set; }
        public string StrCountry { get; set; }
        public string StrThumb { get; set; }
    }
}