using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using PicusPicks.Web.Components.Pages;
using PicusPicks.Web.Models;
using PicusPicks.Web.Services;
using PicusPicks.Web.Tests.Helpers;

namespace PicusPicks.Web.Tests.Components.Pages;

public class GamesTests : TestContext
{
    private readonly Mock<IGamesService> _mockGamesService;
    private readonly Mock<ILogger<Games>> _mockLogger;
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public GamesTests()
    {
        _mockGamesService = new Mock<IGamesService>();
        _mockLogger = new Mock<ILogger<Games>>();
        _mockJsRuntime = new Mock<IJSRuntime>();

        Services.AddScoped<IGamesService>(_ => _mockGamesService.Object);
        Services.AddScoped<ILogger<Games>>(_ => _mockLogger.Object);
        Services.AddScoped<IJSRuntime>(_ => _mockJsRuntime.Object);
    }

    [Fact]
    public void InitialLoad_ShowsLoadingState()
    {
        // Arrange
        var loadingTask = new TaskCompletionSource<IEnumerable<GameDTO>>();
        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(loadingTask.Task);

        // Act
        var cut = RenderComponent<Games>();

        // Assert
        var spinner = cut.Find(".spinner-border");
        Assert.NotNull(spinner);
    }

    [Fact]
    public void LoadGames_DisplaysGames_WhenLoadSucceeds()
    {
        // Arrange
        var sampleGames = TestData.GetSampleGames();
        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(sampleGames);

        // Act
        var cut = RenderComponent<Games>();

        // Assert
        var gameCards = cut.FindAll(".card");
        Assert.Equal(2, gameCards.Count);

        // Verify team names are displayed
        var teamNames = cut.FindAll(".team-name");
        Assert.Contains(teamNames, x => x.TextContent.Contains("Buffalo"));
        Assert.Contains(teamNames, x => x.TextContent.Contains("Kansas City"));
    }

    [Fact]
    public void LoadGames_ShowsError_WhenLoadFails()
    {
        // Arrange
        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Test error"));

        // Act
        var cut = RenderComponent<Games>();

        // Assert
        var errorAlert = cut.Find(".alert-danger");
        Assert.NotNull(errorAlert);
        Assert.Contains("Failed to load games", errorAlert.TextContent);
    }

    [Fact]
    public void WeekSelection_LoadsNewGames_WhenChanged()
    {
        // Arrange
        var sampleGames = TestData.GetSampleGames();
        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(sampleGames);

        var cut = RenderComponent<Games>();

        // Act
        var select = cut.Find("select");
        select.Change("21");

        // Assert
        _mockGamesService.Verify(
            x => x.GetGamesByWeekAndSeasonAsync(21, 2024),
            Times.Once);
    }

    [Fact]
    public void SyncGames_ShowsLoadingState_WhenSyncing()
    {
        // Arrange
        var syncTask = new TaskCompletionSource<IEnumerable<GameDTO>>();
        _mockGamesService
            .Setup(x => x.SyncGamesAsync(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(syncTask.Task);

        var cut = RenderComponent<Games>();

        // Act
        var syncButton = cut.Find("button");
        syncButton.Click();

        // Assert
        Assert.Contains("Syncing", syncButton.TextContent);
        Assert.True(syncButton.HasAttribute("disabled"));
    }

    [Fact]
    public void SyncGames_ShowsSuccess_WhenSyncCompletes()
    {
        // Arrange
        var sampleGames = TestData.GetSampleGames();
        _mockGamesService
            .Setup(x => x.SyncGamesAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(sampleGames);

        var cut = RenderComponent<Games>();

        // Act
        var syncButton = cut.Find("button");
        syncButton.Click();

        // Assert
        var successAlert = cut.Find(".alert-success");
        Assert.NotNull(successAlert);
        Assert.Contains("Successfully synced", successAlert.TextContent);
    }
} 