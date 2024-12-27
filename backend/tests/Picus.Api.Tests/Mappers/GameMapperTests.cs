using Picus.Api.Mappers;
using Picus.Api.Models.SportsDb;
using Xunit;

namespace Picus.Api.Tests.Mappers;

public class GameMapperTests
{
    [Fact]
    public void ToGameEntity_WithValidData_MapsCorrectly()
    {
        // Arrange
        var sportsDbGame = new Game
        {
            Id = "12345",
            Date = "2024-12-27",
            Time = "15:30:00",
            StrVenue = "Test Stadium",
            HomeScore = "21",
            AwayScore = "14"
        };

        // Act
        var result = sportsDbGame.ToGameEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("12345", result.ESPNGameId);
        Assert.Equal("Test Stadium", result.Location);
        Assert.Equal(21, result.HomeTeamScore);
        Assert.Equal(14, result.AwayTeamScore);
        Assert.True(result.IsCompleted);
        Assert.Equal(new DateTime(2024, 12, 27, 15, 30, 0), result.GameTime);
    }

    [Fact]
    public void ToGameEntity_WithNullScores_MapsAsIncomplete()
    {
        // Arrange
        var sportsDbGame = new Game
        {
            Id = "12345",
            Date = "2024-12-27",
            Time = "15:30:00",
            StrVenue = "Test Stadium",
            HomeScore = null,
            AwayScore = null
        };

        // Act
        var result = sportsDbGame.ToGameEntity();

        // Assert
        Assert.Null(result.HomeTeamScore);
        Assert.Null(result.AwayTeamScore);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public void ToGameEntity_WithEmptyTime_DefaultsToMidnight()
    {
        // Arrange
        var sportsDbGame = new Game
        {
            Id = "12345",
            Date = "2024-12-27",
            Time = "",
            StrVenue = "Test Stadium"
        };

        // Act
        var result = sportsDbGame.ToGameEntity();

        // Assert
        Assert.Equal(new DateTime(2024, 12, 27, 0, 0, 0), result.GameTime);
    }

    [Fact]
    public void ToGameEntity_WithNullInput_ThrowsArgumentNullException()
    {
        // Arrange
        Game? sportsDbGame = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => sportsDbGame!.ToGameEntity());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ToGameEntity_WithInvalidDate_ThrowsArgumentException(string date)
    {
        // Arrange
        var sportsDbGame = new Game
        {
            Id = "12345",
            Date = date,
            Time = "15:30:00"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => sportsDbGame.ToGameEntity());
    }
}
