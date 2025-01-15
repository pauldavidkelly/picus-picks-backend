using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using PicusPicks.Web.Models;

namespace PicusPicks.Web.Services;

/// <summary>
/// Service for managing league table statistics
/// </summary>
public class LeagueTableService : ILeagueTableService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LeagueTableService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LeagueTableService(
        HttpClient httpClient,
        ILogger<LeagueTableService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<LeagueTableStats>> GetLeagueTableStatsAsync()
    {
        try
        {
            // Get the auth service
            var authService = _httpContextAccessor.HttpContext!.RequestServices
                .GetService<IAuthenticationService>();

            if (authService == null)
            {
                _logger.LogWarning("Authentication service not available");
                return Enumerable.Empty<LeagueTableStats>();
            }

            // Authenticate to get the token
            var authResult = await authService.AuthenticateAsync(
                _httpContextAccessor.HttpContext,
                OpenIdConnectDefaults.AuthenticationScheme);

            if (!authResult.Succeeded)
            {
                _logger.LogWarning("No access token available for league table stats request");
                return Enumerable.Empty<LeagueTableStats>();
            }

            var accessToken = authResult.Properties?.GetTokenValue("access_token");

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("No access token found in authentication properties");
                return Enumerable.Empty<LeagueTableStats>();
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Get all users' picks from the API
            var response = await _httpClient.GetAsync("/api/picks/stats");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("API request failed with status {Status}", response.StatusCode);
                return Enumerable.Empty<LeagueTableStats>();
            }

            var content = await response.Content.ReadFromJsonAsync<IEnumerable<LeagueTableStats>>();
            
            if (content == null)
            {
                _logger.LogWarning("Received null response from picks stats endpoint");
                return Enumerable.Empty<LeagueTableStats>();
            }

            return content.OrderByDescending(x => x.SuccessRate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching league table stats");
            return Enumerable.Empty<LeagueTableStats>();
        }
    }
}
