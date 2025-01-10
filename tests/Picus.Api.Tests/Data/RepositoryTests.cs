using Picus.Api.Data;
using Picus.Api.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Picus.Api.Tests.Data;

public class RepositoryTests : TestBase
{
    private readonly Mock<ILogger<Repository<Team>>> _mockLogger;
    private readonly IRepository<Team> _repository;

    public RepositoryTests()
    {
        _mockLogger = new Mock<ILogger<Repository<Team>>>();
        _repository = new Repository<Team>(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var team = new Team
        {
            ExternalTeamId = "1",
            Name = "Test Team",
            Abbreviation = "TST",
            IconUrl = "http://test.com/icon.png",
            BannerUrl = "http://test.com/banner.png",
            PrimaryColor = "#000000",
            SecondaryColor = "#FFFFFF",
            TertiaryColor = "#888888"
        };

        // Act
        var result = await _repository.AddAsync(team);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal(team.Name, result.Name);
        var dbTeam = await _context.Teams.FindAsync(result.Id);
        Assert.NotNull(dbTeam);
        Assert.Equal(team.Name, dbTeam.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        // Arrange
        var team = new Team
        {
            ExternalTeamId = "1",
            Name = "Test Team",
            Abbreviation = "TST"
        };
        await _repository.AddAsync(team);

        // Act
        var result = await _repository.GetByIdAsync(team.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(team.Name, result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var teams = new[]
        {
            new Team { ExternalTeamId = "1", Name = "Team 1", Abbreviation = "T1" },
            new Team { ExternalTeamId = "2", Name = "Team 2", Abbreviation = "T2" }
        };
        
        foreach (var team in teams)
        {
            await _repository.AddAsync(team);
        }

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(teams.Length, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var team = new Team
        {
            ExternalTeamId = "1",
            Name = "Test Team",
            Abbreviation = "TST"
        };
        await _repository.AddAsync(team);

        // Act
        team.Name = "Updated Team";
        await _repository.UpdateAsync(team);

        // Assert
        var updatedTeam = await _repository.GetByIdAsync(team.Id);
        Assert.NotNull(updatedTeam);
        Assert.Equal("Updated Team", updatedTeam.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var team = new Team
        {
            ExternalTeamId = "1",
            Name = "Test Team",
            Abbreviation = "TST"
        };
        await _repository.AddAsync(team);

        // Act
        await _repository.DeleteAsync(team.Id);

        // Assert
        var deletedTeam = await _repository.GetByIdAsync(team.Id);
        Assert.Null(deletedTeam);
    }
}
