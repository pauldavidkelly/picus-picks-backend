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
}
