using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Picus.Api.Models.SportsDb;
using Picus.Api.Services;
using Xunit;

namespace Picus.Api.Tests.Services;

public class SportsDbServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly SportsDbService _sportsDbService;
    private readonly IConfiguration _configuration;

    public SportsDbServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        
        // Setup mock configuration using dictionary
        var configValues = new Dictionary<string, string>
        {
            {"SportsDbApi:Url", "http://test-api-url"},
            {"SportsDbApi:ApiKey", "test-api-key"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        _sportsDbService = new SportsDbService(_httpClient, _configuration);
    }

    [Fact]
    public async Task GetLeagueScheduleAsync_SuccessfulResponse_ReturnsSchedule()
    {
        // Arrange
        var leagueId = 4328;
        var season = 2023;
        var expectedGames = new List<Game>
        {
            new Game { Id = "1", Name = "Team A vs Team B", Date = "2024-01-01" }
        };

        var response = new GameSchedule { Schedule = expectedGames };
        var jsonResponse = JsonSerializer.Serialize(response);

        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _sportsDbService.GetLeagueScheduleAsync(leagueId, season);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedGames[0].Id, result[0].Id);
        Assert.Equal(expectedGames[0].Name, result[0].Name);
        Assert.Equal(expectedGames[0].Date, result[0].Date);

        VerifyHttpCall(leagueId, season);
    }

    [Fact]
    public async Task GetLeagueScheduleAsync_EmptyResponse_ReturnsEmptyList()
    {
        // Arrange
        var leagueId = 4328;
        var season = 2023;
        var response = new GameSchedule { Schedule = null };
        var jsonResponse = JsonSerializer.Serialize(response);

        SetupMockHttpMessageHandler(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _sportsDbService.GetLeagueScheduleAsync(leagueId, season);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        VerifyHttpCall(leagueId, season);
    }

    [Fact]
    public async Task GetLeagueScheduleAsync_HttpError_ThrowsSportsDbException()
    {
        // Arrange
        var leagueId = 4328;
        var season = 2023;

        SetupMockHttpMessageHandler(HttpStatusCode.InternalServerError, "Server Error");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SportsDbException>(
            () => _sportsDbService.GetLeagueScheduleAsync(leagueId, season));
        
        Assert.Contains("Failed to retrieve league schedule", exception.Message);
        VerifyHttpCall(leagueId, season);
    }

    [Fact]
    public async Task GetLeagueScheduleAsync_InvalidJson_ThrowsSportsDbException()
    {
        // Arrange
        var leagueId = 4328;
        var season = 2023;
        var invalidJson = "{ invalid json }";

        SetupMockHttpMessageHandler(HttpStatusCode.OK, invalidJson);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<SportsDbException>(
            () => _sportsDbService.GetLeagueScheduleAsync(leagueId, season));
        
        Assert.Contains("Failed to parse league schedule response", exception.Message);
        VerifyHttpCall(leagueId, season);
    }

    private void SetupMockHttpMessageHandler(HttpStatusCode statusCode, string content)
    {
        var baseUrl = _configuration["SportsDbApi:Url"];
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString().StartsWith(baseUrl)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }

    private void VerifyHttpCall(int leagueId, int season)
    {
        var baseUrl = _configuration["SportsDbApi:Url"];
        var expectedUrl = $"{baseUrl}/schedule/league/{leagueId}/{season}";

        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == HttpMethod.Get && 
                    req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>()
            );
    }
}
