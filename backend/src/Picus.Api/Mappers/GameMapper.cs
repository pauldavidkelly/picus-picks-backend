using Picus.Api.Models;
using Picus.Api.Models.SportsDb;

namespace Picus.Api.Mappers;

public static class GameMapper
{
    public static Models.Game ToGameEntity(this Models.SportsDb.Game sportsDbGame)
    {
        if (sportsDbGame == null)
            throw new ArgumentNullException(nameof(sportsDbGame));

        var gameTime = ParseGameDateTime(sportsDbGame.Date, sportsDbGame.Time);
        var week = ParseWeek(sportsDbGame.IntRound);
        var isPlayoffs = DetermineIfPlayoffs(week);
        
        return new Models.Game
        {
            ExternalGameId = sportsDbGame.Id,
            GameTime = gameTime,
            PickDeadline = gameTime, // You might want to adjust this based on your requirements
            Location = sportsDbGame.StrVenue ?? string.Empty,
            HomeTeamScore = ParseScore(sportsDbGame.HomeScore),
            AwayTeamScore = ParseScore(sportsDbGame.AwayScore),
            IsCompleted = sportsDbGame.HomeScore != null && sportsDbGame.AwayScore != null,
            Week = week,
            IsPlayoffs = isPlayoffs,
            Season = DetermineSeason(gameTime) // We'll determine season from the game date
        };
    }

    private static DateTime ParseGameDateTime(string date, string time)
    {
        if (string.IsNullOrEmpty(date))
            throw new ArgumentException("Date cannot be null or empty", nameof(date));

        // Combine date and time, defaulting to midnight if time is not provided
        var dateStr = date;
        var timeStr = string.IsNullOrEmpty(time) ? "00:00:00" : time;

        if (DateTime.TryParse($"{dateStr} {timeStr}", out DateTime result))
            return DateTime.SpecifyKind(result, DateTimeKind.Utc);

        throw new ArgumentException($"Invalid date/time format. Date: {date}, Time: {time}");
    }

    private static int? ParseScore(string? score)
    {
        if (string.IsNullOrEmpty(score))
            return null;

        return int.TryParse(score, out int result) ? result : null;
    }

    public static int ParseWeek(string? round)
    {
        if (string.IsNullOrEmpty(round))
            return 0;

        // Handle playoff weeks
        if (round == "160") return 19;  // Wild Card Round
        if (round == "161") return 20;  // Divisional Round
        if (round == "162") return 21;  // Conference Championships
        if (round == "163") return 22;  // Super Bowl

        // Remove any non-digit characters and parse
        var weekStr = new string(round.Where(char.IsDigit).ToArray());
        if (int.TryParse(weekStr, out int week))
            return week;

        return 0;
    }

    private static bool DetermineIfPlayoffs(int week)
    {
        // In NFL, regular season is weeks 1-18, playoffs are weeks 19-22
        return week >= 19 && week <= 22;
    }

    private static int DetermineSeason(DateTime gameTime)
    {
        // NFL season spans two years, with most games in the first year
        // If the game is in January/February, it's part of the previous year's season
        return gameTime.Month <= 2 ? gameTime.Year - 1 : gameTime.Year;
    }
}
