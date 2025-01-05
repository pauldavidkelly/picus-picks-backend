namespace Picus.Api.Services;

/// <summary>
/// Service for validating email addresses against the allowed list
/// </summary>
public interface IEmailValidationService
{
    /// <summary>
    /// Checks if the given email is in the allowed list
    /// </summary>
    /// <param name="email">Email address to check</param>
    /// <returns>True if the email is allowed, false otherwise</returns>
    bool IsEmailAllowed(string email);
}
