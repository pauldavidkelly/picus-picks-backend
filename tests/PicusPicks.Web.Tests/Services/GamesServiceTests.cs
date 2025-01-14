using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using Moq.Protected;
using PicusPicks.Web.Models;
using PicusPicks.Web.Services;
using PicusPicks.Web.Tests.Helpers;

namespace PicusPicks.Web.Tests.Services;

public class GamesServiceTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ILogger<GamesService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;

    public GamesServiceTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockLogger = new Mock<ILogger<GamesService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://test.com/")
        };

        // Setup default authentication token
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.GetTokenAsync("access_token"))
            .ReturnsAsync("test_token");
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(mockHttpContext.Object);
    }

    [Fact]
    public async Task GetGamesByWeekAndSeasonAsync_ReturnsGames_WhenApiCallSucceeds()
    {
        // Arrange
        var sampleGames = TestData.GetSampleGames();
        SetupMockHttpResponse<IEnumerable<GameDTO>>(HttpStatusCode.OK, sampleGames);
        var service = CreateService();

        // Act
        var result = await service.GetGamesByWeekAndSeasonAsync(20, 2024);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        VerifyHttpCall("api/Games/week/20/season/2024", HttpMethod.Get);
    }

    [Fact]
    public async Task GetGamesByWeekAndSeasonAsync_ThrowsException_WhenNoAuthToken()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.GetTokenAsync("access_token"))
            .ReturnsAsync((string?)null);
        _mockHttpContextAccessor.Setup(x => x.HttpContext)
            .Returns(mockHttpContext.Object);
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetGamesByWeekAndSeasonAsync(20, 2024));
    }

    [Fact]
    public async Task GetGamesByWeekAndSeasonAsync_ThrowsException_WhenApiReturnsError()
    {
        // Arrange
        SetupMockHttpResponse<object>(HttpStatusCode.InternalServerError, null);
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetGamesByWeekAndSeasonAsync(20, 2024));
    }

    [Fact]
    public async Task SyncGamesAsync_ReturnsGames_WhenApiCallSucceeds()
    {
        // Arrange
        var sampleGames = TestData.GetSampleGames();
        SetupMockHttpResponse<IEnumerable<GameDTO>>(HttpStatusCode.OK, sampleGames);
        var service = CreateService();

        // Act
        var result = await service.SyncGamesAsync(4391, 2024);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        VerifyHttpCall("api/Games/upsert/4391/2024", HttpMethod.Post);
    }

    [Fact]
    public async Task SyncGamesAsync_ThrowsException_WhenUnauthorized()
    {
        // Arrange
        SetupMockHttpResponse<object>(HttpStatusCode.Unauthorized, null);
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.SyncGamesAsync(4391, 2024));
    }

    private GamesService CreateService()
    {
        return new GamesService(_httpClient, _mockLogger.Object, _mockHttpContextAccessor.Object);
    }

    private void SetupMockHttpResponse<T>(HttpStatusCode statusCode, T? content)
    {
        var response = new HttpResponseMessage(statusCode);
        if (content != null)
        {
            response.Content = JsonContent.Create(content);
        }

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private void VerifyHttpCall(string expectedPath, HttpMethod expectedMethod)
    {
        _mockHttpMessageHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == expectedMethod && 
                    req.RequestUri!.PathAndQuery == expectedPath),
                ItExpr.IsAny<CancellationToken>());
    }
} 