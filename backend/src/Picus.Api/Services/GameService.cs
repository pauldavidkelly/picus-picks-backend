using Microsoft.EntityFrameworkCore;
using Picus.Api.Data;
using Picus.Api.Mappers;
using Picus.Api.Models;

namespace Picus.Api.Services;

public interface IGameService
{
    Task<IEnumerable<Game>> UpsertGamesForSeasonAsync(int leagueId, int season);
    Task<Game?> GetGameByIdAsync(int id);
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

        // Filter out preseason games (week 500)
        sportsDbGames = sportsDbGames.Where(g => GameMapper.ParseWeek(g.IntRound) != 500).ToList();

        var upsertedGames = new List<Game>();

        foreach (var sportsDbGame in sportsDbGames)
        {
            // Look up teams by their external IDs
            var homeTeam = await _dbContext.Teams.FirstOrDefaultAsync(t => t.ExternalTeamId == sportsDbGame.HomeTeamId);
            var awayTeam = await _dbContext.Teams.FirstOrDefaultAsync(t => t.ExternalTeamId == sportsDbGame.AwayTeamId);

            if (homeTeam == null || awayTeam == null)
            {
                // Log warning and skip this game if teams not found
                Console.WriteLine($"Warning: Could not find teams for game {sportsDbGame.Id}. Home team ID: {sportsDbGame.HomeTeamId}, Away team ID: {sportsDbGame.AwayTeamId}");
                continue;
            }

            // Map the SportsDb game to our model
            var game = sportsDbGame.ToGameEntity();
            game.HomeTeamId = homeTeam.Id;
            game.AwayTeamId = awayTeam.Id;

            // Determine winning team if game is completed
            if (game.IsCompleted && game.HomeTeamScore.HasValue && game.AwayTeamScore.HasValue)
            {
                game.WinningTeamId = game.HomeTeamScore > game.AwayTeamScore ? game.HomeTeamId :
                                    game.AwayTeamScore > game.HomeTeamScore ? game.AwayTeamId :
                                    null; // null for tie games
            }

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
                existingGame.HomeTeamId = game.HomeTeamId;
                existingGame.AwayTeamId = game.AwayTeamId;
                existingGame.WinningTeamId = game.WinningTeamId;

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

    public async Task<Game?> GetGameByIdAsync(int id)
    {
        return await _dbContext.Games
            .Include(g => g.HomeTeam)
            .Include(g => g.AwayTeam)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}
