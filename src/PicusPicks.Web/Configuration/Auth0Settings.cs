namespace PicusPicks.Web.Configuration;

public class Auth0Settings
{
    public const string SectionName = "Auth0";
    
    public string Domain { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = string.Empty;
}
