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

    public EmailValidationMiddleware(
        RequestDelegate next,
        ILogger<EmailValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IEmailValidationService emailValidationService)
    {
        // Skip validation for non-authenticated requests
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var emailClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
        if (emailClaim == null)
        {
            _logger.LogWarning("Authenticated request without email claim");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await WriteJsonResponse(context, "Email claim is required.");
            return;
        }

        if (!emailValidationService.IsEmailAllowed(emailClaim.Value))
        {
            _logger.LogWarning("Access attempt from unauthorized email: {Email}", emailClaim.Value);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await WriteJsonResponse(context, "This site is currently invitation-only. If you believe you should have access, please contact the administrator.");
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
