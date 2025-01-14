using Xunit;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using PicusPicks.Web.Components.Pages;
using Picus.Api.Models.DTOs;
using PicusPicks.Web.Services;
using PicusPicks.Web.Tests.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PicusPicks.Web.Tests.Components.Pages;

public class PicksTests : TestContext
{
    private readonly Mock<IPicksService> _mockPicksService;
    private readonly Mock<IGamesService> _mockGamesService;
    private readonly Mock<ILogger<Picks>> _mockLogger;
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public PicksTests()
    {
        _mockPicksService = new Mock<IPicksService>();
        _mockGamesService = new Mock<IGamesService>();
        _mockLogger = new Mock<ILogger<Picks>>();
        _mockJsRuntime = new Mock<IJSRuntime>();

        Services.AddScoped<IPicksService>(_ => _mockPicksService.Object);
        Services.AddScoped<IGamesService>(_ => _mockGamesService.Object);
        Services.AddScoped<ILogger<Picks>>(_ => _mockLogger.Object);
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
        var cut = RenderComponent<Picks>();

        // Assert
        var spinner = cut.Find(".spinner-border");
        Assert.NotNull(spinner);
    }

    [Fact]
    public void LoadGames_DisplaysGamesAndPicks_WhenLoadSucceeds()
    {
        // Arrange
        var games = TestData.GetTestGames();
        var picks = new List<VisiblePickDto>
        {
            new VisiblePickDto
            {
                GameId = games.First().Id,
                SelectedTeamId = games.First().HomeTeam.Id,
                HasPick = true,
                IsVisible = true
            }
        };
        var status = new PicksStatusDto
        {
            Week = 1,
            Season = 2024,
            TotalGames = 1,
            PicksMade = 1,
            IsComplete = true,
            GamesNeedingPicks = new List<int>()
        };

        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games);

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((picks, status));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(status);

        // Act
        var cut = RenderComponent<Picks>();

        // Assert
        var gameCards = cut.FindAll(".game-card");
        Assert.Single(gameCards);

        var progressBar = cut.Find(".progress");
        Assert.NotNull(progressBar);
        Assert.Equal("100%", progressBar.GetAttribute("style")?.Split(':')[1].Trim());

        var selectedButton = cut.Find(".team-button.selected");
        Assert.NotNull(selectedButton);
        Assert.Contains("Chiefs", selectedButton.TextContent);
    }

    [Fact]
    public async Task MakePick_SubmitsPickAndRefreshesData()
    {
        // Arrange
        var games = TestData.GetTestGames();
        var game = games.First();
        var picks = new List<VisiblePickDto>();
        var status = new PicksStatusDto
        {
            Week = 1,
            Season = 2024,
            TotalGames = 1,
            PicksMade = 0,
            IsComplete = false,
            GamesNeedingPicks = new List<int> { game.Id }
        };

        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games);

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((picks, status));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(status);

        _mockPicksService
            .Setup(x => x.SubmitPickAsync(It.IsAny<SubmitPickDto>()))
            .ReturnsAsync(new VisiblePickDto
            {
                GameId = game.Id,
                SelectedTeamId = game.HomeTeam.Id,
                HasPick = true,
                IsVisible = true
            });

        var cut = RenderComponent<Picks>();

        // Act
        var pickButton = cut.FindAll(".team-button")[0]; // Select home team button
        await cut.InvokeAsync(() => pickButton.Click());

        // Assert
        _mockPicksService.Verify(
            x => x.SubmitPickAsync(It.Is<SubmitPickDto>(p => 
                p.GameId == game.Id && 
                p.SelectedTeamId == game.HomeTeam.Id)),
            Times.Once);

        _mockPicksService.Verify(
            x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.AtLeast(2)); // Initial load + refresh after pick
    }

    [Fact]
    public void WeekNavigation_LoadsNewData_WhenChanged()
    {
        // Arrange
        var games = TestData.GetTestGames();
        var picks = new List<VisiblePickDto>();
        var status = new PicksStatusDto
        {
            Week = 1,
            Season = 2024,
            TotalGames = 1,
            PicksMade = 0,
            IsComplete = false,
            GamesNeedingPicks = new List<int> { games.First().Id }
        };

        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games);

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((picks, status));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(status);

        var cut = RenderComponent<Picks>();

        // Act
        var nextWeekButton = cut.Find("button.nav-btn:last-child");
        nextWeekButton.Click();

        // Assert
        _mockGamesService.Verify(
            x => x.GetGamesByWeekAndSeasonAsync(2, 2024),
            Times.Once);

        _mockPicksService.Verify(
            x => x.GetMyPicksForWeekAsync(2, 2024),
            Times.Once);
    }
} 