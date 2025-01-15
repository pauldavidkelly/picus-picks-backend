using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PicusPicks.Web.Models;
using PicusPicks.Web.Services;
using Xunit;

namespace PicusPicks.Web.Tests.Services;

public class LeagueTableServiceTests
{
    private readonly Mock<ILogger<LeagueTableService>> _loggerMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<HttpContext> _httpContextMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;

    public LeagueTableServiceTests()
    {
        _loggerMock = new Mock<ILogger<LeagueTableService>>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://test.com/")
        };
        _httpContextMock = new Mock<HttpContext>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _serviceProviderMock = new Mock<IServiceProvider>();

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);
        _httpContextMock.Setup(x => x.RequestServices).Returns(_serviceProviderMock.Object);
        
        // Set up the service provider to return our auth service
        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(_authServiceMock.Object);
    }

    [Fact]
    public async Task GetLeagueTableStats_ReturnsCorrectStats()
    {
        // Arrange
        var testData = new List<LeagueTableStats>
        {
            new() { DisplayName = "User1", CorrectPicks = 7, TotalPicks = 10 },
            new() { DisplayName = "User2", CorrectPicks = 3, TotalPicks = 10 },
            new() { DisplayName = "User3", CorrectPicks = 5, TotalPicks = 5 }
        };

        SetupMockAuth();
        SetupMockHttpResponse(testData);

        var service = CreateService();

        // Act
        var result = (await service.GetLeagueTableStatsAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        // Should be ordered by success rate: User3 (100%), User1 (70%), User2 (30%)
        Assert.Equal("User3", result[0].DisplayName);
        Assert.Equal("User1", result[1].DisplayName);
        Assert.Equal("User2", result[2].DisplayName);
        Assert.Equal(100m, result[0].SuccessRate);
        Assert.Equal(70m, result[1].SuccessRate);
        Assert.Equal(30m, result[2].SuccessRate);
    }

    [Fact]
    public async Task GetLeagueTableStats_HandlesNoUsers()
    {
        // Arrange
        SetupMockAuth();
        SetupMockHttpResponse(new List<LeagueTableStats>());

        var service = CreateService();

        // Act
        var result = await service.GetLeagueTableStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLeagueTableStats_HandlesUsersWithNoPicks()
    {
        // Arrange
        var testData = new List<LeagueTableStats>
        {
            new() { DisplayName = "User1", CorrectPicks = 0, TotalPicks = 0 },
            new() { DisplayName = "User2", CorrectPicks = 5, TotalPicks = 10 }
        };

        SetupMockAuth();
        SetupMockHttpResponse(testData);

        var service = CreateService();

        // Act
        var result = (await service.GetLeagueTableStatsAsync()).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        // User with picks should be first, user with no picks (0%) should be last
        Assert.Equal("User2", result[0].DisplayName);
        Assert.Equal("User1", result[1].DisplayName);
        Assert.Equal(50m, result[0].SuccessRate);
        Assert.Equal(0m, result[1].SuccessRate);
    }

    [Fact]
    public async Task GetLeagueTableStats_HandlesNoAuthToken()
    {
        // Arrange
        _authServiceMock
            .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(AuthenticateResult.Fail("No token"));

        var service = CreateService();

        // Act
        var result = await service.GetLeagueTableStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        VerifyLogWarning("No access token available for league table stats request");
    }

    private LeagueTableService CreateService()
    {
        return new LeagueTableService(
            _httpClient,
            _loggerMock.Object,
            _httpContextAccessorMock.Object);
    }

    private void SetupMockAuth()
    {
        var authProps = new AuthenticationProperties();
        authProps.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "access_token", Value = "test-token" }
        });

        _authServiceMock
            .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(AuthenticateResult.Success(new AuthenticationTicket(
                new System.Security.Claims.ClaimsPrincipal(), 
                authProps,
                "test")));
    }

    private void SetupMockHttpResponse(IEnumerable<LeagueTableStats> content)
    {
        var responseContent = JsonContent.Create(content);
        
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>(async (request, token) =>
            {
                // Log the request details
                var stream = new MemoryStream();
                if (content != null)
                {
                    await JsonSerializer.SerializeAsync(stream, content);
                    stream.Position = 0;
                    var contentString = await new StreamReader(stream).ReadToEndAsync();
                    _loggerMock.Object.LogInformation(
                        "Request made to {Url} with auth: {HasAuth}\nContent: {Content}",
                        request.RequestUri,
                        request.Headers.Authorization != null,
                        contentString);
                }
            })
            .ReturnsAsync(() => new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = responseContent
            });
    }

    private void VerifyLogWarning(string message)
    {
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
