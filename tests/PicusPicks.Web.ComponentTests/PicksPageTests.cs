using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PicusPicks.Web.Components.Pages;
using PicusPicks.Web.Services;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using PicusPicks.Web.Models.DTOs;

namespace PicusPicks.Web.ComponentTests;

public class PicksPageTests : TestContext
{
    private readonly Mock<IPicksService> _picksServiceMock;
    private readonly Mock<IGamesService> _gamesServiceMock;
    private readonly Mock<ILogger<Picks>> _loggerMock;
    private readonly Mock<IJSRuntime> _jsRuntimeMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IConfigurationSection> _featureFlagsSectionMock;

    public PicksPageTests()
    {
        _picksServiceMock = new Mock<IPicksService>();
        _gamesServiceMock = new Mock<IGamesService>();
        _loggerMock = new Mock<ILogger<Picks>>();
        _jsRuntimeMock = new Mock<IJSRuntime>();
        _configurationMock = new Mock<IConfiguration>();
        _featureFlagsSectionMock = new Mock<IConfigurationSection>();

        // Setup configuration mock for feature flags
        _featureFlagsSectionMock.Setup(x => x.Value).Returns("false");
        _configurationMock.Setup(x => x.GetSection("FeatureFlags:BypassPickDeadlines"))
            .Returns(_featureFlagsSectionMock.Object);

        Services.AddSingleton(_picksServiceMock.Object);
        Services.AddSingleton(_gamesServiceMock.Object);
        Services.AddSingleton(_loggerMock.Object);
        Services.AddSingleton(_jsRuntimeMock.Object);
        Services.AddSingleton(_configurationMock.Object);
    }

    [Fact]
    public async Task CorrectPicksCount_WithNoCompletedGames_ReturnsZero()
    {
        // Arrange
        var games = new List<GameDTO>
        {
            new() 
            { 
                Id = 1, 
                IsCompleted = false, 
                GameTime = DateTime.UtcNow.AddDays(1),
                HomeTeam = new TeamDTO { Id = 1, Name = "Team 1" },
                AwayTeam = new TeamDTO { Id = 2, Name = "Team 2" }
            },
            new() 
            { 
                Id = 2, 
                IsCompleted = false, 
                GameTime = DateTime.UtcNow.AddDays(1),
                HomeTeam = new TeamDTO { Id = 3, Name = "Team 3" },
                AwayTeam = new TeamDTO { Id = 4, Name = "Team 4" }
            }
        };

        var picks = new List<VisiblePickDto>
        {
            new() { GameId = 1, SelectedTeamId = 1 },
            new() { GameId = 2, SelectedTeamId = 2 }
        };

        var status = new PicksStatusDto
        {
            Week = 1,
            Season = 2024,
            TotalGames = 2,
            PicksMade = 2,
            IsComplete = true,
            GamesNeedingPicks = new List<int>()
        };

        var gamesTask = new TaskCompletionSource<IEnumerable<GameDTO>>();
        var picksTask = new TaskCompletionSource<(IEnumerable<VisiblePickDto>, PicksStatusDto)>();
        var statusTask = new TaskCompletionSource<PicksStatusDto>();

        _gamesServiceMock.Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(gamesTask.Task);

        _picksServiceMock.Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(picksTask.Task);

        _picksServiceMock.Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(statusTask.Task);

        // Act
        var cut = RenderComponent<Picks>();

        // Complete tasks in sequence
        await cut.InvokeAsync(() => gamesTask.SetResult(games));
        await Task.Delay(100); // Wait for games to process

        await cut.InvokeAsync(() => picksTask.SetResult((picks, status)));
        await Task.Delay(100); // Wait for picks to process

        await cut.InvokeAsync(() => statusTask.SetResult(status));
        await Task.Delay(100); // Wait for status to process

        // Force a re-render
        cut.Render();
        await Task.Delay(100);

        // Assert
        var statusText = cut.Find(".status-text").TextContent;
        Assert.Contains("(0 CORRECT)", statusText);
    }

    [Fact]
    public void CorrectPicksCount_WithCompletedGames_ReturnsCorrectCount()
    {
        // Arrange
        var games = new List<GameDTO>
        {
            new() 
            { 
                Id = 1, 
                IsCompleted = true,
                HomeTeamScore = 24,
                AwayTeamScore = 17,
                HomeTeam = new TeamDTO { Id = 1 },
                AwayTeam = new TeamDTO { Id = 2 },
                WinningTeam = new TeamDTO { Id = 1 }
            },
            new() 
            { 
                Id = 2, 
                IsCompleted = true,
                HomeTeamScore = 14,
                AwayTeamScore = 28,
                HomeTeam = new TeamDTO { Id = 3 },
                AwayTeam = new TeamDTO { Id = 4 },
                WinningTeam = new TeamDTO { Id = 4 }
            }
        };

        var picks = new List<VisiblePickDto>
        {
            new() { GameId = 1, SelectedTeamId = 1 }, // Correct pick (home team won)
            new() { GameId = 2, SelectedTeamId = 3 }  // Incorrect pick (away team won)
        };

        var status = new PicksStatusDto
        {
            Week = 1,
            Season = 2024,
            TotalGames = 2,
            PicksMade = 2,
            IsComplete = true,
            GamesNeedingPicks = new List<int>()
        };

        _gamesServiceMock.Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games.AsEnumerable());

        _picksServiceMock.Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((picks, status));

        _picksServiceMock.Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(status);

        // Act
        var cut = RenderComponent<Picks>();

        // Assert
        var statusText = cut.Find(".status-text").TextContent;
        Assert.Contains("(1 CORRECT)", statusText);
    }

    [Fact]
    public void CorrectPicksCount_WithMixedGameStates_OnlyCountsCompletedGames()
    {
        // Arrange
        var games = new List<GameDTO>
        {
            new() 
            { 
                Id = 1, 
                IsCompleted = true,
                HomeTeamScore = 24,
                AwayTeamScore = 17,
                HomeTeam = new TeamDTO { Id = 1 },
                AwayTeam = new TeamDTO { Id = 2 },
                WinningTeam = new TeamDTO { Id = 1 }
            },
            new() 
            { 
                Id = 2, 
                IsCompleted = false,
                HomeTeam = new TeamDTO { Id = 3 },
                AwayTeam = new TeamDTO { Id = 4 }
            }
        };

        var picks = new List<VisiblePickDto>
        {
            new() { GameId = 1, SelectedTeamId = 1 }, // Correct pick for completed game
            new() { GameId = 2, SelectedTeamId = 3 }  // Pick for incomplete game (shouldn't count)
        };

        var status = new PicksStatusDto
        {
            Week = 1,
            Season = 2024,
            TotalGames = 2,
            PicksMade = 2,
            IsComplete = true,
            GamesNeedingPicks = new List<int>()
        };

        _gamesServiceMock.Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games.AsEnumerable());

        _picksServiceMock.Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((picks, status));

        _picksServiceMock.Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(status);

        // Act
        var cut = RenderComponent<Picks>();

        // Assert
        var statusText = cut.Find(".status-text").TextContent;
        Assert.Contains("(1 CORRECT)", statusText);
    }
}
