using Microsoft.Extensions.Configuration;
using PicusPicks.Web.Configuration;
using Xunit;

namespace PicusPicks.Web.Tests.Configuration;

public class Auth0SettingsTests
{
    [Fact]
    public void Auth0Settings_CanBindFromConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Auth0:Domain", "test-domain"},
                {"Auth0:ClientId", "test-client-id"},
                {"Auth0:ClientSecret", "test-client-secret"},
                {"Auth0:Audience", "test-audience"},
                {"Auth0:CallbackPath", "/test-callback"}
            })
            .Build();

        // Act
        var settings = configuration.GetSection(Auth0Settings.SectionName).Get<Auth0Settings>();

        // Assert
        Assert.NotNull(settings);
        Assert.Equal("test-domain", settings.Domain);
        Assert.Equal("test-client-id", settings.ClientId);
        Assert.Equal("test-client-secret", settings.ClientSecret);
        Assert.Equal("test-audience", settings.Audience);
        Assert.Equal("/test-callback", settings.CallbackPath);
    }

    [Fact]
    public void Auth0Settings_DefaultValues_AreEmpty()
    {
        // Arrange & Act
        var settings = new Auth0Settings();

        // Assert
        Assert.Equal(string.Empty, settings.Domain);
        Assert.Equal(string.Empty, settings.ClientId);
        Assert.Equal(string.Empty, settings.ClientSecret);
        Assert.Equal(string.Empty, settings.Audience);
        Assert.Equal(string.Empty, settings.CallbackPath);
    }
}
