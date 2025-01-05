using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Picus.Api.Tests.Middleware;

/// <summary>
/// Helper class for building HttpContext instances for testing
/// </summary>
public class TestHttpContextBuilder
{
    private readonly DefaultHttpContext _context;
    private readonly List<Claim> _claims;
    private bool _isAuthenticated;

    public TestHttpContextBuilder()
    {
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
        _claims = new List<Claim>();
        _isAuthenticated = false;
    }

    public TestHttpContextBuilder WithEmail(string email)
    {
        _claims.Add(new Claim(ClaimTypes.Email, email));
        return this;
    }

    public TestHttpContextBuilder WithAuthentication()
    {
        _isAuthenticated = true;
        return this;
    }

    public HttpContext Build()
    {
        if (_isAuthenticated)
        {
            var identity = new ClaimsIdentity(_claims, "TestAuth");
            _context.User = new ClaimsPrincipal(identity);
        }

        return _context;
    }
}
