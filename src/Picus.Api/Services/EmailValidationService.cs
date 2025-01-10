using Microsoft.Extensions.Options;
using Picus.Api.Configuration;

namespace Picus.Api.Services;

/// <summary>
/// Service for validating email addresses against the allowed list
/// </summary>
public class EmailValidationService : IEmailValidationService
{
    private readonly ILogger<EmailValidationService> _logger;
    private readonly HashSet<string> _allowedEmails;

    public EmailValidationService(
        IOptions<AllowedEmailsConfig> config,
        ILogger<EmailValidationService> logger)
    {
        _logger = logger;
        _allowedEmails = new HashSet<string>(
            config.Value.Entries.Select(e => e.ToLowerInvariant()),
            StringComparer.OrdinalIgnoreCase
        );
    }

    /// <inheritdoc />
    public bool IsEmailAllowed(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("Attempted to validate null or empty email");
            return false;
        }

        var isAllowed = _allowedEmails.Contains(email);
        
        if (!isAllowed)
        {
            _logger.LogWarning("Email {Email} is not in the allowed list", email);
        }

        return isAllowed;
    }
}
