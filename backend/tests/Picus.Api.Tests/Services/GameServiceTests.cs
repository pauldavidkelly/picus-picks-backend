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
                AwayScore = "14"
            },
            new()
            {
                Id = "game2",
                Date = "2024-01-02",
                Time = "19:30:00",
                StrVenue = "Stadium 2",
                HomeScore = null,
                AwayScore = null
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

        var game2 = games.First(g => g.ExternalGameId == "game2");
        Assert.Equal("Stadium 2", game2.Location);
        Assert.Null(game2.HomeTeamScore);
        Assert.Null(game2.AwayTeamScore);
        Assert.False(game2.IsCompleted);
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
            GameTime = DateTime.Parse("2024-01-01 20:00:00"),
            PickDeadline = DateTime.Parse("2024-01-01 20:00:00"),
            HomeTeamScore = null,
            AwayTeamScore = null,
            IsCompleted = false
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
                AwayScore = "21"
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
            GameTime = DateTime.Parse("2024-01-01 20:00:00"),
            PickDeadline = DateTime.Parse("2024-01-01 20:00:00"),
            HomeTeamScore = null,
            AwayTeamScore = null,
            IsCompleted = false
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
                AwayScore = "21"
            },
            new()
            {
                Id = "game2",
                Date = "2024-01-02",
                Time = "19:30:00",
                StrVenue = "New Stadium",
                HomeScore = null,
                AwayScore = null
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

        var newGame = games.First(g => g.ExternalGameId == "game2");
        Assert.Equal("New Stadium", newGame.Location);
        Assert.Null(newGame.HomeTeamScore);
        Assert.Null(newGame.AwayTeamScore);
        Assert.False(newGame.IsCompleted);
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
