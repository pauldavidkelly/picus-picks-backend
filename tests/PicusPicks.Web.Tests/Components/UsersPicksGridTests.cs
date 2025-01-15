using Bunit;
using PicusPicks.Web.Components;
using Picus.Api.Models;
using Picus.Api.Models.DTOs;
using PicusPicks.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PicusPicks.Web.Tests.Components
{
    public class UsersPicksGridTests : TestContext
    {
        private readonly Mock<IPicksService> _picksServiceMock;
        private readonly Mock<IGamesService> _gamesServiceMock;
        private readonly List<GameDTO> _testGames;
        private readonly List<VisiblePickDto> _testPicks;

        public UsersPicksGridTests()
        {
            _picksServiceMock = new Mock<IPicksService>();
            _gamesServiceMock = new Mock<IGamesService>();
            Services.AddScoped(_ => _picksServiceMock.Object);
            Services.AddScoped(_ => _gamesServiceMock.Object);

            // Setup test data
            _testGames = new List<GameDTO>
            {
                new GameDTO
                {
                    Id = 1,
                    GameTime = DateTime.UtcNow,
                    AwayTeam = new TeamDTO { Id = 1, Name = "Away Team", IconUrl = "away.png" },
                    HomeTeam = new TeamDTO { Id = 2, Name = "Home Team", IconUrl = "home.png" },
                    IsCompleted = true,
                    WinningTeam = new TeamDTO { Id = 1, Name = "Away Team", IconUrl = "away.png" }
                }
            };

            _testPicks = new List<VisiblePickDto>
            {
                new VisiblePickDto
                {
                    UserId = 1,
                    GameId = 1,
                    SelectedTeamId = 1
                }
            };

            _gamesServiceMock.Setup(x => x.GetGamesByWeekAndSeasonAsync(1, 2024))
                .ReturnsAsync(_testGames);
        }

        [Fact]
        public void Should_Render_Games_And_Picks()
        {
            // Arrange
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(1, 2024))
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
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(1, 2024))
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
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(1, 2024))
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
            _picksServiceMock.Setup(x => x.GetAllPicksForWeekAsync(1, 2024))
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
