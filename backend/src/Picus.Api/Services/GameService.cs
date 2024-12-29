using Microsoft.EntityFrameworkCore;
using Picus.Api.Data;
using Picus.Api.Mappers;
using Picus.Api.Models;
using Picus.Api.Models.DTOs;

namespace Picus.Api.Services;

public interface IGameService
{
    Task<IEnumerable<Game>> UpsertGamesForSeasonAsync(int leagueId, int season);
    Task<GameDTO?> GetGameByIdAsync(int id);
    Task<IEnumerable<GameDTO>> GetGamesByWeekAndSeasonAsync(int week, int season);
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

    public async Task<GameDTO?> GetGameByIdAsync(int id)
    {
        var game = await _dbContext.Games
            .Include(g => g.HomeTeam)
            .Include(g => g.AwayTeam)
            .Include(g => g.WinningTeam)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null)
            return null;

        return new GameDTO
        {
            Id = game.Id,
            ExternalGameId = game.ExternalGameId,
            GameTime = game.GameTime,
            PickDeadline = game.PickDeadline,
            Week = game.Week,
            Season = game.Season,
            IsCompleted = game.IsCompleted,
            IsPlayoffs = game.IsPlayoffs,
            Location = game.Location,
            HomeTeamScore = game.HomeTeamScore,
            AwayTeamScore = game.AwayTeamScore,
            HomeTeam = new TeamDTO
            {
                Id = game.HomeTeam.Id,
                Name = game.HomeTeam.Name,
                Abbreviation = game.HomeTeam.Abbreviation,
                City = game.HomeTeam.City,
                IconUrl = game.HomeTeam.IconUrl,
                BannerUrl = game.HomeTeam.BannerUrl,
                PrimaryColor = game.HomeTeam.PrimaryColor,
                SecondaryColor = game.HomeTeam.SecondaryColor,
                TertiaryColor = game.HomeTeam.TertiaryColor,
                Conference = game.HomeTeam.Conference,
                Division = game.HomeTeam.Division
            },
            AwayTeam = new TeamDTO
            {
                Id = game.AwayTeam.Id,
                Name = game.AwayTeam.Name,
                Abbreviation = game.AwayTeam.Abbreviation,
                City = game.AwayTeam.City,
                IconUrl = game.AwayTeam.IconUrl,
                BannerUrl = game.AwayTeam.BannerUrl,
                PrimaryColor = game.AwayTeam.PrimaryColor,
                SecondaryColor = game.AwayTeam.SecondaryColor,
                TertiaryColor = game.AwayTeam.TertiaryColor,
                Conference = game.AwayTeam.Conference,
                Division = game.AwayTeam.Division
            },
            WinningTeam = game.WinningTeam == null ? null : new TeamDTO
            {
                Id = game.WinningTeam.Id,
                Name = game.WinningTeam.Name,
                Abbreviation = game.WinningTeam.Abbreviation,
                City = game.WinningTeam.City,
                IconUrl = game.WinningTeam.IconUrl,
                BannerUrl = game.WinningTeam.BannerUrl,
                PrimaryColor = game.WinningTeam.PrimaryColor,
                SecondaryColor = game.WinningTeam.SecondaryColor,
                TertiaryColor = game.WinningTeam.TertiaryColor,
                Conference = game.WinningTeam.Conference,
                Division = game.WinningTeam.Division
            }
        };
    }

    public async Task<IEnumerable<GameDTO>> GetGamesByWeekAndSeasonAsync(int week, int season)
    {
        var games = await _dbContext.Games
            .Include(g => g.HomeTeam)
            .Include(g => g.AwayTeam)
            .Include(g => g.WinningTeam)
            .Where(g => g.Week == week && g.Season == season)
            .OrderBy(g => g.GameTime)
            .ToListAsync();

        return games.Select(game => new GameDTO
        {
            Id = game.Id,
            ExternalGameId = game.ExternalGameId,
            GameTime = game.GameTime,
            PickDeadline = game.PickDeadline,
            Week = game.Week,
            Season = game.Season,
            IsCompleted = game.IsCompleted,
            IsPlayoffs = game.IsPlayoffs,
            Location = game.Location,
            HomeTeamScore = game.HomeTeamScore,
            AwayTeamScore = game.AwayTeamScore,
            HomeTeam = new TeamDTO
            {
                Id = game.HomeTeam.Id,
                Name = game.HomeTeam.Name,
                Abbreviation = game.HomeTeam.Abbreviation,
                City = game.HomeTeam.City,
                IconUrl = game.HomeTeam.IconUrl,
                BannerUrl = game.HomeTeam.BannerUrl,
                PrimaryColor = game.HomeTeam.PrimaryColor,
                SecondaryColor = game.HomeTeam.SecondaryColor,
                TertiaryColor = game.HomeTeam.TertiaryColor,
                Conference = game.HomeTeam.Conference,
                Division = game.HomeTeam.Division
            },
            AwayTeam = new TeamDTO
            {
                Id = game.AwayTeam.Id,
                Name = game.AwayTeam.Name,
                Abbreviation = game.AwayTeam.Abbreviation,
                City = game.AwayTeam.City,
                IconUrl = game.AwayTeam.IconUrl,
                BannerUrl = game.AwayTeam.BannerUrl,
                PrimaryColor = game.AwayTeam.PrimaryColor,
                SecondaryColor = game.AwayTeam.SecondaryColor,
                TertiaryColor = game.AwayTeam.TertiaryColor,
                Conference = game.AwayTeam.Conference,
                Division = game.AwayTeam.Division
            },
            WinningTeam = game.WinningTeam == null ? null : new TeamDTO
            {
                Id = game.WinningTeam.Id,
                Name = game.WinningTeam.Name,
                Abbreviation = game.WinningTeam.Abbreviation,
                City = game.WinningTeam.City,
                IconUrl = game.WinningTeam.IconUrl,
                BannerUrl = game.WinningTeam.BannerUrl,
                PrimaryColor = game.WinningTeam.PrimaryColor,
                SecondaryColor = game.WinningTeam.SecondaryColor,
                TertiaryColor = game.WinningTeam.TertiaryColor,
                Conference = game.WinningTeam.Conference,
                Division = game.WinningTeam.Division
            }
        }).ToList();
    }
}
