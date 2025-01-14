using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Picus.Api.Models.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PicusPicks.Web.Services;

public interface IGamesService
{
    Task<IEnumerable<GameDTO>> SyncGamesAsync(int leagueId, int season);
    Task<IEnumerable<GameDTO>> GetGamesByWeekAndSeasonAsync(int week, int season);
}

public class GamesService : IGamesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GamesService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public GamesService(
        HttpClient httpClient, 
        ILogger<GamesService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<IEnumerable<GameDTO>> GetGamesByWeekAndSeasonAsync(int week, int season)
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

            var response = await _httpClient.GetAsync($"api/games/week/{week}/season/{season}");
            response.EnsureSuccessStatusCode();
            var games = await response.Content.ReadFromJsonAsync<IEnumerable<GameDTO>>(_jsonOptions);
            return games ?? Enumerable.Empty<GameDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting games for week {Week} and season {Season}", week, season);
            throw;
        }
    }

    public async Task<IEnumerable<GameDTO>> SyncGamesAsync(int leagueId, int season)
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

            var response = await _httpClient.PostAsync($"api/games/upsert/{leagueId}/{season}", null);
            response.EnsureSuccessStatusCode();
            var games = await response.Content.ReadFromJsonAsync<IEnumerable<GameDTO>>(_jsonOptions);
            return games ?? Enumerable.Empty<GameDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing games for league {LeagueId} and season {Season}", leagueId, season);
            throw;
        }
    }
} 