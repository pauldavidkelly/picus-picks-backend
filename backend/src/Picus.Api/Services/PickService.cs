using Microsoft.EntityFrameworkCore;
using Picus.Api.Data;
using Picus.Api.Models;
using Picus.Api.Models.DTOs;

namespace Picus.Api.Services;

public class PickService : IPickService
{
    private readonly PicusDbContext _context;
    private readonly ILogger<PickService> _logger;

    public PickService(PicusDbContext context, ILogger<PickService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Pick> SubmitPickAsync(int userId, SubmitPickDto pickDto)
    {
        var game = await _context.Games
            .Include(g => g.HomeTeam)
            .Include(g => g.AwayTeam)
            .FirstOrDefaultAsync(g => g.Id == pickDto.GameId);

        if (game == null)
        {
            throw new ArgumentException($"Game with ID {pickDto.GameId} not found");
        }

        if (DateTime.UtcNow > game.PickDeadline)
        {
            throw new InvalidOperationException($"Pick deadline has passed for game {game.Id}");
        }

        // Validate selected team
        if (pickDto.SelectedTeamId != game.HomeTeamId && pickDto.SelectedTeamId != game.AwayTeamId)
        {
            throw new ArgumentException($"Selected team {pickDto.SelectedTeamId} is not playing in game {game.Id}");
        }

        // Check for existing pick
        var existingPick = await _context.Picks
            .FirstOrDefaultAsync(p => p.UserId == userId && p.GameId == game.Id);

        if (existingPick != null)
        {
            // Update existing pick
            existingPick.SelectedTeamId = pickDto.SelectedTeamId;
            existingPick.Notes = pickDto.Notes;
            existingPick.SubmissionTime = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existingPick;
        }

        // Create new pick
        var pick = new Pick
        {
            UserId = userId,
            GameId = game.Id,
            SelectedTeamId = pickDto.SelectedTeamId,
            Notes = pickDto.Notes,
            SubmissionTime = DateTime.UtcNow,
            Points = 0 // Points will be updated when game is completed
        };

        _context.Picks.Add(pick);
        await _context.SaveChangesAsync();

        return pick;
    }

    public async Task<IEnumerable<Pick>> GetUserPicksByWeekAsync(int userId, int week, int season)
    {
        return await _context.Picks
            .Include(p => p.Game)
            .Include(p => p.SelectedTeam)
            .Where(p => p.UserId == userId && p.Game.Week == week && p.Game.Season == season)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pick>> GetLeaguePicksByWeekAsync(int leagueId, int week, int season)
    {
        var leagueUsers = await _context.Users
            .Where(u => u.LeagueId == leagueId)
            .Select(u => u.Id)
            .ToListAsync();

        return await _context.Picks
            .Include(p => p.Game)
            .Include(p => p.SelectedTeam)
            .Include(p => p.User)
            .Where(p => leagueUsers.Contains(p.UserId) && 
                       p.Game.Week == week && 
                       p.Game.Season == season)
            .ToListAsync();
    }

    public async Task<IEnumerable<VisiblePickDto>> ApplyPickVisibilityRulesAsync(IEnumerable<Pick> picks)
    {
        var visiblePicks = new List<VisiblePickDto>();

        foreach (var pick in picks)
        {
            var game = await _context.Games.FindAsync(pick.GameId);
            if (game == null) continue;

            bool isVisible = DateTime.UtcNow > game.PickDeadline;

            visiblePicks.Add(new VisiblePickDto
            {
                UserId = pick.UserId,
                GameId = pick.GameId,
                SelectedTeamId = isVisible ? pick.SelectedTeamId : null,
                HasPick = true,
                IsVisible = isVisible
            });
        }

        return visiblePicks;
    }

    public async Task<bool> UserBelongsToLeagueAsync(int userId, int leagueId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
            
        return user?.LeagueId == leagueId;
    }

    public async Task<PicksStatusDto> GetPickStatusAsync(int userId, int week, int season)
    {
        var games = await _context.Games
            .Where(g => g.Week == week && g.Season == season)
            .ToListAsync();

        var picks = await _context.Picks
            .Where(p => p.UserId == userId && 
                       p.Game.Week == week && 
                       p.Game.Season == season)
            .ToListAsync();

        var gamesNeedingPicks = games
            .Where(g => !picks.Any(p => p.GameId == g.Id) && DateTime.UtcNow <= g.PickDeadline)
            .Select(g => g.Id)
            .ToList();

        return new PicksStatusDto
        {
            Week = week,
            Season = season,
            TotalGames = games.Count,
            PicksMade = picks.Count,
            IsComplete = !gamesNeedingPicks.Any(),
            GamesNeedingPicks = gamesNeedingPicks
        };
    }
}
