using Microsoft.EntityFrameworkCore;
using Picus.Api.Data;
using Picus.Api.Mappers;
using Picus.Api.Models;

namespace Picus.Api.Services;

public interface IGameService
{
    Task<IEnumerable<Game>> UpsertGamesForSeasonAsync(int leagueId, int season);
}

public class GameService : IGameService
{
    private readonly IRepository<Game> _gameRepository;
    private readonly ISportsDbService _sportsDbService;
    private readonly PicusDbContext _dbContext;

    public GameService(
        IRepository<Game> gameRepository,
        ISportsDbService sportsDbService,
        PicusDbContext dbContext)
    {
        _gameRepository = gameRepository;
        _sportsDbService = sportsDbService;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Game>> UpsertGamesForSeasonAsync(int leagueId, int season)
    {
        // Get games from SportsDb API
        var sportsDbGames = await _sportsDbService.GetLeagueScheduleAsync(leagueId, season);
        
        var upsertedGames = new List<Game>();
        
        foreach (var sportsDbGame in sportsDbGames)
        {
            // Map the SportsDb game to our model
            var game = sportsDbGame.ToGameEntity();
            
            // Try to find existing game by external ID
            var existingGame = await _dbContext.Games
                .FirstOrDefaultAsync(g => g.ExternalGameId == game.ExternalGameId);

            if (existingGame != null)
            {
                // Update existing game
                existingGame.GameTime = game.GameTime;
                existingGame.PickDeadline = game.PickDeadline;
                existingGame.Location = game.Location;
                existingGame.HomeTeamScore = game.HomeTeamScore;
                existingGame.AwayTeamScore = game.AwayTeamScore;
                existingGame.IsCompleted = game.IsCompleted;
                
                await _gameRepository.UpdateAsync(existingGame);
                upsertedGames.Add(existingGame);
            }
            else
            {
                // Add new game
                var newGame = await _gameRepository.AddAsync(game);
                upsertedGames.Add(newGame);
            }
        }

        return upsertedGames;
    }
}
