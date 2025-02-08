using Xunit;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using PicusPicks.Web.Components.Pages;
using PicusPicks.Web.Models.DTOs;
using PicusPicks.Web.Services;
using PicusPicks.Web.Tests.Helpers;

namespace PicusPicks.Web.Tests.Components.Pages;

public class GamesTests : TestContext
{
    private readonly Mock<IGamesService> _gamesServiceMock;
    private readonly Mock<ILogger<Games>> _loggerMock;
    private readonly Mock<IJSRuntime> _jsRuntimeMock;

    public GamesTests()
    {
        _gamesServiceMock = new Mock<IGamesService>();
        _loggerMock = new Mock<ILogger<Games>>();
        _jsRuntimeMock = new Mock<IJSRuntime>();

        Services.AddScoped<IGamesService>(_ => _gamesServiceMock.Object);
        Services.AddScoped<ILogger<Games>>(_ => _loggerMock.Object);
        Services.AddScoped<IJSRuntime>(_ => _jsRuntimeMock.Object);
    }

    [Fact]
    public void InitialLoad_ShowsLoadingState()
    {
        // Arrange
        var loadingTask = new TaskCompletionSource<IEnumerable<GameDTO>>();
        _gamesServiceMock
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
        var sampleGames = TestData.GetTestGames();
        _gamesServiceMock
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(sampleGames);

        // Act
        var cut = RenderComponent<Games>();

        // Assert
        var gameCards = cut.FindAll(".card");
        Assert.Single(gameCards);

        // Verify team names are displayed
        var teamNames = cut.FindAll(".team-name");
        Assert.Contains(teamNames, x => x.TextContent.Contains("Buffalo"));
        Assert.Contains(teamNames, x => x.TextContent.Contains("Kansas City"));
    }

    [Fact]
    public void LoadGames_ShowsError_WhenLoadFails()
    {
        // Arrange
        _gamesServiceMock
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
        var sampleGames = TestData.GetTestGames();
        _gamesServiceMock
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(sampleGames);

        var cut = RenderComponent<Games>();

        // Act
        var select = cut.Find("select");
        select.Change("21");

        // Assert
        _gamesServiceMock.Verify(
            x => x.GetGamesByWeekAndSeasonAsync(21, 2024),
            Times.Once);
    }

    [Fact]
    public void SyncGames_ShowsLoadingState_WhenSyncing()
    {
        // Arrange
        var syncTask = new TaskCompletionSource<IEnumerable<GameDTO>>();
        _gamesServiceMock
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
        var sampleGames = TestData.GetTestGames();
        _gamesServiceMock
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

    [Theory]
    [InlineData("2025-01-15T08:31:49Z", 19)]  // Wild Card
    [InlineData("2025-01-22T08:31:49Z", 20)]  // Divisional
    [InlineData("2025-01-29T08:31:49Z", 21)]  // Conference
    [InlineData("2025-02-12T08:31:49Z", 22)]  // Super Bowl
    [InlineData("2024-09-05T08:31:49Z", 1)]   // Regular Season Week 1
    [InlineData("2024-09-12T08:31:49Z", 2)]   // Regular Season Week 2
    [InlineData("2024-09-19T08:31:49Z", 3)]   // Regular Season Week 3
    [InlineData("2024-09-26T08:31:49Z", 4)]   // Regular Season Week 4
    [InlineData("2024-07-01T08:31:49Z", 1)]   // Off-season
    public void InitialWeek_SetsCorrectWeekBasedOnDate(string currentDate, int expectedWeek)
    {
        // Arrange
        var games = TestData.GetTestGames();
        _gamesServiceMock
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(expectedWeek, 2024))
            .ReturnsAsync(games);

        // Act
        var cut = RenderComponent<Games>(parameters => parameters
            .Add(p => p.CurrentDateOverride, DateTimeOffset.Parse(currentDate)));

        // Assert
        var weekSelect = cut.Find("select.form-select");
        Assert.Equal(expectedWeek.ToString(), weekSelect.GetAttribute("value"));
    }

    [Fact]
    public void WeekNavigation_LoadsNewData_WhenChanged()
    {
        // Arrange
        var games = TestData.GetTestGames();
        _gamesServiceMock
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games);

        // Set initial date to Wild Card week
        var cut = RenderComponent<Games>(parameters => parameters
            .Add(p => p.CurrentDateOverride, DateTimeOffset.Parse("2025-01-15T08:31:49Z")));

        // Act - Change to Divisional week
        var weekSelect = cut.Find("select.form-select");
        weekSelect.Change("20");

        // Assert
        _gamesServiceMock.Verify(
            x => x.GetGamesByWeekAndSeasonAsync(20, 2024),
            Times.Once);
    }
} 