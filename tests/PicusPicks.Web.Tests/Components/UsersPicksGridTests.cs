using Bunit;
using PicusPicks.Web.Components;
using PicusPicks.Web.Models.DTOs;
using PicusPicks.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PicusPicks.Web.Tests.Components
{
    public class UsersPicksGridTests : TestContext
    {
        private readonly Mock<IPicksService> _picksServiceMock;
        private readonly Mock<IGamesService> _gamesServiceMock;
        private readonly Mock<ILogger<UsersPicksGrid>> _loggerMock;
        private readonly List<GameDTO> _testGames;
        private readonly List<VisiblePickDto> _testPicks;

        public UsersPicksGridTests()
        {
            _picksServiceMock = new Mock<IPicksService>();
            _gamesServiceMock = new Mock<IGamesService>();
            _loggerMock = new Mock<ILogger<UsersPicksGrid>>();

            Services.AddScoped(_ => _picksServiceMock.Object);
            Services.AddScoped(_ => _gamesServiceMock.Object);
            Services.AddScoped(_ => _loggerMock.Object);

            // Setup test data
            _testGames = new List<GameDTO>
            {
                new GameDTO
                {
                    Id = 1,
                    GameTime = DateTime.UtcNow,
                    PickDeadline = DateTime.UtcNow.AddMinutes(-15),
                    HomeTeam = new TeamDTO { Id = 1, Name = "Home Team", IconUrl = "home.png" },
                    AwayTeam = new TeamDTO { Id = 2, Name = "Away Team", IconUrl = "away.png" },
                    IsCompleted = true,
                    WinningTeam = new TeamDTO { Id = 1, Name = "Home Team", IconUrl = "home.png" }
                }
            };

            _testPicks = new List<VisiblePickDto>
            {
                new VisiblePickDto
                {
                    UserId = 1,
                    GameId = 1,
                    SelectedTeamId = 1,
                    HasPick = true,
                    IsVisible = true
                }
            };

            _gamesServiceMock.Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_testGames);
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_testPicks);
        }

        [Fact]
        public void ShowsLoadingState_WhenDataIsLoading()
        {
            // Arrange
            var taskCompletionSource = new TaskCompletionSource<IEnumerable<GameDTO>>();
            _gamesServiceMock.Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(taskCompletionSource.Task);

            // Act
            var cut = RenderComponent<UsersPicksGrid>(parameters => parameters
                .Add(p => p.Week, 1)
                .Add(p => p.Season, 2024));

            // Assert
            cut.Find(".spinner-border").MarkupMatches("<div class=\"spinner-border\" role=\"status\"><span class=\"visually-hidden\">Loading...</span></div>");
        }

        [Fact]
        public void ShowsNoGamesMessage_WhenNoGamesExist()
        {
            // Arrange
            _gamesServiceMock.Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<GameDTO>());

            // Act
            var cut = RenderComponent<UsersPicksGrid>(parameters => parameters
                .Add(p => p.Week, 1)
                .Add(p => p.Season, 2024));

            // Assert
            cut.Find(".alert-info").MarkupMatches("<div class=\"alert alert-info\">No games found for Week 1.</div>");
        }

        [Fact]
        public void ShowsErrorMessage_WhenPicksLoadingFails()
        {
            // Arrange
            var games = new List<GameDTO>
            {
                new() { Id = 1, HomeTeam = new TeamDTO { Id = 1, Name = "Team 1" }, AwayTeam = new TeamDTO { Id = 2, Name = "Team 2" } }
            };

            _gamesServiceMock.Setup(x => x.GetGamesByWeekAndSeasonAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(games);

            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Failed to load picks"));

            // Act
            var cut = RenderComponent<UsersPicksGrid>(parameters => parameters
                .Add(p => p.Week, 1)
                .Add(p => p.Season, 2024));

            // Wait for the component to finish rendering
            cut.WaitForState(() => cut.FindAll(".alert-warning").Any());

            // Assert
            var errorMessage = cut.Find(".alert-warning");
            Assert.Contains("Error loading picks data", errorMessage.TextContent);
        }

        [Fact]
        public void Should_Render_Games_And_Picks()
        {
            // Arrange
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_testPicks);

            var cut = RenderComponent<UsersPicksGrid>(parameters => parameters
                .Add(p => p.Week, 1)
                .Add(p => p.Season, 2024));

            // Act & Assert
            var gameInfoCells = cut.FindAll(".game-info-cell");
            var pickCells = cut.FindAll(".pick-cell");

            Assert.NotEmpty(gameInfoCells);
            Assert.NotEmpty(pickCells);
        }

        [Fact]
        public void Should_Show_Winner_Indicator_For_Completed_Games()
        {
            // Arrange
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_testPicks);

            var cut = RenderComponent<UsersPicksGrid>(parameters => parameters
                .Add(p => p.Week, 1)
                .Add(p => p.Season, 2024));

            // Act & Assert
            var winnerIndicator = cut.Find(".winner-indicator");
            Assert.Contains("✓", winnerIndicator.TextContent);
        }

        [Fact]
        public void Should_Show_Correct_Pick_Styling()
        {
            // Arrange
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_testPicks);

            var cut = RenderComponent<UsersPicksGrid>(parameters => parameters
                .Add(p => p.Week, 1)
                .Add(p => p.Season, 2024));

            // Act & Assert
            var pickCell = cut.Find(".pick-cell");
            Assert.Contains("correct", pickCell.ClassList);
        }

        [Fact]
        public void Should_Show_Team_Logos()
        {
            // Arrange
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_testPicks);

            var cut = RenderComponent<UsersPicksGrid>(parameters => parameters
                .Add(p => p.Week, 1)
                .Add(p => p.Season, 2024));

            // Act & Assert
            var logos = cut.FindAll(".team-logo");
            Assert.Equal(2, logos.Count); // One for away team, one for home team

            var awayLogo = logos[0];
            var homeLogo = logos[1];

            Assert.Equal("away.png", awayLogo.GetAttribute("src"));
            Assert.Equal("home.png", homeLogo.GetAttribute("src"));
        }
    }
}
