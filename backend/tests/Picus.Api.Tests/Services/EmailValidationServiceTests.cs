using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Picus.Api.Configuration;
using Picus.Api.Services;
using Xunit;

namespace Picus.Api.Tests.Services;

public class EmailValidationServiceTests
{
    private readonly Mock<ILogger<EmailValidationService>> _loggerMock;
    private const string ValidEmail = "test@example.com";
    private const string ValidEmailDifferentCase = "TEST@EXAMPLE.COM";
    private const string UnauthorizedEmail = "unauthorized@example.com";

    public EmailValidationServiceTests()
    {
        _loggerMock = new Mock<ILogger<EmailValidationService>>();
    }

    private EmailValidationService CreateService(List<string> allowedEmails)
    {
        var config = Options.Create(new AllowedEmailsConfig
        {
            Entries = allowedEmails
        });

        return new EmailValidationService(config, _loggerMock.Object);
    }

    [Fact]
    public void IsEmailAllowed_WithAllowedEmail_ReturnsTrue()
    {
        // Arrange
        var service = CreateService(new List<string> { ValidEmail });

        // Act
        var result = service.IsEmailAllowed(ValidEmail);

        // Assert
        Assert.True(result);
        VerifyNoWarningLogged();
    }

    [Fact]
    public void IsEmailAllowed_WithUnallowedEmail_ReturnsFalse()
    {
        // Arrange
        var service = CreateService(new List<string> { ValidEmail });

        // Act
        var result = service.IsEmailAllowed(UnauthorizedEmail);

        // Assert
        Assert.False(result);
        VerifyWarningLogged($"Email {UnauthorizedEmail} is not in the allowed list");
    }

    [Fact]
    public void IsEmailAllowed_WithDifferentCase_ReturnsTrue()
    {
        // Arrange
        var service = CreateService(new List<string> { ValidEmail });

        // Act
        var result = service.IsEmailAllowed(ValidEmailDifferentCase);

        // Assert
        Assert.True(result);
        VerifyNoWarningLogged();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void IsEmailAllowed_WithInvalidEmail_ReturnsFalse(string? email)
    {
        // Arrange
        var service = CreateService(new List<string> { ValidEmail });

        // Act
        var result = service.IsEmailAllowed(email!);

        // Assert
        Assert.False(result);
        VerifyWarningLogged("Attempted to validate null or empty email");
    }

    [Fact]
    public void IsEmailAllowed_WithEmptyAllowList_ReturnsFalse()
    {
        // Arrange
        var service = CreateService(new List<string>());

        // Act
        var result = service.IsEmailAllowed(ValidEmail);

        // Assert
        Assert.False(result);
        VerifyWarningLogged($"Email {ValidEmail} is not in the allowed list");
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

    private void VerifyNoWarningLogged()
    {
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}
