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
            AwayScore = "14",
            IntRound = "Week 17"
        };

        // Act
        var result = sportsDbGame.ToGameEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("12345", result.ExternalGameId);
        Assert.Equal("Test Stadium", result.Location);
        Assert.Equal(21, result.HomeTeamScore);
        Assert.Equal(14, result.AwayTeamScore);
        Assert.True(result.IsCompleted);
        Assert.Equal(DateTime.SpecifyKind(new DateTime(2024, 12, 27, 15, 30, 0), DateTimeKind.Utc), result.GameTime);
        Assert.Equal(17, result.Week);
        Assert.Equal(2024, result.Season);
        Assert.False(result.IsPlayoffs);
    }

    [Fact]
    public void ToGameEntity_WithPlayoffGame_MapsCorrectly()
    {
        // Arrange
        var sportsDbGame = new Game
        {
            Id = "12345",
            Date = "2024-01-20",  // January game in playoffs
            Time = "15:30:00",
            StrVenue = "Test Stadium",
            HomeScore = "21",
            AwayScore = "14",
            IntRound = "Week 19"  // Playoff week
        };

        // Act
        var result = sportsDbGame.ToGameEntity();

        // Assert
        Assert.True(result.IsPlayoffs);
        Assert.Equal(19, result.Week);
        Assert.Equal(2023, result.Season);  // January 2024 game belongs to 2023 season
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
    [InlineData((string?)null)]
    public void ToGameEntity_WithInvalidDate_ThrowsArgumentException(string? date)
    {
        // Arrange
        var sportsDbGame = new Game
        {
            Id = "12345",
            Date = date ?? string.Empty,
            Time = "15:30:00"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => sportsDbGame.ToGameEntity());
    }

    [Theory]
    [InlineData("160", 19, true)]  // Wild Card Round
    [InlineData("161", 20, true)]  // Divisional Round
    [InlineData("162", 21, true)]  // Conference Championships
    [InlineData("163", 22, true)]  // Super Bowl
    [InlineData("Week 17", 17, false)]  // Regular season
    [InlineData("500", 500, false)]  // Preseason
    public void ToGameEntity_HandlesWeekAndPlayoffStatus_Correctly(string intRound, int expectedWeek, bool expectedIsPlayoffs)
    {
        // Arrange
        var sportsDbGame = new Game
        {
            Id = "12345",
            Date = "2024-01-20",
            Time = "15:30:00",
            StrVenue = "Test Stadium",
            IntRound = intRound
        };

        // Act
        var result = sportsDbGame.ToGameEntity();

        // Assert
        Assert.Equal(expectedWeek, result.Week);
        Assert.Equal(expectedIsPlayoffs, result.IsPlayoffs);
    }
}
