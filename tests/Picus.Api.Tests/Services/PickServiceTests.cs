using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Picus.Api.Data;
using Picus.Api.Models;
using Picus.Api.Models.DTOs;
using Picus.Api.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Picus.Api.Tests.Services;

public class PickServiceTests : TestBase
{
    private readonly Mock<ILogger<PickService>> _logger;
    private readonly Mock<IConfiguration> _configuration;
    private readonly Mock<IConfigurationSection> _configSection;
    private readonly PickService _service;
    private readonly Mock<IGameService> _gameService;

    public PickServiceTests()
    {
        _logger = new Mock<ILogger<PickService>>();
        _configuration = new Mock<IConfiguration>();
        _configSection = new Mock<IConfigurationSection>();
        _gameService = new Mock<IGameService>();

        // Setup configuration
        _configuration
            .Setup(c => c.GetSection("FeatureFlags:BypassPickDeadlines"))
            .Returns(_configSection.Object);

        _service = new PickService(_context, _gameService.Object, _logger.Object, _configuration.Object);
    }

    private async Task SeedTestGame(Game game)
    {
        // First add the teams if they don't exist
        if (!await _context.Teams.AnyAsync(t => t.Id == game.HomeTeamId))
        {
            _context.Teams.Add(new Team { Id = game.HomeTeamId, Name = "Home Team", City = "Home City" });
        }
        if (!await _context.Teams.AnyAsync(t => t.Id == game.AwayTeamId))
        {
            _context.Teams.Add(new Team { Id = game.AwayTeamId, Name = "Away Team", City = "Away City" });
        }
        
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task SubmitPickAsync_ThrowsException_WhenGameNotFound()
    {
        // Arrange
        var pickDto = new SubmitPickDto { GameId = 999, SelectedTeamId = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SubmitPickAsync(1, pickDto));
    }

    [Fact]
    public async Task SubmitPickAsync_ThrowsException_WhenDeadlinePassedAndBypassDisabled()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            PickDeadline = DateTime.UtcNow.AddMinutes(-30)
        };
        await SeedTestGame(game);

        var pickDto = new SubmitPickDto { GameId = 1, SelectedTeamId = 1 };
        _configSection.Setup(c => c.Value).Returns("false");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SubmitPickAsync(1, pickDto));
    }

    [Fact]
    public async Task SubmitPickAsync_Succeeds_WhenDeadlinePassedButBypassEnabled()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            PickDeadline = DateTime.UtcNow.AddMinutes(-30)
        };
        await SeedTestGame(game);

        var pickDto = new SubmitPickDto { GameId = 1, SelectedTeamId = 1 };
        _configSection.Setup(c => c.Value).Returns("true");

        // Act
        await _service.SubmitPickAsync(1, pickDto);

        // Assert
        var pick = await _context.Picks.FirstOrDefaultAsync(p => p.GameId == 1);
        Assert.NotNull(pick);
        Assert.Equal(1, pick.SelectedTeamId);
    }

    [Fact]
    public async Task SubmitPickAsync_ValidPick_ReturnsCreatedPick()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            PickDeadline = DateTime.UtcNow.AddHours(1)
        };
        await SeedTestGame(game);

        var pickDto = new SubmitPickDto
        {
            GameId = 1,
            SelectedTeamId = 1,
            Notes = "Test pick"
        };

        // Act
        var result = await _service.SubmitPickAsync(1, pickDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal(1, result.GameId);
        Assert.Equal(1, result.SelectedTeamId);
        Assert.Equal("Test pick", result.Notes);
    }

    [Fact]
    public async Task SubmitPickAsync_PastDeadline_ThrowsInvalidOperationException()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            PickDeadline = DateTime.UtcNow.AddHours(-1)
        };
        await SeedTestGame(game);

        var pickDto = new SubmitPickDto
        {
            GameId = 1,
            SelectedTeamId = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.SubmitPickAsync(1, pickDto));
        Assert.Contains("deadline has passed", exception.Message);
    }

    [Fact]
    public async Task GetUserPicksByWeekAsync_ReturnsUserPicks()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            Week = 1,
            Season = 2024,
            HomeTeamId = 1,
            AwayTeamId = 2
        };
        await SeedTestGame(game);

        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1
        };
        _context.Picks.Add(pick);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetUserPicksByWeekAsync(1, 1, 2024);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().UserId);
    }

    [Fact]
    public async Task GetLeaguePicksByWeekAsync_ReturnsLeaguePicks()
    {
        // Arrange
        var user = new User { Id = 1, LeagueId = 1 };
        var game = new Game
        {
            Id = 1,
            Week = 1,
            Season = 2024,
            HomeTeamId = 1,
            AwayTeamId = 2
        };
        await SeedTestGame(game);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1
        };
        _context.Picks.Add(pick);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLeaguePicksByWeekAsync(1, 1, 2024);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().UserId);
    }

    [Fact]
    public async Task ApplyPickVisibilityRulesAsync_HidesPastDeadlinePicks()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            PickDeadline = DateTime.UtcNow.AddHours(1)
        };
        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1,
            Game = game
        };
        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ApplyPickVisibilityRulesAsync(new[] { pick });

        // Assert
        var visiblePick = result.First();
        Assert.False(visiblePick.IsVisible);
        Assert.Null(visiblePick.SelectedTeamId);
    }

    [Fact]
    public async Task GetPickStatusAsync_ReturnsCorrectStatus()
    {
        // Arrange
        var game1 = new Game
        {
            Id = 1,
            Week = 1,
            Season = 2024,
            PickDeadline = DateTime.UtcNow.AddHours(1)
        };
        var game2 = new Game
        {
            Id = 2,
            Week = 1,
            Season = 2024,
            PickDeadline = DateTime.UtcNow.AddHours(1)
        };
        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            Game = game1
        };
        _context.Games.AddRange(game1, game2);
        _context.Picks.Add(pick);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPickStatusAsync(1, 1, 2024);

        // Assert
        Assert.Equal(2, result.TotalGames);
        Assert.Equal(1, result.PicksMade);
        Assert.False(result.IsComplete);
        Assert.Single(result.GamesNeedingPicks);
    }

    [Fact]
    public async Task GetAllPicksByWeekAsync_ReturnsAllPicks()
    {
        // Arrange
        var user1 = new User { Id = 1, DisplayName = "User 1", Username = "user1" };
        var user2 = new User { Id = 2, DisplayName = "User 2", Username = "user2" };
        var game = new Game
        {
            Id = 1,
            Week = 1,
            Season = 2024,
            HomeTeamId = 1,
            AwayTeamId = 2
        };
        await SeedTestGame(game);

        _context.Users.AddRange(user1, user2);
        await _context.SaveChangesAsync();

        var picks = new[]
        {
            new Pick { UserId = 1, GameId = 1, SelectedTeamId = 1 },
            new Pick { UserId = 2, GameId = 1, SelectedTeamId = 2 }
        };
        _context.Picks.AddRange(picks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllPicksByWeekAsync(1, 2024);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.UserId == 1 && p.SelectedTeamId == 1);
        Assert.Contains(result, p => p.UserId == 2 && p.SelectedTeamId == 2);
    }

    [Fact]
    public async Task GetAllPicksByWeekAsync_ReturnsEmptyList_WhenNoPicksExist()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            Week = 1,
            Season = 2024,
            HomeTeamId = 1,
            AwayTeamId = 2
        };
        await SeedTestGame(game);

        // Act
        var result = await _service.GetAllPicksByWeekAsync(1, 2024);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLeagueTableStatsAsync_ReturnsCorrectStats()
    {
        // Arrange
        var user1 = new User { Id = 1, DisplayName = "User 1" };
        var user2 = new User { Id = 2, DisplayName = "User 2" };
        _context.Users.AddRange(user1, user2);

        var game1 = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            IsCompleted = true,
            WinningTeamId = 1
        };
        await SeedTestGame(game1);

        var game2 = new Game
        {
            Id = 2,
            HomeTeamId = 3,
            AwayTeamId = 4,
            IsCompleted = true,
            WinningTeamId = 3
        };
        await SeedTestGame(game2);

        var picks = new List<Pick>
        {
            new() { UserId = 1, GameId = 1, SelectedTeamId = 1 }, // Correct pick
            new() { UserId = 1, GameId = 2, SelectedTeamId = 3 }, // Correct pick
            new() { UserId = 2, GameId = 1, SelectedTeamId = 2 }  // Incorrect pick
        };
        _context.Picks.AddRange(picks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLeagueTableStatsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        var user1Stats = result.First(s => s.DisplayName == "User 1");
        var user2Stats = result.First(s => s.DisplayName == "User 2");
        Assert.Equal(2, user1Stats.CorrectPicks);
        Assert.Equal(2, user1Stats.TotalPicks);
        Assert.Equal(100m, user1Stats.SuccessRate);
        Assert.Equal(0, user2Stats.CorrectPicks);
        Assert.Equal(1, user2Stats.TotalPicks);
        Assert.Equal(0m, user2Stats.SuccessRate);
    }

    [Fact]
    public async Task GetLeagueTableStatsAsync_HandlesNoCompletedGames()
    {
        // Arrange
        var user = new User { Id = 1, DisplayName = "User1" };
        _context.Users.Add(user);

        var game = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            IsCompleted = false,
            WinningTeamId = null
        };
        await SeedTestGame(game);

        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1
        };
        _context.Picks.Add(pick);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetLeagueTableStatsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLeagueTableStatsAsync_HandlesNoUsers()
    {
        // Act
        var result = await _service.GetLeagueTableStatsAsync();

        // Assert
        Assert.Empty(result);
    }
}
