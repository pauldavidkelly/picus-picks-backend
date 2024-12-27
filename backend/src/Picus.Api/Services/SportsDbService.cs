using System.Text.Json;
using Picus.Api.Models.SportsDb;

namespace Picus.Api.Services
{
    public class SportsDbService : ISportsDbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public SportsDbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = configuration.GetSection("SportsDbApi").GetValue<string>("Url") ?? throw new InvalidOperationException("SportsDbApi:Url is not configured");
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", configuration.GetSection("SportsDbApi").GetValue<string>("ApiKey") ?? throw new InvalidOperationException("SportsDbApi:ApiKey is not configured"));

        }

        public async Task<List<Game>> GetLeagueScheduleAsync(int leagueId, int season)
        {
            try
            {
                // Construct the URL using string interpolation for readability
                string url = $"{_baseUrl}/schedule/league/{leagueId}/{season}";

                // Make the API call and deserialize the response
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var schedule = JsonSerializer.Deserialize<GameSchedule>(content, 
                    new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });

                return schedule?.Schedule ?? new List<Game>();
            }
            catch (HttpRequestException ex)
            {
                // Log the exception details here if you have a logging framework
                throw new SportsDbException("Failed to retrieve league schedule", ex);
            }
            catch (JsonException ex)
            {
                throw new SportsDbException("Failed to parse league schedule response", ex);
            }
        }

    }

    // Custom exception for better error handling
    public class SportsDbException : Exception
    {
        public SportsDbException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}