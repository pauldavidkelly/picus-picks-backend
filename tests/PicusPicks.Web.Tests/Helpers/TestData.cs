using System;
using System.Collections.Generic;
using PicusPicks.Web.Models.DTOs;

namespace PicusPicks.Web.Tests.Helpers;

public static class TestData
{
    public static IEnumerable<GameDTO> GetTestGames()
    {
        return new List<GameDTO>
        {
            new GameDTO
            {
                Id = 1,
                ExternalGameId = "2024_01_KC_BUF",
                GameTime = DateTime.UtcNow.AddDays(1),
                PickDeadline = DateTime.UtcNow.AddDays(1).AddMinutes(-15),
                Week = 1,
                Season = 2024,
                IsCompleted = false,
                IsPlayoffs = false,
                Location = "Arrowhead Stadium",
                HomeTeam = new TeamDTO
                {
                    Id = 1,
                    Name = "Chiefs",
                    City = "Kansas City",
                    IconUrl = "https://example.com/chiefs.png",
                    PrimaryColor = "#E31837"
                },
                AwayTeam = new TeamDTO
                {
                    Id = 2,
                    Name = "Bills",
                    City = "Buffalo",
                    IconUrl = "https://example.com/bills.png",
                    PrimaryColor = "#00338D"
                }
            }
        };
    }
} 