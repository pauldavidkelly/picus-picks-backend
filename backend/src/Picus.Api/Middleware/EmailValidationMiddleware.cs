using System.Security.Claims;
using Picus.Api.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Picus.Api.Middleware;

/// <summary>
/// Middleware that validates user's email against the allowed list
/// </summary>
public class EmailValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EmailValidationMiddleware> _logger;
    private static readonly string[] EmailClaimTypes = new[]
    {
        ClaimTypes.Email,
        "email",  // Auth0's email claim type
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"  // Another common email claim type
    };

    public EmailValidationMiddleware(
        RequestDelegate next,
        ILogger<EmailValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IEmailValidationService emailValidationService)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var emailClaim = context.User.Claims
            .FirstOrDefault(c => EmailClaimTypes.Contains(c.Type));

        if (emailClaim == null)
        {
            _logger.LogWarning("Authenticated request without email claim. Available claims: {Claims}", 
                string.Join(", ", context.User.Claims.Select(c => $"{c.Type}: {c.Value}")));
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await WriteJsonResponse(context, "Email claim is required.");
            return;
        }

        if (!emailValidationService.IsEmailAllowed(emailClaim.Value))
        {
            _logger.LogWarning("Access attempt from unauthorized email: {Email}", emailClaim.Value);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await WriteJsonResponse(context, "This site is currently invitation-only");
            return;
        }

        await _next(context);
    }

    private static async Task WriteJsonResponse(HttpContext context, string message)
    {
        var response = new { message };
        context.Response.ContentType = "application/json";
        await JsonSerializer.SerializeAsync(context.Response.Body, response);
    }
}
