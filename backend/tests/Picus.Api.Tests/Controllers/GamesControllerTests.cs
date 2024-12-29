using Microsoft.AspNetCore.Mvc;
using Moq;
using Picus.Api.Controllers;
using Picus.Api.Models;
using Picus.Api.Services;
using Xunit;

namespace Picus.Api.Tests.Controllers;

public class GamesControllerTests
{
    private readonly Mock<IGameService> _mockGameService;
    private readonly GamesController _controller;

    public GamesControllerTests()
    {
        _mockGameService = new Mock<IGameService>();
        _controller = new GamesController(_mockGameService.Object);
    }

    [Fact]
    public async Task GetGameById_WithExistingGame_ReturnsOkResult()
    {
        // Arrange
        var game = new Game
        {
            Id = 1,
            ExternalGameId = "game1",
            Location = "Test Stadium",
            GameTime = DateTime.UtcNow,
            PickDeadline = DateTime.UtcNow,
            HomeTeamId = 1,
            AwayTeamId = 2,
            HomeTeamScore = 21,
            AwayTeamScore = 14,
            IsCompleted = true,
            Week = 17,
            Season = 2023,
            IsPlayoffs = false,
            HomeTeam = new Team { Id = 1, Name = "Home Team" },
            AwayTeam = new Team { Id = 2, Name = "Away Team" }
        };

        _mockGameService.Setup(s => s.GetGameByIdAsync(1))
            .ReturnsAsync(game);

        // Act
        var result = await _controller.GetGameById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<Game>(okResult.Value);
        Assert.Equal(1, returnedGame.Id);
        Assert.Equal("Test Stadium", returnedGame.Location);
        Assert.Equal(21, returnedGame.HomeTeamScore);
        Assert.Equal(14, returnedGame.AwayTeamScore);
    }

    [Fact]
    public async Task GetGameById_WithNonExistingGame_ReturnsNotFound()
    {
        // Arrange
        _mockGameService.Setup(s => s.GetGameByIdAsync(999))
            .ReturnsAsync((Game?)null);

        // Act
        var result = await _controller.GetGameById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
