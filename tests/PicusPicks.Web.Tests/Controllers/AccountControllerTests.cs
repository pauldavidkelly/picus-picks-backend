using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PicusPicks.Web.Controllers;
using Xunit;

namespace PicusPicks.Web.Tests.Controllers;

public class AccountControllerTests
{
    [Fact]
    public async Task Login_ShouldChallenge_WithAuth0AndRedirectUri()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        var controller = new AccountController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProviderMock.Object
                }
            }
        };

        var returnUrl = "/test-return";

        // Act
        await controller.Login(returnUrl);

        // Assert
        authServiceMock.Verify(
            auth => auth.ChallengeAsync(
                It.IsAny<HttpContext>(),
                "OpenIdConnect",
                It.Is<AuthenticationProperties>(p => 
                    p.RedirectUri == returnUrl)),
            Times.Once);
    }

    [Fact]
    public async Task LogOut_ShouldSignOut_FromBothAuth0AndCookies()
    {
        // Arrange
        var authServiceMock = new Mock<IAuthenticationService>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);

        var controller = new AccountController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProviderMock.Object
                }
            }
        };

        // Act
        await controller.LogOut();

        // Assert
        authServiceMock.Verify(
            auth => auth.SignOutAsync(
                It.IsAny<HttpContext>(),
                "OpenIdConnect",
                It.Is<AuthenticationProperties>(p => 
                    p.RedirectUri == "/")),
            Times.Once);

        authServiceMock.Verify(
            auth => auth.SignOutAsync(
                It.IsAny<HttpContext>(),
                "Cookies",
                null),
            Times.Once);
    }
}
