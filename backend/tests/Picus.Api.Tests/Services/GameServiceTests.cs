using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly Mock<ILogger<Repository<Models.Game>>> _mockLogger;
    private readonly IRepository<Models.Game> _gameRepository;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _mockSportsDbService = new Mock<ISportsDbService>();
        _mockLogger = new Mock<ILogger<Repository<Models.Game>>>();
        _gameRepository = new Repository<Models.Game>(_context, _mockLogger.Object);
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
        var season = 2023;
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
        Assert.Equal(2023, game1.Season);
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
        Assert.Equal(2023, game2.Season);
        Assert.False(game2.IsPlayoffs);
        Assert.Null(game2.WinningTeamId);  // No winner yet
    }

    [Fact]
    public async Task UpsertGamesForSeasonAsync_WithExistingGames_ShouldUpdateGames()
    {
        // Arrange
        var leagueId = 1;
        var season = 2023;
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
            Season = 2023,
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
        Assert.Equal(2023, updatedGame.Season);
        Assert.False(updatedGame.IsPlayoffs);
        Assert.Equal(1, updatedGame.WinningTeamId);  // Home team won
    }

    [Fact]
    public async Task UpsertGamesForSeasonAsync_WithMixOfNewAndExistingGames_ShouldUpsertCorrectly()
    {
        // Arrange
        var leagueId = 1;
        var season = 2023;
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
            Season = 2023,
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
        Assert.Equal(2023, updatedGame.Season);
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
        Assert.Equal(2023, newGame.Season);
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

    [Fact]
    public async Task UpsertGamesForSeasonAsync_WithPlayoffGames_ShouldHandleCorrectly()
    {
        // Arrange
        var leagueId = 1;
        var season = 2023;
        var sportsDbGames = new List<Models.SportsDb.Game>
        {
            new()
            {
                Id = "playoff1",
                Date = "2024-01-13",  // Wild Card weekend
                Time = "20:00:00",
                StrVenue = "Stadium 1",
                HomeScore = "28",
                AwayScore = "21",
                HomeTeamId = "team1",
                AwayTeamId = "team2",
                IntRound = "160"  // Wild Card Round
            },
            new()
            {
                Id = "playoff2",
                Date = "2024-01-20",  // Divisional round
                Time = "19:30:00",
                StrVenue = "Stadium 2",
                HomeScore = "35",
                AwayScore = "17",
                HomeTeamId = "team2",
                AwayTeamId = "team1",
                IntRound = "161"  // Divisional Round
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
        
        var wildCardGame = games.First(g => g.ExternalGameId == "playoff1");
        Assert.Equal("Stadium 1", wildCardGame.Location);
        Assert.Equal(28, wildCardGame.HomeTeamScore);
        Assert.Equal(21, wildCardGame.AwayTeamScore);
        Assert.True(wildCardGame.IsCompleted);
        Assert.Equal(1, wildCardGame.HomeTeamId);
        Assert.Equal(2, wildCardGame.AwayTeamId);
        Assert.Equal(19, wildCardGame.Week);  // Week 19 is Wild Card round
        Assert.Equal(2023, wildCardGame.Season);
        Assert.True(wildCardGame.IsPlayoffs);
        Assert.Equal(1, wildCardGame.WinningTeamId);

        var divisionalGame = games.First(g => g.ExternalGameId == "playoff2");
        Assert.Equal("Stadium 2", divisionalGame.Location);
        Assert.Equal(35, divisionalGame.HomeTeamScore);
        Assert.Equal(17, divisionalGame.AwayTeamScore);
        Assert.True(divisionalGame.IsCompleted);
        Assert.Equal(2, divisionalGame.HomeTeamId);
        Assert.Equal(1, divisionalGame.AwayTeamId);
        Assert.Equal(20, divisionalGame.Week);  // Week 20 is Divisional round
        Assert.Equal(2023, divisionalGame.Season);
        Assert.True(divisionalGame.IsPlayoffs);
        Assert.Equal(2, divisionalGame.WinningTeamId);
    }

    [Fact]
    public async Task GetGameByIdAsync_WithExistingGame_ShouldReturnGameDTO()
    {
        // Arrange
        var game = new Models.Game
        {
            ExternalGameId = "game1",
            Location = "Test Stadium",
            GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
            PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
            HomeTeamId = 1,
            AwayTeamId = 2,
            HomeTeamScore = 21,
            AwayTeamScore = 14,
            IsCompleted = true,
            Week = 17,
            Season = 2023,
            IsPlayoffs = false
        };
        await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();

        // Act
        var result = await _gameService.GetGameByIdAsync(game.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(game.Id, result.Id);
        Assert.Equal("Test Stadium", result.Location);
        Assert.Equal(21, result.HomeTeamScore);
        Assert.Equal(14, result.AwayTeamScore);
        Assert.NotNull(result.HomeTeam);
        Assert.NotNull(result.AwayTeam);
        Assert.Equal("Home Team", result.HomeTeam.Name);
        Assert.Equal("Away Team", result.AwayTeam.Name);
        Assert.Equal(1, result.HomeTeam.Id);
        Assert.Equal(2, result.AwayTeam.Id);
    }

    [Fact]
    public async Task GetGameByIdAsync_WithNonExistingGame_ShouldReturnNull()
    {
        // Act
        var result = await _gameService.GetGameByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetGamesByWeekAndSeasonAsync_WithExistingGames_ShouldReturnGames()
    {
        // Arrange
        var games = new List<Models.Game>
        {
            new()
            {
                ExternalGameId = "game1",
                Location = "Stadium 1",
                GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
                PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
                HomeTeamId = 1,
                AwayTeamId = 2,
                HomeTeamScore = 21,
                AwayTeamScore = 14,
                IsCompleted = true,
                Week = 17,
                Season = 2023,
                IsPlayoffs = false
            },
            new()
            {
                ExternalGameId = "game2",
                Location = "Stadium 2",
                GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 16:00:00"), DateTimeKind.Utc),
                PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 16:00:00"), DateTimeKind.Utc),
                HomeTeamId = 2,
                AwayTeamId = 1,
                HomeTeamScore = 28,
                AwayTeamScore = 35,
                IsCompleted = true,
                Week = 17,
                Season = 2023,
                IsPlayoffs = false
            },
            new() // Different week
            {
                ExternalGameId = "game3",
                Location = "Stadium 3",
                GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-01-08 20:00:00"), DateTimeKind.Utc),
                PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-01-08 20:00:00"), DateTimeKind.Utc),
                HomeTeamId = 1,
                AwayTeamId = 2,
                HomeTeamScore = null,
                AwayTeamScore = null,
                IsCompleted = false,
                Week = 18,
                Season = 2023,
                IsPlayoffs = false
            }
        };

        await _context.Games.AddRangeAsync(games);
        await _context.SaveChangesAsync();

        // Act
        var result = await _gameService.GetGamesByWeekAndSeasonAsync(17, 2023);

        // Assert
        var gamesList = result.ToList();
        Assert.Equal(2, gamesList.Count);
        Assert.All(gamesList, g => Assert.Equal(17, g.Week));
        Assert.All(gamesList, g => Assert.Equal(2023, g.Season));
        
        // Verify games are ordered by game time
        Assert.Equal("Stadium 2", gamesList[0].Location); // 16:00 game
        Assert.Equal("Stadium 1", gamesList[1].Location); // 20:00 game
        
        // Verify team details are included
        Assert.All(gamesList, g =>
        {
            Assert.NotNull(g.HomeTeam);
            Assert.NotNull(g.AwayTeam);
            Assert.NotNull(g.HomeTeam.Name);
            Assert.NotNull(g.AwayTeam.Name);
        });
    }

    [Fact]
    public async Task GetGamesByWeekAndSeasonAsync_WithNoGames_ShouldReturnEmptyList()
    {
        // Act
        var result = await _gameService.GetGamesByWeekAndSeasonAsync(1, 2023);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTeamGamesBySeasonAsync_WithExistingGames_ShouldReturnTeamGames()
    {
        // Arrange
        var games = new List<Models.Game>
        {
            new()
            {
                ExternalGameId = "game1",
                Location = "Stadium 1",
                GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
                PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-01-01 20:00:00"), DateTimeKind.Utc),
                HomeTeamId = 1,
                AwayTeamId = 2,
                HomeTeamScore = 21,
                AwayTeamScore = 14,
                IsCompleted = true,
                Week = 17,
                Season = 2023,
                IsPlayoffs = false
            },
            new()
            {
                ExternalGameId = "game2",
                Location = "Stadium 2",
                GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-01-08 16:00:00"), DateTimeKind.Utc),
                PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-01-08 16:00:00"), DateTimeKind.Utc),
                HomeTeamId = 2,
                AwayTeamId = 1,
                HomeTeamScore = 28,
                AwayTeamScore = 35,
                IsCompleted = true,
                Week = 18,
                Season = 2023,
                IsPlayoffs = false
            },
            new() // Different season
            {
                ExternalGameId = "game3",
                Location = "Stadium 3",
                GameTime = DateTime.SpecifyKind(DateTime.Parse("2024-09-08 20:00:00"), DateTimeKind.Utc),
                PickDeadline = DateTime.SpecifyKind(DateTime.Parse("2024-09-08 20:00:00"), DateTimeKind.Utc),
                HomeTeamId = 1,
                AwayTeamId = 2,
                HomeTeamScore = null,
                AwayTeamScore = null,
                IsCompleted = false,
                Week = 1,
                Season = 2024,
                IsPlayoffs = false
            }
        };

        await _context.Games.AddRangeAsync(games);
        await _context.SaveChangesAsync();

        // Act
        var result = await _gameService.GetTeamGamesBySeasonAsync(1, 2023);

        // Assert
        var gamesList = result.ToList();
        Assert.Equal(2, gamesList.Count);
        Assert.All(gamesList, g => Assert.Equal(2023, g.Season));
        Assert.All(gamesList, g => Assert.True(g.HomeTeam.Id == 1 || g.AwayTeam.Id == 1));
        
        // Verify games are ordered by week and game time
        Assert.Equal(17, gamesList[0].Week);
        Assert.Equal(18, gamesList[1].Week);
        
        // Verify team details are included
        Assert.All(gamesList, g =>
        {
            Assert.NotNull(g.HomeTeam);
            Assert.NotNull(g.AwayTeam);
            Assert.NotNull(g.HomeTeam.Name);
            Assert.NotNull(g.AwayTeam.Name);
        });
    }

    [Fact]
    public async Task GetTeamGamesBySeasonAsync_WithNoGames_ShouldReturnEmptyList()
    {
        // Act
        var result = await _gameService.GetTeamGamesBySeasonAsync(1, 2023);

        // Assert
        Assert.Empty(result);
    }
}
