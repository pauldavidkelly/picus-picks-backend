namespace Picus.Api.Middleware;

/// <summary>
/// Extension methods for the EmailValidationMiddleware
/// </summary>
public static class EmailValidationMiddlewareExtensions
{
    /// <summary>
    /// Adds the email validation middleware to validate user emails against the allowed list
    /// </summary>
    public static IApplicationBuilder UseEmailValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EmailValidationMiddleware>();
    }
}
