using Xunit;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using PicusPicks.Web.Components.Pages;
using PicusPicks.Web.Models.DTOs;
using PicusPicks.Web.Services;
using PicusPicks.Web.Tests.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PicusPicks.Web.Tests.Components.Pages;

public class PicksTests : TestContext
{
    private readonly Mock<IPicksService> _mockPicksService;
    private readonly Mock<IGamesService> _mockGamesService;
    private readonly Mock<ILogger<Picks>> _mockLogger;
    private readonly Mock<IJSRuntime> _mockJsRuntime;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IConfigurationSection> _mockFeatureFlagsSection;

    public PicksTests()
    {
        _mockPicksService = new Mock<IPicksService>();
        _mockGamesService = new Mock<IGamesService>();
        _mockLogger = new Mock<ILogger<Picks>>();
        _mockJsRuntime = new Mock<IJSRuntime>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockFeatureFlagsSection = new Mock<IConfigurationSection>();

        // Setup configuration sections
        _mockFeatureFlagsSection.Setup(s => s.Value).Returns("false");
        _mockConfiguration
            .Setup(c => c.GetSection("FeatureFlags:BypassPickDeadlines"))
            .Returns(_mockFeatureFlagsSection.Object);

        Services.AddScoped<IPicksService>(_ => _mockPicksService.Object);
        Services.AddScoped<IGamesService>(_ => _mockGamesService.Object);
        Services.AddScoped<ILogger<Picks>>(_ => _mockLogger.Object);
        Services.AddScoped<IJSRuntime>(_ => _mockJsRuntime.Object);
        Services.AddScoped<IConfiguration>(_ => _mockConfiguration.Object);
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
                SelectedTeamId = game.AwayTeam.Id,
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
                p.SelectedTeamId == game.AwayTeam.Id)),
            Times.Once);

        _mockPicksService.Verify(
            x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()),
            Times.AtLeast(2)); // Initial load + refresh after pick
    }

    [Theory]
    [InlineData("2025-01-15T08:27:35Z", 19)]  // Wild Card
    [InlineData("2025-01-22T08:27:35Z", 20)]  // Divisional
    [InlineData("2025-01-29T08:27:35Z", 21)]  // Conference
    [InlineData("2025-02-12T08:27:35Z", 22)]  // Super Bowl
    [InlineData("2024-09-05T08:27:35Z", 1)]   // Regular Season Week 1
    [InlineData("2024-09-12T08:27:35Z", 2)]   // Regular Season Week 2
    [InlineData("2024-09-19T08:27:35Z", 3)]   // Regular Season Week 3
    [InlineData("2024-09-26T08:27:35Z", 4)]   // Regular Season Week 4
    [InlineData("2024-07-01T08:27:35Z", 1)]   // Off-season
    public void InitialWeek_SetsCorrectWeekBasedOnDate(string currentDate, int expectedWeek)
    {
        // Arrange
        var games = TestData.GetTestGames();
        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(expectedWeek, 2024))
            .ReturnsAsync(games);

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(expectedWeek, 2024))
            .ReturnsAsync((new List<VisiblePickDto>(), new PicksStatusDto()));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(expectedWeek, 2024))
            .ReturnsAsync(new PicksStatusDto());

        // Act
        var cut = RenderComponent<Picks>(parameters => parameters
            .Add(p => p.CurrentDateOverride, DateTimeOffset.Parse(currentDate)));

        // Assert
        var weekDisplay = cut.Find(".week-display");
        Assert.Contains($"WEEK {expectedWeek}", weekDisplay.TextContent);
    }

    [Fact]
    public void WeekNavigation_LoadsNewData_WhenChanged()
    {
        // Arrange
        var games = TestData.GetTestGames();
        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games);

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<VisiblePickDto>(), new PicksStatusDto()));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PicksStatusDto());

        // Set initial date to Wild Card week
        var cut = RenderComponent<Picks>(parameters => parameters
            .Add(p => p.CurrentDateOverride, DateTimeOffset.Parse("2025-01-15T08:26:33Z")));

        // Navigate to next week
        var nextButton = cut.Find("button.nav-btn:last-child");
        nextButton.Click();

        // Assert
        _mockGamesService.Verify(
            x => x.GetGamesByWeekAndSeasonAsync(20, 2024),
            Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsGameLocked_RespectsDeadlineBypass(bool bypassEnabled)
    {
        // Arrange
        var pastDeadlineGame = TestData.GetTestGames().First();
        pastDeadlineGame.PickDeadline = DateTime.UtcNow.AddHours(-1); // Game deadline was 1 hour ago

        _mockFeatureFlagsSection.Setup(s => s.Value).Returns(bypassEnabled.ToString().ToLower());

        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new[] { pastDeadlineGame });

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((new List<VisiblePickDto>(), new PicksStatusDto()));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PicksStatusDto());

        // Act
        var cut = RenderComponent<Picks>();

        // Assert
        var gameCard = cut.Find(".game-card");
        var isLocked = gameCard.ClassList.Contains("locked");
        Assert.Equal(!bypassEnabled, isLocked); // Should be locked only when bypass is disabled
    }

    [Fact]
    public async Task CorrectPicksCount_WithNoCompletedGames_ReturnsZero()
    {
        // Arrange
        var homeTeam1 = new TeamDTO { Id = 1, Name = "Home Team 1" };
        var awayTeam1 = new TeamDTO { Id = 2, Name = "Away Team 1" };
        var homeTeam2 = new TeamDTO { Id = 3, Name = "Home Team 2" };
        var awayTeam2 = new TeamDTO { Id = 4, Name = "Away Team 2" };

        var games = new List<GameDTO>
        {
            new() 
            { 
                Id = 1, 
                IsCompleted = false,
                HomeTeam = homeTeam1,
                AwayTeam = awayTeam1,
                WinningTeam = null
            },
            new() 
            { 
                Id = 2, 
                IsCompleted = false,
                HomeTeam = homeTeam2,
                AwayTeam = awayTeam2,
                WinningTeam = null
            }
        };

        var picks = new List<VisiblePickDto>
        {
            new() { GameId = 1, SelectedTeamId = 1, HasPick = true, IsVisible = true },
            new() { GameId = 2, SelectedTeamId = 2, HasPick = true, IsVisible = true }
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

        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games);

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((picks, status));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(status);

        // Mock the feature flag to be disabled
        _mockFeatureFlagsSection.Setup(s => s.Value).Returns("false");
        _mockConfiguration
            .Setup(c => c.GetSection("FeatureFlags:BypassPickDeadlines"))
            .Returns(_mockFeatureFlagsSection.Object);

        // Act
        var cut = RenderComponent<Picks>();

        // Wait for async operations to complete
        await cut.InvokeAsync(() => Task.Delay(100));

        // Assert
        var statusText = cut.Find(".status-text").TextContent;
        Assert.Contains("(0 CORRECT)", statusText);
    }

    [Fact]
    public async Task CorrectPicksCount_WithCompletedGames_ReturnsCorrectCount()
    {
        // Arrange
        var homeTeam1 = new TeamDTO { Id = 1, Name = "Home Team 1" };
        var awayTeam1 = new TeamDTO { Id = 2, Name = "Away Team 1" };
        var homeTeam2 = new TeamDTO { Id = 3, Name = "Home Team 2" };
        var awayTeam2 = new TeamDTO { Id = 4, Name = "Away Team 2" };

        var games = new List<GameDTO>
        {
            new() 
            { 
                Id = 1, 
                IsCompleted = true,
                HomeTeamScore = 24,
                AwayTeamScore = 17,
                HomeTeam = homeTeam1,
                AwayTeam = awayTeam1,
                WinningTeam = homeTeam1  // Home team won (24-17)
            },
            new() 
            { 
                Id = 2, 
                IsCompleted = true,
                HomeTeamScore = 14,
                AwayTeamScore = 28,
                HomeTeam = homeTeam2,
                AwayTeam = awayTeam2,
                WinningTeam = awayTeam2  // Away team won (28-14)
            }
        };

        var picks = new List<VisiblePickDto>
        {
            new() { GameId = 1, SelectedTeamId = 1, HasPick = true, IsVisible = true }, // Correct pick (home team won)
            new() { GameId = 2, SelectedTeamId = 3, HasPick = true, IsVisible = true }  // Incorrect pick (away team won)
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

        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games);

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((picks, status));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(status);

        // Mock the feature flag to be disabled
        _mockFeatureFlagsSection.Setup(s => s.Value).Returns("false");
        _mockConfiguration
            .Setup(c => c.GetSection("FeatureFlags:BypassPickDeadlines"))
            .Returns(_mockFeatureFlagsSection.Object);

        // Act
        var cut = RenderComponent<Picks>();

        // Wait for async operations to complete
        await cut.InvokeAsync(() => Task.Delay(100));

        // Assert
        var statusText = cut.Find(".status-text").TextContent;
        Assert.Contains("(1 CORRECT)", statusText);
    }

    [Fact]
    public async Task CorrectPicksCount_WithMixedGameStates_OnlyCountsCompletedGames()
    {
        // Arrange
        var homeTeam1 = new TeamDTO { Id = 1, Name = "Home Team 1" };
        var awayTeam1 = new TeamDTO { Id = 2, Name = "Away Team 1" };
        var homeTeam2 = new TeamDTO { Id = 3, Name = "Home Team 2" };
        var awayTeam2 = new TeamDTO { Id = 4, Name = "Away Team 2" };

        var games = new List<GameDTO>
        {
            new() 
            { 
                Id = 1, 
                IsCompleted = true,
                HomeTeamScore = 24,
                AwayTeamScore = 17,
                HomeTeam = homeTeam1,
                AwayTeam = awayTeam1,
                WinningTeam = homeTeam1  // Home team won (24-17)
            },
            new() 
            { 
                Id = 2, 
                IsCompleted = false,
                HomeTeam = homeTeam2,
                AwayTeam = awayTeam2,
                WinningTeam = null  // Game not completed
            }
        };

        var picks = new List<VisiblePickDto>
        {
            new() { GameId = 1, SelectedTeamId = 1, HasPick = true, IsVisible = true }, // Correct pick for completed game
            new() { GameId = 2, SelectedTeamId = 3, HasPick = true, IsVisible = true }  // Pick for incomplete game (shouldn't count)
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

        _mockGamesService
            .Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(games);

        _mockPicksService
            .Setup(x => x.GetMyPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((picks, status));

        _mockPicksService
            .Setup(x => x.GetPickStatusAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(status);

        // Mock the feature flag to be disabled
        _mockFeatureFlagsSection.Setup(s => s.Value).Returns("false");
        _mockConfiguration
            .Setup(c => c.GetSection("FeatureFlags:BypassPickDeadlines"))
            .Returns(_mockFeatureFlagsSection.Object);

        // Act
        var cut = RenderComponent<Picks>();

        // Wait for async operations to complete
        await cut.InvokeAsync(() => Task.Delay(100));

        // Assert
        var statusText = cut.Find(".status-text").TextContent;
        Assert.Contains("(1 CORRECT)", statusText);
    }
} 