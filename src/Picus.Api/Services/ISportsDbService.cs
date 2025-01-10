using Picus.Api.Models;

namespace Picus.Api.Services
{
    public interface ISportsDbService
    {
        Task<List<Models.SportsDb.Game>> GetLeagueScheduleAsync(int leagueId, int season);
        // Future methods can be added here
    }
}