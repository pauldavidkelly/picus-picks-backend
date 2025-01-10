namespace Picus.Api.Configuration;

/// <summary>
/// Configuration for allowed email addresses
/// </summary>
public class AllowedEmailsConfig
{
    public const string SectionName = "AllowedEmails";
    
    /// <summary>
    /// List of email addresses that are allowed to access the application
    /// </summary>
    public List<string> Entries { get; set; } = new();
}
