using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Picus.Api.Middleware;
using Picus.Api.Services;
using System.Security.Claims;
using System.Text.Json;
using Xunit;

namespace Picus.Api.Tests.Middleware;

public class EmailValidationMiddlewareTests
{
    private readonly Mock<IEmailValidationService> _emailValidationServiceMock;
    private readonly Mock<ILogger<EmailValidationMiddleware>> _loggerMock;
    private readonly TestHttpContextBuilder _contextBuilder;
    private const string ValidEmail = "test@example.com";
    private const string UnauthorizedEmail = "unauthorized@example.com";

    public EmailValidationMiddlewareTests()
    {
        _emailValidationServiceMock = new Mock<IEmailValidationService>();
        _loggerMock = new Mock<ILogger<EmailValidationMiddleware>>();
        _contextBuilder = new TestHttpContextBuilder();
    }

    private EmailValidationMiddleware CreateMiddleware(RequestDelegate next)
    {
        return new EmailValidationMiddleware(next, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_UnauthenticatedRequest_PassesThrough()
    {
        // Arrange
        var context = _contextBuilder.Build();
        var nextCalled = false;
        RequestDelegate next = (HttpContext ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);

        // Act
        await middleware.InvokeAsync(context, _emailValidationServiceMock.Object);

        // Assert
        Assert.True(nextCalled, "Next delegate should be called for unauthenticated requests");
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_NoEmailClaim_Returns401()
    {
        // Arrange
        var context = _contextBuilder
            .WithAuthentication()
            .Build();

        var middleware = CreateMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context, _emailValidationServiceMock.Object);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
        await AssertResponseContainsMessage(context, "Email claim is required.");
        VerifyWarningLogged("Authenticated request without email claim");
    }

    [Fact]
    public async Task InvokeAsync_UnauthorizedEmail_Returns403()
    {
        // Arrange
        var context = _contextBuilder
            .WithAuthentication()
            .WithEmail(UnauthorizedEmail)
            .Build();

        _emailValidationServiceMock
            .Setup(x => x.IsEmailAllowed(UnauthorizedEmail))
            .Returns(false);

        var middleware = CreateMiddleware(_ => Task.CompletedTask);

        // Act
        await middleware.InvokeAsync(context, _emailValidationServiceMock.Object);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        await AssertResponseContainsMessage(context, "This site is currently invitation-only");
        VerifyWarningLogged($"Access attempt from unauthorized email: {UnauthorizedEmail}");
    }

    [Theory]
    [InlineData(ClaimTypes.Email)]
    [InlineData("email")]
    [InlineData("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")]
    public async Task InvokeAsync_AuthorizedEmail_WithClaimType_PassesThrough(string claimType)
    {
        // Arrange
        var context = _contextBuilder
            .WithAuthentication()
            .WithEmail(ValidEmail, claimType)
            .Build();

        _emailValidationServiceMock
            .Setup(x => x.IsEmailAllowed(ValidEmail))
            .Returns(true);

        var nextCalled = false;
        RequestDelegate next = (HttpContext ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = CreateMiddleware(next);

        // Act
        await middleware.InvokeAsync(context, _emailValidationServiceMock.Object);

        // Assert
        Assert.True(nextCalled, "Next delegate should be called for authorized emails");
        Assert.Equal(200, context.Response.StatusCode);
    }

    private static async Task AssertResponseContainsMessage(HttpContext context, string expectedMessage)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        var response = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);

        Assert.NotNull(response);
        Assert.Contains(expectedMessage, response["message"]);
    }

    private void VerifyWarningLogged(string message)
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
