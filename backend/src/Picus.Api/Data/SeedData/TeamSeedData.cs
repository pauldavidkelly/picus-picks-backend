using Picus.Api.Models;
using Picus.Api.Models.Enums;

namespace Picus.Api.Data.SeedData;

public static class TeamSeedData
{
    public static IEnumerable<Team> GetTeams()
    {
        return new List<Team>
        {
            // AFC North
            new Team
            {
                Id = 1,
                ExternalTeamId = "33",
                Name = "Ravens",
                City = "Baltimore",
                Abbreviation = "BAL",
                Conference = ConferenceType.AFC,
                Division = DivisionType.North,
                PrimaryColor = "#241773",
                SecondaryColor = "#000000",
                TertiaryColor = "#9E7C0C",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/bal.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/bal.png"
            },
            new Team
            {
                Id = 2,
                ExternalTeamId = "4",
                Name = "Bengals",
                City = "Cincinnati",
                Abbreviation = "CIN",
                Conference = ConferenceType.AFC,
                Division = DivisionType.North,
                PrimaryColor = "#FB4F14",
                SecondaryColor = "#000000",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/cin.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/cin.png"
            },
            new Team
            {
                Id = 3,
                ExternalTeamId = "5",
                Name = "Browns",
                City = "Cleveland",
                Abbreviation = "CLE",
                Conference = ConferenceType.AFC,
                Division = DivisionType.North,
                PrimaryColor = "#311D00",
                SecondaryColor = "#FF3C00",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/cle.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/cle.png"
            },
            new Team
            {
                Id = 4,
                ExternalTeamId = "23",
                Name = "Steelers",
                City = "Pittsburgh",
                Abbreviation = "PIT",
                Conference = ConferenceType.AFC,
                Division = DivisionType.North,
                PrimaryColor = "#FFB612",
                SecondaryColor = "#101820",
                TertiaryColor = "#A5ACAF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/pit.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/pit.png"
            },

            // AFC South
            new Team
            {
                Id = 5,
                ExternalTeamId = "34",
                Name = "Texans",
                City = "Houston",
                Abbreviation = "HOU",
                Conference = ConferenceType.AFC,
                Division = DivisionType.South,
                PrimaryColor = "#03202F",
                SecondaryColor = "#A71930",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/hou.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/hou.png"
            },
            new Team
            {
                Id = 6,
                ExternalTeamId = "11",
                Name = "Colts",
                City = "Indianapolis",
                Abbreviation = "IND",
                Conference = ConferenceType.AFC,
                Division = DivisionType.South,
                PrimaryColor = "#002C5F",
                SecondaryColor = "#A2AAAD",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/ind.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ind.png"
            },
            new Team
            {
                Id = 7,
                ExternalTeamId = "30",
                Name = "Jaguars",
                City = "Jacksonville",
                Abbreviation = "JAX",
                Conference = ConferenceType.AFC,
                Division = DivisionType.South,
                PrimaryColor = "#006778",
                SecondaryColor = "#9F792C",
                TertiaryColor = "#000000",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/jax.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/jax.png"
            },
            new Team
            {
                Id = 8,
                ExternalTeamId = "10",
                Name = "Titans",
                City = "Tennessee",
                Abbreviation = "TEN",
                Conference = ConferenceType.AFC,
                Division = DivisionType.South,
                PrimaryColor = "#0C2340",
                SecondaryColor = "#4B92DB",
                TertiaryColor = "#C8102E",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/ten.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ten.png"
            },

            // AFC East
            new Team
            {
                Id = 9,
                ExternalTeamId = "2",
                Name = "Bills",
                City = "Buffalo",
                Abbreviation = "BUF",
                Conference = ConferenceType.AFC,
                Division = DivisionType.East,
                PrimaryColor = "#00338D",
                SecondaryColor = "#C60C30",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/buf.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/buf.png"
            },
            new Team
            {
                Id = 10,
                ExternalTeamId = "15",
                Name = "Dolphins",
                City = "Miami",
                Abbreviation = "MIA",
                Conference = ConferenceType.AFC,
                Division = DivisionType.East,
                PrimaryColor = "#008E97",
                SecondaryColor = "#FC4C02",
                TertiaryColor = "#005778",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/mia.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/mia.png"
            },
            new Team
            {
                Id = 11,
                ExternalTeamId = "17",
                Name = "Patriots",
                City = "New England",
                Abbreviation = "NE",
                Conference = ConferenceType.AFC,
                Division = DivisionType.East,
                PrimaryColor = "#002244",
                SecondaryColor = "#C60C30",
                TertiaryColor = "#B0B7BC",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/ne.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ne.png"
            },
            new Team
            {
                Id = 12,
                ExternalTeamId = "20",
                Name = "Jets",
                City = "New York",
                Abbreviation = "NYJ",
                Conference = ConferenceType.AFC,
                Division = DivisionType.East,
                PrimaryColor = "#125740",
                SecondaryColor = "#000000",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/nyj.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/nyj.png"
            },

            // AFC West
            new Team
            {
                Id = 13,
                ExternalTeamId = "7",
                Name = "Broncos",
                City = "Denver",
                Abbreviation = "DEN",
                Conference = ConferenceType.AFC,
                Division = DivisionType.West,
                PrimaryColor = "#FB4F14",
                SecondaryColor = "#002244",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/den.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/den.png"
            },
            new Team
            {
                Id = 14,
                ExternalTeamId = "12",
                Name = "Chiefs",
                City = "Kansas City",
                Abbreviation = "KC",
                Conference = ConferenceType.AFC,
                Division = DivisionType.West,
                PrimaryColor = "#E31837",
                SecondaryColor = "#FFB81C",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/kc.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/kc.png"
            },
            new Team
            {
                Id = 15,
                ExternalTeamId = "13",
                Name = "Raiders",
                City = "Las Vegas",
                Abbreviation = "LV",
                Conference = ConferenceType.AFC,
                Division = DivisionType.West,
                PrimaryColor = "#000000",
                SecondaryColor = "#A5ACAF",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/lv.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lv.png"
            },
            new Team
            {
                Id = 16,
                ExternalTeamId = "24",
                Name = "Chargers",
                City = "Los Angeles",
                Abbreviation = "LAC",
                Conference = ConferenceType.AFC,
                Division = DivisionType.West,
                PrimaryColor = "#0080C6",
                SecondaryColor = "#FFC20E",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/lac.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lac.png"
            },

            // NFC North
            new Team
            {
                Id = 17,
                ExternalTeamId = "3",
                Name = "Bears",
                City = "Chicago",
                Abbreviation = "CHI",
                Conference = ConferenceType.NFC,
                Division = DivisionType.North,
                PrimaryColor = "#0B162A",
                SecondaryColor = "#C83803",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/chi.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/chi.png"
            },
            new Team
            {
                Id = 18,
                ExternalTeamId = "8",
                Name = "Lions",
                City = "Detroit",
                Abbreviation = "DET",
                Conference = ConferenceType.NFC,
                Division = DivisionType.North,
                PrimaryColor = "#0076B6",
                SecondaryColor = "#B0B7BC",
                TertiaryColor = "#000000",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/det.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/det.png"
            },
            new Team
            {
                Id = 19,
                ExternalTeamId = "9",
                Name = "Packers",
                City = "Green Bay",
                Abbreviation = "GB",
                Conference = ConferenceType.NFC,
                Division = DivisionType.North,
                PrimaryColor = "#203731",
                SecondaryColor = "#FFB612",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/gb.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/gb.png"
            },
            new Team
            {
                Id = 20,
                ExternalTeamId = "16",
                Name = "Vikings",
                City = "Minnesota",
                Abbreviation = "MIN",
                Conference = ConferenceType.NFC,
                Division = DivisionType.North,
                PrimaryColor = "#4F2683",
                SecondaryColor = "#FFC62F",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/min.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/min.png"
            },

            // NFC South
            new Team
            {
                Id = 21,
                ExternalTeamId = "1",
                Name = "Falcons",
                City = "Atlanta",
                Abbreviation = "ATL",
                Conference = ConferenceType.NFC,
                Division = DivisionType.South,
                PrimaryColor = "#A71930",
                SecondaryColor = "#000000",
                TertiaryColor = "#A5ACAF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/atl.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/atl.png"
            },
            new Team
            {
                Id = 22,
                ExternalTeamId = "29",
                Name = "Panthers",
                City = "Carolina",
                Abbreviation = "CAR",
                Conference = ConferenceType.NFC,
                Division = DivisionType.South,
                PrimaryColor = "#0085CA",
                SecondaryColor = "#101820",
                TertiaryColor = "#BFC0BF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/car.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/car.png"
            },
            new Team
            {
                Id = 23,
                ExternalTeamId = "18",
                Name = "Saints",
                City = "New Orleans",
                Abbreviation = "NO",
                Conference = ConferenceType.NFC,
                Division = DivisionType.South,
                PrimaryColor = "#D3BC8D",
                SecondaryColor = "#101820",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/no.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/no.png"
            },
            new Team
            {
                Id = 24,
                ExternalTeamId = "27",
                Name = "Buccaneers",
                City = "Tampa Bay",
                Abbreviation = "TB",
                Conference = ConferenceType.NFC,
                Division = DivisionType.South,
                PrimaryColor = "#D50A0A",
                SecondaryColor = "#FF7900",
                TertiaryColor = "#B1BABF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/tb.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/tb.png"
            },

            // NFC East
            new Team
            {
                Id = 25,
                ExternalTeamId = "6",
                Name = "Cowboys",
                City = "Dallas",
                Abbreviation = "DAL",
                Conference = ConferenceType.NFC,
                Division = DivisionType.East,
                PrimaryColor = "#003594",
                SecondaryColor = "#041E42",
                TertiaryColor = "#869397",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/dal.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/dal.png"
            },
            new Team
            {
                Id = 26,
                ExternalTeamId = "19",
                Name = "Giants",
                City = "New York",
                Abbreviation = "NYG",
                Conference = ConferenceType.NFC,
                Division = DivisionType.East,
                PrimaryColor = "#0B2265",
                SecondaryColor = "#A71930",
                TertiaryColor = "#A5ACAF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/nyg.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/nyg.png"
            },
            new Team
            {
                Id = 27,
                ExternalTeamId = "21",
                Name = "Eagles",
                City = "Philadelphia",
                Abbreviation = "PHI",
                Conference = ConferenceType.NFC,
                Division = DivisionType.East,
                PrimaryColor = "#004C54",
                SecondaryColor = "#A5ACAF",
                TertiaryColor = "#ACC0C6",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/phi.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/phi.png"
            },
            new Team
            {
                Id = 28,
                ExternalTeamId = "28",
                Name = "Commanders",
                City = "Washington",
                Abbreviation = "WSH",
                Conference = ConferenceType.NFC,
                Division = DivisionType.East,
                PrimaryColor = "#5A1414",
                SecondaryColor = "#FFB612",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/wsh.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/wsh.png"
            },

            // NFC West
            new Team
            {
                Id = 29,
                ExternalTeamId = "22",
                Name = "Cardinals",
                City = "Arizona",
                Abbreviation = "ARI",
                Conference = ConferenceType.NFC,
                Division = DivisionType.West,
                PrimaryColor = "#97233F",
                SecondaryColor = "#000000",
                TertiaryColor = "#FFB612",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/ari.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ari.png"
            },
            new Team
            {
                Id = 30,
                ExternalTeamId = "14",
                Name = "Rams",
                City = "Los Angeles",
                Abbreviation = "LAR",
                Conference = ConferenceType.NFC,
                Division = DivisionType.West,
                PrimaryColor = "#003594",
                SecondaryColor = "#FFA300",
                TertiaryColor = "#FF8200",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/lar.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lar.png"
            },
            new Team
            {
                Id = 31,
                ExternalTeamId = "25",
                Name = "49ers",
                City = "San Francisco",
                Abbreviation = "SF",
                Conference = ConferenceType.NFC,
                Division = DivisionType.West,
                PrimaryColor = "#AA0000",
                SecondaryColor = "#B3995D",
                TertiaryColor = "#FFFFFF",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/sf.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/sf.png"
            },
            new Team
            {
                Id = 32,
                ExternalTeamId = "26",
                Name = "Seahawks",
                City = "Seattle",
                Abbreviation = "SEA",
                Conference = ConferenceType.NFC,
                Division = DivisionType.West,
                PrimaryColor = "#002244",
                SecondaryColor = "#69BE28",
                TertiaryColor = "#A5ACB0",
                IconUrl = "https://a.espncdn.com/i/teamlogos/nfl/500/sea.png",
                BannerUrl = "https://a.espncdn.com/i/teamlogos/nfl/500-dark/sea.png"
            }
        };
    }
}
