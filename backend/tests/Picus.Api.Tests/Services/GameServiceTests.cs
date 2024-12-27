using Microsoft.EntityFrameworkCore;
using Moq;
using Picus.Api.Data;
using Picus.Api.Models;
using Picus.Api.Models.SportsDb;
using Picus.Api.Services;
using Xunit;

namespace Picus.Api.Tests.Services;

public class GameServiceTests : TestBase
{
    private readonly Mock<ISportsDbService> _mockSportsDbService;
    private readonly IRepository<Models.Game> _gameRepository;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _mockSportsDbService = new Mock<ISportsDbService>();
        _gameRepository = new Repository<Models.Game>(_context);
        _gameService = new GameService(_gameRepository, _mockSportsDbService.Object, _context);

        // Add test teams to context
        var teams = new List<Models.Team>
        {
            new() { Id = 1, ExternalTeamId = "team1", Name = "Home Team" },
            new() { Id = 2, ExternalTeamId = "team2", Name = "Away Team" }
        };
        _context.Teams.AddRange(teams);
        _context.SaveChanges();
    }

    [Fact]
    public async Task UpsertGamesForSeasonAsync_WithNewGames_ShouldInsertGames()
    {
        // Arrange
        var leagueId = 1;
        var season = 2024;
        var sportsDbGames = new List<Models.SportsDb.Game>
        {
            new()
            {
                Id = "game1",
                Date = "2024-01-01",
                Time = "20:00:00",
                StrVenue = "Stadium 1",
                HomeScore = "21",
                AwayScore = "14",
                HomeTeamId = "team1",
                AwayTeamId = "team2",
                IntRound = "Week 17"
            },
            new()
            {
                Id = "game2",
                Date = "2024-01-02",
                Time = "19:30:00",
                StrVenue = "Stadium 2",
                HomeScore = null,
                AwayScore = null,
                HomeTeamId = "team2",
                AwayTeamId = "team1",
                IntRound = "Week 17"
            }
        };

        _mockSportsDbService
            .Setup(x => x.GetLeagueScheduleAsync(leagueId, season))
            .ReturnsAsync(sportsDbGames);

        // Act
        var result = await _gameService.UpsertGamesForSeasonAsync(leagueId, season);

        // Assert
        Assert.Equal(2, result.Count());
        var games = await _context.Games.ToListAsync();
        Assert.Equal(2, games.Count);
        
        var game1 = games.First(g => g.ExternalGameId == "game1");
        Assert.Equal("Stadium 1", game1.Location);
        Assert.Equal(21, game1.HomeTeamScore);
        Assert.Equal(14, game1.AwayTeamScore);
        Assert.True(game1.IsCompleted);
        Assert.Equal(1, game1.HomeTeamId);  // team1
        Assert.Equal(2, game1.AwayTeamId);  // team2
        Assert.Equal(17, game1.Week);
        Assert.Equal(2024, game1.Season);
        Assert.False(game1.IsPlayoffs);
        Assert.Equal(1, game1.WinningTeamId);  // Home team won

        var game2 = games.First(g => g.ExternalGameId == "game2");
        Assert.Equal("Stadium 2", game2.Location);
        Assert.Null(game2.HomeTeamScore);
        Assert.Null(game2.AwayTeamScore);
        Assert.False(game2.IsCompleted);
        Assert.Equal(2, game2.HomeTeamId);  // team2
        Assert.Equal(1, game2.AwayTeamId);  // team1
        Assert.Equal(17, game2.Week);
        Assert.Equal(2024, game2.Season);
        Assert.False(game2.IsPlayoffs);
        Assert.Null(game2.WinningTeamId);  // No winner yet
    }

    [Fact]
    public async Task UpsertGamesForSeasonAsync_WithExistingGames_ShouldUpdateGames()
    {
        // Arrange
        var leagueId = 1;
        var season = 2024;
        var existingGame = new Models.Game
        {
            ExternalGameId = "game1",
            Location = "Old Stadium",
            GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
            PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
            HomeTeamId = 1,
            AwayTeamId = 2,
            HomeTeamScore = null,
            AwayTeamScore = null,
            IsCompleted = false,
            Week = 17,
            Season = 2024,
            IsPlayoffs = false
        };
        await _context.Games.AddAsync(existingGame);
        await _context.SaveChangesAsync();

        var sportsDbGames = new List<Models.SportsDb.Game>
        {
            new()
            {
                Id = "game1",
                Date = "2024-01-01",
                Time = "20:00:00",
                StrVenue = "Updated Stadium",
                HomeScore = "28",
                AwayScore = "21",
                HomeTeamId = "team1",
                AwayTeamId = "team2",
                IntRound = "Week 17"
            }
        };

        _mockSportsDbService
            .Setup(x => x.GetLeagueScheduleAsync(leagueId, season))
            .ReturnsAsync(sportsDbGames);

        // Act
        var result = await _gameService.UpsertGamesForSeasonAsync(leagueId, season);

        // Assert
        Assert.Single(result);
        var games = await _context.Games.ToListAsync();
        Assert.Single(games);
        
        var updatedGame = games.First();
        Assert.Equal("Updated Stadium", updatedGame.Location);
        Assert.Equal(28, updatedGame.HomeTeamScore);
        Assert.Equal(21, updatedGame.AwayTeamScore);
        Assert.True(updatedGame.IsCompleted);
        Assert.Equal(1, updatedGame.HomeTeamId);
        Assert.Equal(2, updatedGame.AwayTeamId);
        Assert.Equal(17, updatedGame.Week);
        Assert.Equal(2024, updatedGame.Season);
        Assert.False(updatedGame.IsPlayoffs);
        Assert.Equal(1, updatedGame.WinningTeamId);  // Home team won
    }

    [Fact]
    public async Task UpsertGamesForSeasonAsync_WithMixOfNewAndExistingGames_ShouldUpsertCorrectly()
    {
        // Arrange
        var leagueId = 1;
        var season = 2024;
        var existingGame = new Models.Game
        {
            ExternalGameId = "game1",
            Location = "Old Stadium",
            GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
            PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
            HomeTeamId = 1,
            AwayTeamId = 2,
            HomeTeamScore = null,
            AwayTeamScore = null,
            IsCompleted = false,
            Week = 17,
            Season = 2024,
            IsPlayoffs = false
        };
        await _context.Games.AddAsync(existingGame);
        await _context.SaveChangesAsync();

        var sportsDbGames = new List<Models.SportsDb.Game>
        {
            new()
            {
                Id = "game1",
                Date = "2024-01-01",
                Time = "20:00:00",
                StrVenue = "Updated Stadium",
                HomeScore = "28",
                AwayScore = "21",
                HomeTeamId = "team1",
                AwayTeamId = "team2",
                IntRound = "Week 17"
            },
            new()
            {
                Id = "game2",
                Date = "2024-01-02",
                Time = "19:30:00",
                StrVenue = "New Stadium",
                HomeScore = null,
                AwayScore = null,
                HomeTeamId = "team2",
                AwayTeamId = "team1",
                IntRound = "Week 17"
            }
        };

        _mockSportsDbService
            .Setup(x => x.GetLeagueScheduleAsync(leagueId, season))
            .ReturnsAsync(sportsDbGames);

        // Act
        var result = await _gameService.UpsertGamesForSeasonAsync(leagueId, season);

        // Assert
        Assert.Equal(2, result.Count());
        var games = await _context.Games.ToListAsync();
        Assert.Equal(2, games.Count);
        
        var updatedGame = games.First(g => g.ExternalGameId == "game1");
        Assert.Equal("Updated Stadium", updatedGame.Location);
        Assert.Equal(28, updatedGame.HomeTeamScore);
        Assert.Equal(21, updatedGame.AwayTeamScore);
        Assert.True(updatedGame.IsCompleted);
        Assert.Equal(1, updatedGame.HomeTeamId);
        Assert.Equal(2, updatedGame.AwayTeamId);
        Assert.Equal(17, updatedGame.Week);
        Assert.Equal(2024, updatedGame.Season);
        Assert.False(updatedGame.IsPlayoffs);
        Assert.Equal(1, updatedGame.WinningTeamId);  // Home team won

        var newGame = games.First(g => g.ExternalGameId == "game2");
        Assert.Equal("New Stadium", newGame.Location);
        Assert.Null(newGame.HomeTeamScore);
        Assert.Null(newGame.AwayTeamScore);
        Assert.False(newGame.IsCompleted);
        Assert.Equal(2, newGame.HomeTeamId);  // team2
        Assert.Equal(1, newGame.AwayTeamId);  // team1
        Assert.Equal(17, newGame.Week);
        Assert.Equal(2024, newGame.Season);
        Assert.False(newGame.IsPlayoffs);
        Assert.Null(newGame.WinningTeamId);  // No winner yet
    }

    [Fact]
    public async Task UpsertGamesForSeasonAsync_WithEmptyResponse_ShouldReturnEmptyList()
    {
        // Arrange
        var leagueId = 1;
        var season = 2024;
        var sportsDbGames = new List<Models.SportsDb.Game>();

        _mockSportsDbService
            .Setup(x => x.GetLeagueScheduleAsync(leagueId, season))
            .ReturnsAsync(sportsDbGames);

        // Act
        var result = await _gameService.UpsertGamesForSeasonAsync(leagueId, season);

        // Assert
        Assert.Empty(result);
        var games = await _context.Games.ToListAsync();
        Assert.Empty(games);
    }
}
