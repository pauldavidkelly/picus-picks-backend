using Picus.Api.Mappers;
using Picus.Api.Models;
using Picus.Api.Models.Enums;
using Xunit;

namespace Picus.Api.Tests.Mappers;

public class DTOMapperTests
{
    [Fact]
    public void ToTeamDTO_MapsAllProperties()
    {
        // Arrange
        var team = new Team
        {
            Id = 1,
            Name = "Test Team",
            Abbreviation = "TST",
            City = "Test City",
            IconUrl = "http://test.com/icon.png",
            BannerUrl = "http://test.com/banner.png",
            PrimaryColor = "#000000",
            SecondaryColor = "#FFFFFF",
            TertiaryColor = "#FF0000",
            Conference = ConferenceType.AFC,
            Division = DivisionType.East
        };

        // Act
        var dto = team.ToTeamDTO();

        // Assert
        Assert.Equal(team.Id, dto.Id);
        Assert.Equal(team.Name, dto.Name);
        Assert.Equal(team.Abbreviation, dto.Abbreviation);
        Assert.Equal(team.City, dto.City);
        Assert.Equal(team.IconUrl, dto.IconUrl);
        Assert.Equal(team.BannerUrl, dto.BannerUrl);
        Assert.Equal(team.PrimaryColor, dto.PrimaryColor);
        Assert.Equal(team.SecondaryColor, dto.SecondaryColor);
        Assert.Equal(team.TertiaryColor, dto.TertiaryColor);
        Assert.Equal(team.Conference, dto.Conference);
        Assert.Equal(team.Division, dto.Division);
    }

    [Fact]
    public void ToGameDTO_MapsAllProperties()
    {
        // Arrange
        var homeTeam = new Team { Id = 1, Name = "Home Team" };
        var awayTeam = new Team { Id = 2, Name = "Away Team" };
        var winningTeam = new Team { Id = 1, Name = "Home Team" };

        var game = new Game
        {
            Id = 1,
            ExternalGameId = "EXT123",
            GameTime = DateTime.UtcNow,
            PickDeadline = DateTime.UtcNow.AddHours(-1),
            Week = 1,
            Season = 2024,
            IsCompleted = true,
            IsPlayoffs = false,
            Location = "Test Stadium",
            HomeTeamScore = 24,
            AwayTeamScore = 17,
            HomeTeam = homeTeam,
            AwayTeam = awayTeam,
            WinningTeam = winningTeam
        };

        // Act
        var dto = game.ToGameDTO();

        // Assert
        Assert.Equal(game.Id, dto.Id);
        Assert.Equal(game.ExternalGameId, dto.ExternalGameId);
        Assert.Equal(game.GameTime, dto.GameTime);
        Assert.Equal(game.PickDeadline, dto.PickDeadline);
        Assert.Equal(game.Week, dto.Week);
        Assert.Equal(game.Season, dto.Season);
        Assert.Equal(game.IsCompleted, dto.IsCompleted);
        Assert.Equal(game.IsPlayoffs, dto.IsPlayoffs);
        Assert.Equal(game.Location, dto.Location);
        Assert.Equal(game.HomeTeamScore, dto.HomeTeamScore);
        Assert.Equal(game.AwayTeamScore, dto.AwayTeamScore);

        // Verify team mappings
        Assert.Equal(game.HomeTeam.Id, dto.HomeTeam.Id);
        Assert.Equal(game.HomeTeam.Name, dto.HomeTeam.Name);
        Assert.Equal(game.AwayTeam.Id, dto.AwayTeam.Id);
        Assert.Equal(game.AwayTeam.Name, dto.AwayTeam.Name);
        Assert.Equal(game.WinningTeam?.Id, dto.WinningTeam?.Id);
        Assert.Equal(game.WinningTeam?.Name, dto.WinningTeam?.Name);
    }

    [Fact]
    public void ToGameDTO_WithNullWinningTeam_MapsCorrectly()
    {
        // Arrange
        var homeTeam = new Team { Id = 1, Name = "Home Team" };
        var awayTeam = new Team { Id = 2, Name = "Away Team" };

        var game = new Game
        {
            Id = 1,
            HomeTeam = homeTeam,
            AwayTeam = awayTeam,
            WinningTeam = null
        };

        // Act
        var dto = game.ToGameDTO();

        // Assert
        Assert.NotNull(dto.HomeTeam);
        Assert.NotNull(dto.AwayTeam);
        Assert.Null(dto.WinningTeam);
    }
}
