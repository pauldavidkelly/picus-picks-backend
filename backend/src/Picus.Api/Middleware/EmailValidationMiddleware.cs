using System.Security.Claims;
using Picus.Api.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Picus.Api.Middleware;

/// <summary>
/// Middleware that validates user's email against the allowed list and creates users if they don't exist
/// </summary>
public class EmailValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EmailValidationMiddleware> _logger;
    private static readonly string[] EmailClaimTypes = new[]
    {
        ClaimTypes.Email,
        "email",
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
        "https://picus-picks/email"
    };

    public EmailValidationMiddleware(
        RequestDelegate next,
        ILogger<EmailValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context, 
        IEmailValidationService emailValidationService,
        IUserService userService)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var emailClaim = context.User.Claims.FirstOrDefault(c => EmailClaimTypes.Contains(c.Type));
        if (emailClaim == null)
        {
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

        // Get Auth0 ID from claims
        var auth0Id = context.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (auth0Id == null)
        {
            _logger.LogWarning("No Auth0 ID found in claims for user with email: {Email}", emailClaim.Value);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await WriteJsonResponse(context, "User ID is required.");
            return;
        }

        // Create or get user
        try
        {
            var user = await userService.GetOrCreateUserAsync(auth0Id, emailClaim.Value);
            if (user == null)
            {
                _logger.LogError("Failed to create or get user with Auth0Id: {Auth0Id}", auth0Id);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await WriteJsonResponse(context, "Failed to process user account.");
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user with Auth0Id: {Auth0Id}", auth0Id);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await WriteJsonResponse(context, "An error occurred while processing your account.");
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
