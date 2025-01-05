using System.Security.Claims;
using Picus.Api.Services;

namespace Picus.Api.Middleware;

/// <summary>
/// Middleware that validates user's email against the allowed list
/// </summary>
public class EmailValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IEmailValidationService _emailValidationService;
    private readonly ILogger<EmailValidationMiddleware> _logger;

    public EmailValidationMiddleware(
        RequestDelegate next,
        IEmailValidationService emailValidationService,
        ILogger<EmailValidationMiddleware> logger)
    {
        _next = next;
        _emailValidationService = emailValidationService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip validation for non-authenticated requests
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        var email = context.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Authenticated request without email claim");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Email claim is required." });
            return;
        }

        if (!_emailValidationService.IsEmailAllowed(email))
        {
            _logger.LogWarning("Access attempt from unauthorized email: {Email}", email);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "This site is currently invitation-only. If you believe you should have access, please contact the administrator."
            });
            return;
        }

        await _next(context);
    }
}
