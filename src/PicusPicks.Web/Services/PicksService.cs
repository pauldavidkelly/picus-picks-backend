using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Picus.Api.Models.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PicusPicks.Web.Services;

public class PicksService : IPicksService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PicksService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public PicksService(
        HttpClient httpClient,
        ILogger<PicksService> logger,
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

    public async Task<(IEnumerable<VisiblePickDto> picks, PicksStatusDto status)> GetMyPicksForWeekAsync(int week, int season)
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

            var response = await _httpClient.GetAsync($"api/picks/my-picks/week/{week}/season/{season}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            
            var picksArray = doc.RootElement.GetProperty("picks");
            var statusObj = doc.RootElement.GetProperty("status");
            
            var picks = JsonSerializer.Deserialize<IEnumerable<VisiblePickDto>>(picksArray.GetRawText(), _jsonOptions);
            var status = JsonSerializer.Deserialize<PicksStatusDto>(statusObj.GetRawText(), _jsonOptions);

            if (picks == null || status == null)
            {
                throw new InvalidOperationException("Failed to deserialize picks data");
            }

            return (picks, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting picks for week {Week}", week);
            throw;
        }
    }

    public async Task<PicksStatusDto> GetPickStatusAsync(int week, int season)
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

            var status = await _httpClient.GetFromJsonAsync<PicksStatusDto>($"api/picks/status/week/{week}/season/{season}", _jsonOptions);
            if (status == null)
            {
                throw new InvalidOperationException("Failed to get pick status");
            }
            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pick status for week {Week}", week);
            throw;
        }
    }

    public async Task<VisiblePickDto> SubmitPickAsync(SubmitPickDto pickDto)
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

            var response = await _httpClient.PostAsJsonAsync("api/picks", pickDto);
            response.EnsureSuccessStatusCode();
            
            var pick = await response.Content.ReadFromJsonAsync<VisiblePickDto>(_jsonOptions);
            if (pick == null)
            {
                throw new InvalidOperationException("Failed to submit pick");
            }
            return pick;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting pick for game {GameId}", pickDto.GameId);
            throw;
        }
    }

    public async Task<IEnumerable<VisiblePickDto>> GetAllPicksForWeekAsync(int week, int season)
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

            var picks = await _httpClient.GetFromJsonAsync<IEnumerable<VisiblePickDto>>($"api/picks/week/{week}/season/{season}", _jsonOptions);
            if (picks == null)
            {
                throw new InvalidOperationException("Failed to get picks");
            }
            return picks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all picks for week {Week}", week);
            throw;
        }
    }
} 