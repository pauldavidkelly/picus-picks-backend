using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Picus.Api.Data;
using Picus.Api.Models;
using Picus.Api.Models.DTOs;
using Picus.Api.Services;
using Xunit;

namespace Picus.Api.Tests.Services;

public class PickServiceTests
{
    private readonly DbContextOptions<PicusDbContext> _options;
    private readonly Mock<ILogger<PickService>> _loggerMock;

    public PickServiceTests()
    {
        _options = new DbContextOptionsBuilder<PicusDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _loggerMock = new Mock<ILogger<PickService>>();

        // Initialize database with test data
        using var context = new PicusDbContext(_options);
        context.Database.EnsureCreated();
    }

    private async Task SeedTestGame(PicusDbContext context, Game game)
    {
        // First add the teams if they don't exist
        if (!await context.Teams.AnyAsync(t => t.Id == game.HomeTeamId))
        {
            context.Teams.Add(new Team { Id = game.HomeTeamId, Name = "Home Team" });
        }
        if (!await context.Teams.AnyAsync(t => t.Id == game.AwayTeamId))
        {
            context.Teams.Add(new Team { Id = game.AwayTeamId, Name = "Away Team" });
        }
        
        context.Games.Add(game);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task SubmitPickAsync_ValidPick_ReturnsCreatedPick()
    {
        // Arrange
        using var context = new PicusDbContext(_options);
        var service = new PickService(context, _loggerMock.Object);

        var game = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            PickDeadline = DateTime.UtcNow.AddHours(1)
        };
        await SeedTestGame(context, game);

        var pickDto = new SubmitPickDto
        {
            GameId = 1,
            SelectedTeamId = 1,
            Notes = "Test pick"
        };

        // Act
        var result = await service.SubmitPickAsync(1, pickDto);

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
        using var context = new PicusDbContext(_options);
        var service = new PickService(context, _loggerMock.Object);

        var game = new Game
        {
            Id = 1,
            HomeTeamId = 1,
            AwayTeamId = 2,
            PickDeadline = DateTime.UtcNow.AddHours(-1)
        };
        await SeedTestGame(context, game);

        var pickDto = new SubmitPickDto
        {
            GameId = 1,
            SelectedTeamId = 1
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SubmitPickAsync(1, pickDto));
        Assert.Contains("deadline has passed", exception.Message);
    }

    [Fact]
    public async Task GetUserPicksByWeekAsync_ReturnsUserPicks()
    {
        // Arrange
        using var context = new PicusDbContext(_options);
        var service = new PickService(context, _loggerMock.Object);

        var game = new Game
        {
            Id = 1,
            Week = 1,
            Season = 2024,
            HomeTeamId = 1,
            AwayTeamId = 2
        };
        await SeedTestGame(context, game);

        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1
        };
        context.Picks.Add(pick);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetUserPicksByWeekAsync(1, 1, 2024);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().UserId);
    }

    [Fact]
    public async Task GetLeaguePicksByWeekAsync_ReturnsLeaguePicks()
    {
        // Arrange
        using var context = new PicusDbContext(_options);
        var service = new PickService(context, _loggerMock.Object);

        var user = new User { Id = 1, LeagueId = 1 };
        var game = new Game
        {
            Id = 1,
            Week = 1,
            Season = 2024,
            HomeTeamId = 1,
            AwayTeamId = 2
        };
        await SeedTestGame(context, game);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var pick = new Pick
        {
            UserId = 1,
            GameId = 1,
            SelectedTeamId = 1
        };
        context.Picks.Add(pick);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetLeaguePicksByWeekAsync(1, 1, 2024);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().UserId);
    }

    [Fact]
    public async Task ApplyPickVisibilityRulesAsync_HidesPastDeadlinePicks()
    {
        // Arrange
        using var context = new PicusDbContext(_options);
        var service = new PickService(context, _loggerMock.Object);

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
        context.Games.Add(game);
        await context.SaveChangesAsync();

        // Act
        var result = await service.ApplyPickVisibilityRulesAsync(new[] { pick });

        // Assert
        var visiblePick = result.First();
        Assert.False(visiblePick.IsVisible);
        Assert.Null(visiblePick.SelectedTeamId);
    }

    [Fact]
    public async Task GetPickStatusAsync_ReturnsCorrectStatus()
    {
        // Arrange
        using var context = new PicusDbContext(_options);
        var service = new PickService(context, _loggerMock.Object);

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
        context.Games.AddRange(game1, game2);
        context.Picks.Add(pick);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetPickStatusAsync(1, 1, 2024);

        // Assert
        Assert.Equal(2, result.TotalGames);
        Assert.Equal(1, result.PicksMade);
        Assert.False(result.IsComplete);
        Assert.Single(result.GamesNeedingPicks);
    }
}
