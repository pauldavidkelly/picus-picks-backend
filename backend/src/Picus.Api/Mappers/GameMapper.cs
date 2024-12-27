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
        
        return new Models.Game
        {
            ESPNGameId = sportsDbGame.Id,
            GameTime = gameTime,
            PickDeadline = gameTime, // You might want to adjust this based on your requirements
            Location = sportsDbGame.StrVenue ?? string.Empty,
            HomeTeamScore = ParseScore(sportsDbGame.HomeScore),
            AwayTeamScore = ParseScore(sportsDbGame.AwayScore),
            IsCompleted = sportsDbGame.HomeScore != null && sportsDbGame.AwayScore != null
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
            return result;

        throw new ArgumentException($"Invalid date/time format. Date: {date}, Time: {time}");
    }

    private static int? ParseScore(string? score)
    {
        if (string.IsNullOrEmpty(score))
            return null;

        return int.TryParse(score, out int result) ? result : null;
    }
}
