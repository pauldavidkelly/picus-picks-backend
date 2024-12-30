using Picus.Api.Models;
using Picus.Api.Models.DTOs;

namespace Picus.Api.Mappers;

public static class DTOMapper
{
    public static TeamDTO ToTeamDTO(this Team team)
    {
        return new TeamDTO
        {
            Id = team.Id,
            Name = team.Name,
            Abbreviation = team.Abbreviation,
            City = team.City,
            IconUrl = team.IconUrl,
            BannerUrl = team.BannerUrl,
            PrimaryColor = team.PrimaryColor,
            SecondaryColor = team.SecondaryColor,
            TertiaryColor = team.TertiaryColor,
            Conference = team.Conference,
            Division = team.Division
        };
    }

    public static GameDTO ToGameDTO(this Game game)
    {
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
            HomeTeam = game.HomeTeam.ToTeamDTO(),
            AwayTeam = game.AwayTeam.ToTeamDTO(),
            WinningTeam = game.WinningTeam?.ToTeamDTO()
        };
    }
}
