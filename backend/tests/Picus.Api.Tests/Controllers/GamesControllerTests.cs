using Microsoft.AspNetCore.Mvc;
using Moq;
using Picus.Api.Controllers;
using Picus.Api.Models;
using Picus.Api.Models.DTOs;
using Picus.Api.Models.Enums;
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
        var gameDto = new GameDTO
        {
            Id = 1,
            ExternalGameId = "game1",
            Location = "Test Stadium",
            GameTime = DateTime.UtcNow,
            PickDeadline = DateTime.UtcNow,
            HomeTeamScore = 21,
            AwayTeamScore = 14,
            IsCompleted = true,
            Week = 17,
            Season = 2023,
            IsPlayoffs = false,
            HomeTeam = new TeamDTO 
            { 
                Id = 1, 
                Name = "Home Team",
                Abbreviation = "HT",
                City = "Home City",
                IconUrl = "home-icon.png",
                BannerUrl = "home-banner.png",
                PrimaryColor = "#000000",
                SecondaryColor = "#FFFFFF",
                TertiaryColor = "#FF0000",
                Conference = ConferenceType.AFC,
                Division = DivisionType.North
            },
            AwayTeam = new TeamDTO 
            { 
                Id = 2, 
                Name = "Away Team",
                Abbreviation = "AT",
                City = "Away City",
                IconUrl = "away-icon.png",
                BannerUrl = "away-banner.png",
                PrimaryColor = "#000000",
                SecondaryColor = "#FFFFFF",
                TertiaryColor = "#0000FF",
                Conference = ConferenceType.NFC,
                Division = DivisionType.South
            }
        };

        _mockGameService.Setup(s => s.GetGameByIdAsync(1))
            .ReturnsAsync(gameDto);

        // Act
        var result = await _controller.GetGameById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGame = Assert.IsType<GameDTO>(okResult.Value);
        Assert.Equal(1, returnedGame.Id);
        Assert.Equal("Test Stadium", returnedGame.Location);
        Assert.Equal(21, returnedGame.HomeTeamScore);
        Assert.Equal(14, returnedGame.AwayTeamScore);
        Assert.Equal("Home Team", returnedGame.HomeTeam.Name);
        Assert.Equal("Away Team", returnedGame.AwayTeam.Name);
        Assert.Equal("home-icon.png", returnedGame.HomeTeam.IconUrl);
        Assert.Equal("away-icon.png", returnedGame.AwayTeam.IconUrl);
    }

    [Fact]
    public async Task GetGameById_WithNonExistingGame_ReturnsNotFound()
    {
        // Arrange
        _mockGameService.Setup(s => s.GetGameByIdAsync(999))
            .ReturnsAsync((GameDTO?)null);

        // Act
        var result = await _controller.GetGameById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetGamesByWeekAndSeason_WithExistingGames_ReturnsOkResult()
    {
        // Arrange
        var games = new List<GameDTO>
        {
            new()
            {
                Id = 1,
                ExternalGameId = "game1",
                Location = "Stadium 1",
                GameTime = DateTime.UtcNow,
                PickDeadline = DateTime.UtcNow,
                HomeTeamScore = 21,
                AwayTeamScore = 14,
                IsCompleted = true,
                Week = 17,
                Season = 2023,
                IsPlayoffs = false,
                HomeTeam = new TeamDTO 
                { 
                    Id = 1, 
                    Name = "Home Team",
                    Abbreviation = "HT",
                    City = "Home City",
                    IconUrl = "home-icon.png",
                    BannerUrl = "home-banner.png",
                    PrimaryColor = "#000000",
                    SecondaryColor = "#FFFFFF",
                    TertiaryColor = "#FF0000",
                    Conference = ConferenceType.AFC,
                    Division = DivisionType.North
                },
                AwayTeam = new TeamDTO 
                { 
                    Id = 2, 
                    Name = "Away Team",
                    Abbreviation = "AT",
                    City = "Away City",
                    IconUrl = "away-icon.png",
                    BannerUrl = "away-banner.png",
                    PrimaryColor = "#000000",
                    SecondaryColor = "#FFFFFF",
                    TertiaryColor = "#0000FF",
                    Conference = ConferenceType.NFC,
                    Division = DivisionType.South
                }
            },
            new()
            {
                Id = 2,
                ExternalGameId = "game2",
                Location = "Stadium 2",
                GameTime = DateTime.UtcNow.AddHours(1),
                PickDeadline = DateTime.UtcNow.AddHours(1),
                HomeTeamScore = null,
                AwayTeamScore = null,
                IsCompleted = false,
                Week = 17,
                Season = 2023,
                IsPlayoffs = false,
                HomeTeam = new TeamDTO 
                { 
                    Id = 2, 
                    Name = "Away Team",
                    Abbreviation = "AT",
                    City = "Away City",
                    IconUrl = "away-icon.png",
                    BannerUrl = "away-banner.png",
                    PrimaryColor = "#000000",
                    SecondaryColor = "#FFFFFF",
                    TertiaryColor = "#0000FF",
                    Conference = ConferenceType.NFC,
                    Division = DivisionType.South
                },
                AwayTeam = new TeamDTO 
                { 
                    Id = 1, 
                    Name = "Home Team",
                    Abbreviation = "HT",
                    City = "Home City",
                    IconUrl = "home-icon.png",
                    BannerUrl = "home-banner.png",
                    PrimaryColor = "#000000",
                    SecondaryColor = "#FFFFFF",
                    TertiaryColor = "#FF0000",
                    Conference = ConferenceType.AFC,
                    Division = DivisionType.North
                }
            }
        };

        _mockGameService.Setup(s => s.GetGamesByWeekAndSeasonAsync(17, 2023))
            .ReturnsAsync(games);

        // Act
        var result = await _controller.GetGamesByWeekAndSeason(17, 2023);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGames = Assert.IsType<List<GameDTO>>(okResult.Value);
        Assert.Equal(2, returnedGames.Count);
        Assert.All(returnedGames, g => Assert.Equal(17, g.Week));
        Assert.All(returnedGames, g => Assert.Equal(2023, g.Season));
        Assert.Equal("Stadium 1", returnedGames[0].Location);
        Assert.Equal("Stadium 2", returnedGames[1].Location);
    }

    [Fact]
    public async Task GetGamesByWeekAndSeason_WithNoGames_ReturnsEmptyList()
    {
        // Arrange
        _mockGameService.Setup(s => s.GetGamesByWeekAndSeasonAsync(1, 2023))
            .ReturnsAsync(new List<GameDTO>());

        // Act
        var result = await _controller.GetGamesByWeekAndSeason(1, 2023);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedGames = Assert.IsType<List<GameDTO>>(okResult.Value);
        Assert.Empty(returnedGames);
    }
}
