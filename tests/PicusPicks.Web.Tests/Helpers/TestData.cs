using PicusPicks.Web.Models;

namespace PicusPicks.Web.Tests.Helpers;

public static class TestData
{
    public static IEnumerable<GameDTO> GetSampleGames()
    {
        return new List<GameDTO>
        {
            new GameDTO
            {
                Id = 1,
                ExternalGameId = "2024_1_KC_BUF",
                GameTime = DateTime.Parse("2024-01-21T20:00:00Z"),
                PickDeadline = DateTime.Parse("2024-01-21T19:45:00Z"),
                Week = 20,
                Season = 2024,
                IsCompleted = false,
                IsPlayoffs = true,
                Location = "Highmark Stadium, Orchard Park, NY",
                HomeTeamScore = null,
                AwayTeamScore = null,
                HomeTeam = new TeamDTO
                {
                    Id = 2,
                    Name = "Bills",
                    City = "Buffalo",
                    Abbreviation = "BUF",
                    IconUrl = "https://example.com/buf.png",
                    BannerUrl = "https://example.com/buf-banner.png",
                    PrimaryColor = "#C60C30",
                    SecondaryColor = "#00338D",
                    TertiaryColor = "#FFFFFF",
                    Conference = "AFC",
                    Division = "East"
                },
                AwayTeam = new TeamDTO
                {
                    Id = 1,
                    Name = "Chiefs",
                    City = "Kansas City",
                    Abbreviation = "KC",
                    IconUrl = "https://example.com/kc.png",
                    BannerUrl = "https://example.com/kc-banner.png",
                    PrimaryColor = "#E31837",
                    SecondaryColor = "#FFB81C",
                    TertiaryColor = "#FFFFFF",
                    Conference = "AFC",
                    Division = "West"
                }
            },
            new GameDTO
            {
                Id = 2,
                ExternalGameId = "2024_1_SF_GB",
                GameTime = DateTime.Parse("2024-01-20T20:00:00Z"),
                PickDeadline = DateTime.Parse("2024-01-20T19:45:00Z"),
                Week = 20,
                Season = 2024,
                IsCompleted = true,
                IsPlayoffs = true,
                Location = "Levi's Stadium, Santa Clara, CA",
                HomeTeamScore = 24,
                AwayTeamScore = 21,
                HomeTeam = new TeamDTO
                {
                    Id = 3,
                    Name = "49ers",
                    City = "San Francisco",
                    Abbreviation = "SF",
                    IconUrl = "https://example.com/sf.png",
                    BannerUrl = "https://example.com/sf-banner.png",
                    PrimaryColor = "#AA0000",
                    SecondaryColor = "#B3995D",
                    TertiaryColor = "#FFFFFF",
                    Conference = "NFC",
                    Division = "West"
                },
                AwayTeam = new TeamDTO
                {
                    Id = 4,
                    Name = "Packers",
                    City = "Green Bay",
                    Abbreviation = "GB",
                    IconUrl = "https://example.com/gb.png",
                    BannerUrl = "https://example.com/gb-banner.png",
                    PrimaryColor = "#203731",
                    SecondaryColor = "#FFB612",
                    TertiaryColor = "#FFFFFF",
                    Conference = "NFC",
                    Division = "North"
                }
            }
        };
    }
} 