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

    public PickServiceTests()
    {
        _logger = new Mock<ILogger<PickService>>();
        _configuration = new Mock<IConfiguration>();
        _configSection = new Mock<IConfigurationSection>();

        // Setup configuration
        _configuration
            .Setup(c => c.GetSection("FeatureFlags:BypassPickDeadlines"))
            .Returns(_configSection.Object);

        _service = new PickService(Context, _logger.Object, _configuration.Object);
    }

    private async Task SeedTestGame(Game game)
    {
        // First add the teams if they don't exist
        if (!await Context.Teams.AnyAsync(t => t.Id == game.HomeTeamId))
        {
            Context.Teams.Add(new Team { Id = game.HomeTeamId, Name = "Home Team" });
        }
        if (!await Context.Teams.AnyAsync(t => t.Id == game.AwayTeamId))
        {
            Context.Teams.Add(new Team { Id = game.AwayTeamId, Name = "Away Team" });
        }
        
        Context.Games.Add(game);
        await Context.SaveChangesAsync();
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
        Context.Games.Add(game);
        await Context.SaveChangesAsync();

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
        Context.Games.Add(game);
        await Context.SaveChangesAsync();

        var pickDto = new SubmitPickDto { GameId = 1, SelectedTeamId = 1 };
        _configSection.Setup(c => c.Value).Returns("true");

        // Act
        var result = await _service.SubmitPickAsync(1, pickDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.GameId);
        Assert.Equal(1, result.SelectedTeamId);
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
        Context.Picks.Add(pick);
        await Context.SaveChangesAsync();

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

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1
        };
        Context.Picks.Add(pick);
        await Context.SaveChangesAsync();

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
        Context.Games.Add(game);
        await Context.SaveChangesAsync();

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
        Context.Games.AddRange(game1, game2);
        Context.Picks.Add(pick);
        await Context.SaveChangesAsync();

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
        var user1 = new User { Id = 1, Name = "User 1" };
        var user2 = new User { Id = 2, Name = "User 2" };
        var game = new Game
        {
            Id = 1,
            Week = 1,
            Season = 2024,
            HomeTeamId = 1,
            AwayTeamId = 2
        };
        await SeedTestGame(game);

        Context.Users.AddRange(user1, user2);
        await Context.SaveChangesAsync();

        var picks = new[]
        {
            new Pick { UserId = 1, GameId = 1, SelectedTeamId = 1 },
            new Pick { UserId = 2, GameId = 1, SelectedTeamId = 2 }
        };
        Context.Picks.AddRange(picks);
        await Context.SaveChangesAsync();

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
        var user1 = new User { Id = 1, DisplayName = "User1" };
        var user2 = new User { Id = 2, DisplayName = "User2" };
        Context.Users.AddRange(user1, user2);

        var game1 = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            WinningTeamId = 1,
            IsCompleted = true
        };
        var game2 = new Game
        {
            Id = 2,
            HomeTeamId = 1,
            AwayTeamId = 2,
            WinningTeamId = 2,
            IsCompleted = true
        };
        var game3 = new Game
        {
            Id = 3,
            HomeTeamId = 1,
            AwayTeamId = 2,
            IsCompleted = false
        };
        await SeedTestGame(game1);
        await SeedTestGame(game2);
        await SeedTestGame(game3);

        // User1: 2/2 correct picks (100%)
        var pick1 = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1 // Correct
        };
        var pick2 = new Pick
        {
            UserId = 1,
            GameId = 2,
            SelectedTeamId = 2 // Correct
        };
        var pick3 = new Pick
        {
            UserId = 1,
            GameId = 3,
            SelectedTeamId = 1 // Not counted (game not completed)
        };

        // User2: 1/2 correct picks (50%)
        var pick4 = new Pick
        {
            UserId = 2,
            GameId = 1,
            SelectedTeamId = 1 // Correct
        };
        var pick5 = new Pick
        {
            UserId = 2,
            GameId = 2,
            SelectedTeamId = 1 // Incorrect
        };

        Context.Picks.AddRange(pick1, pick2, pick3, pick4, pick5);
        await Context.SaveChangesAsync();

        // Act
        var result = (await _service.GetLeagueTableStatsAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        // User1 should be first with 100% success rate
        Assert.Equal("User1", result[0].DisplayName);
        Assert.Equal(2, result[0].CorrectPicks);
        Assert.Equal(2, result[0].TotalPicks);
        Assert.Equal(100m, result[0].SuccessRate);

        // User2 should be second with 50% success rate
        Assert.Equal("User2", result[1].DisplayName);
        Assert.Equal(1, result[1].CorrectPicks);
        Assert.Equal(2, result[1].TotalPicks);
        Assert.Equal(50m, result[1].SuccessRate);
    }

    [Fact]
    public async Task GetLeagueTableStatsAsync_HandlesNoCompletedGames()
    {
        // Arrange
        var user = new User { Id = 1, DisplayName = "User1" };
        Context.Users.Add(user);

        var game = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            IsCompleted = false
        };
        await SeedTestGame(game);

        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1
        };
        Context.Picks.Add(pick);
        await Context.SaveChangesAsync();

        // Act
        var result = await _service.GetLeagueTableStatsAsync();

        // Assert
        Assert.Single(result);
        var stats = result.First();
        Assert.Equal("User1", stats.DisplayName);
        Assert.Equal(0, stats.CorrectPicks);
        Assert.Equal(0, stats.TotalPicks);
        Assert.Equal(0m, stats.SuccessRate);
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
