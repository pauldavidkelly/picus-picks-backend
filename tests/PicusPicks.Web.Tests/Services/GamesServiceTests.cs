using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Picus.Api.Models.DTOs;
using PicusPicks.Web.Services;
using PicusPicks.Web.Tests.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace PicusPicks.Web.Tests.Services;

public class GamesServiceTests
{
    private readonly Mock<ILogger<GamesService>> _mockLogger;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;

    public GamesServiceTests()
    {
        _mockLogger = new Mock<ILogger<GamesService>>();
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockAuthService = new Mock<IAuthenticationService>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        // Setup HttpClient with mock handler
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:5172/")
        };

        // Setup authentication service
        var authResult = AuthenticateResult.Success(
            new AuthenticationTicket(
                new System.Security.Claims.ClaimsPrincipal(),
                new AuthenticationProperties(),
                "Bearer"
            )
        );
        authResult.Properties.SetString(".Token.access_token", "test_token");

        _mockAuthService
            .Setup(x => x.AuthenticateAsync(It.IsAny<HttpContext>(), It.IsAny<string>()))
            .ReturnsAsync(authResult);

        // Setup HttpContext
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(_mockAuthService.Object);

        _mockHttpContext
            .Setup(x => x.RequestServices)
            .Returns(serviceProvider.Object);

        _mockHttpContextAccessor
            .Setup(x => x.HttpContext)
            .Returns(_mockHttpContext.Object);
    }

    [Fact]
    public async Task GetGamesByWeekAndSeasonAsync_ReturnsGames_WhenApiCallSucceeds()
    {
        // Arrange
        var expectedGames = TestData.GetTestGames();
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    JsonSerializer.Serialize(expectedGames, jsonOptions),
                    System.Text.Encoding.UTF8,
                    "application/json"
                )
            });

        var gamesService = new GamesService(_httpClient, _mockLogger.Object, _mockHttpContextAccessor.Object);

        // Act
        var result = await gamesService.GetGamesByWeekAndSeasonAsync(1, 2024);

        // Assert
        Assert.NotNull(result);
        Assert.Collection(result, 
            item => Assert.Equal(
                JsonSerializer.Serialize(expectedGames.First(), jsonOptions),
                JsonSerializer.Serialize(item, jsonOptions)
            )
        );
    }
} 