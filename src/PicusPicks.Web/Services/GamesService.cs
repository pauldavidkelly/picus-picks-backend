using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using PicusPicks.Web.Models;

namespace PicusPicks.Web.Services;

public interface IGamesService
{
    Task<IEnumerable<GameDTO>?> SyncGamesAsync(int leagueId, int season);
}

public class GamesService : IGamesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GamesService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GamesService(
        HttpClient httpClient, 
        ILogger<GamesService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<GameDTO>?> SyncGamesAsync(int leagueId, int season)
    {
        try
        {
            var accessToken = await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new InvalidOperationException("No access token available");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync($"api/Games/upsert/{leagueId}/{season}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<GameDTO>>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing games for league {LeagueId} and season {Season}", leagueId, season);
            throw;
        }
    }
} 