using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Picus.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAllNFLTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Abbreviation", "BannerUrl", "City", "Conference", "CreatedAt", "Division", "ExternalTeamId", "IconUrl", "Name", "PrimaryColor", "SecondaryColor", "TertiaryColor", "UpdatedAt" },
                values: new object[,]
                {
                    { 3, "CLE", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/cle.png", "Cleveland", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "134924", "https://a.espncdn.com/i/teamlogos/nfl/500/cle.png", "Browns", "#311D00", "#FF3C00", "#FFFFFF", null },
                    { 4, "PIT", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/pit.png", "Pittsburgh", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "134925", "https://a.espncdn.com/i/teamlogos/nfl/500/pit.png", "Steelers", "#FFB612", "#101820", "#A5ACAF", null },
                    { 5, "HOU", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/hou.png", "Houston", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "134926", "https://a.espncdn.com/i/teamlogos/nfl/500/hou.png", "Texans", "#03202F", "#A71930", "#FFFFFF", null },
                    { 6, "IND", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ind.png", "Indianapolis", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "134927", "https://a.espncdn.com/i/teamlogos/nfl/500/ind.png", "Colts", "#002C5F", "#A2AAAD", "#FFFFFF", null },
                    { 7, "JAX", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/jax.png", "Jacksonville", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "134928", "https://a.espncdn.com/i/teamlogos/nfl/500/jax.png", "Jaguars", "#006778", "#9F792C", "#000000", null },
                    { 8, "TEN", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ten.png", "Tennessee", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "134929", "https://a.espncdn.com/i/teamlogos/nfl/500/ten.png", "Titans", "#0C2340", "#4B92DB", "#C8102E", null },
                    { 9, "BUF", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/buf.png", "Buffalo", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "134918", "https://a.espncdn.com/i/teamlogos/nfl/500/buf.png", "Bills", "#00338D", "#C60C30", "#FFFFFF", null },
                    { 10, "MIA", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/mia.png", "Miami", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "134919", "https://a.espncdn.com/i/teamlogos/nfl/500/mia.png", "Dolphins", "#008E97", "#FC4C02", "#005778", null },
                    { 11, "NE", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ne.png", "New England", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "134920", "https://a.espncdn.com/i/teamlogos/nfl/500/ne.png", "Patriots", "#002244", "#C60C30", "#B0B7BC", null },
                    { 12, "NYJ", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/nyj.png", "New York", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "134921", "https://a.espncdn.com/i/teamlogos/nfl/500/nyj.png", "Jets", "#125740", "#000000", "#FFFFFF", null },
                    { 13, "DEN", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/den.png", "Denver", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "134930", "https://a.espncdn.com/i/teamlogos/nfl/500/den.png", "Broncos", "#FB4F14", "#002244", "#FFFFFF", null },
                    { 14, "KC", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/kc.png", "Kansas City", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "134931", "https://a.espncdn.com/i/teamlogos/nfl/500/kc.png", "Chiefs", "#E31837", "#FFB81C", "#FFFFFF", null },
                    { 15, "LV", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lv.png", "Las Vegas", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "134932", "https://a.espncdn.com/i/teamlogos/nfl/500/lv.png", "Raiders", "#000000", "#A5ACAF", "#FFFFFF", null },
                    { 16, "LAC", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lac.png", "Los Angeles", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "135908", "https://a.espncdn.com/i/teamlogos/nfl/500/lac.png", "Chargers", "#0080C6", "#FFC20E", "#FFFFFF", null },
                    { 17, "CHI", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/chi.png", "Chicago", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "134938", "https://a.espncdn.com/i/teamlogos/nfl/500/chi.png", "Bears", "#0B162A", "#C83803", "#FFFFFF", null },
                    { 18, "DET", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/det.png", "Detroit", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "134939", "https://a.espncdn.com/i/teamlogos/nfl/500/det.png", "Lions", "#0076B6", "#B0B7BC", "#000000", null },
                    { 19, "GB", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/gb.png", "Green Bay", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "134940", "https://a.espncdn.com/i/teamlogos/nfl/500/gb.png", "Packers", "#203731", "#FFB612", "#FFFFFF", null },
                    { 20, "MIN", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/min.png", "Minnesota", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "134941", "https://a.espncdn.com/i/teamlogos/nfl/500/min.png", "Vikings", "#4F2683", "#FFC62F", "#FFFFFF", null },
                    { 21, "ATL", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/atl.png", "Atlanta", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "134942", "https://a.espncdn.com/i/teamlogos/nfl/500/atl.png", "Falcons", "#A71930", "#000000", "#A5ACAF", null },
                    { 22, "CAR", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/car.png", "Carolina", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "134943", "https://a.espncdn.com/i/teamlogos/nfl/500/car.png", "Panthers", "#0085CA", "#101820", "#BFC0BF", null },
                    { 23, "NO", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/no.png", "New Orleans", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "134944", "https://a.espncdn.com/i/teamlogos/nfl/500/no.png", "Saints", "#D3BC8D", "#101820", "#FFFFFF", null },
                    { 24, "TB", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/tb.png", "Tampa Bay", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "134945", "https://a.espncdn.com/i/teamlogos/nfl/500/tb.png", "Buccaneers", "#D50A0A", "#34302B", "#B1BABF", null },
                    { 25, "DAL", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/dal.png", "Dallas", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "134934", "https://a.espncdn.com/i/teamlogos/nfl/500/dal.png", "Cowboys", "#003594", "#869397", "#FFFFFF", null },
                    { 26, "NYG", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/nyg.png", "New York", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "134935", "https://a.espncdn.com/i/teamlogos/nfl/500/nyg.png", "Giants", "#0B2265", "#A71930", "#A5ACAF", null },
                    { 27, "PHI", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/phi.png", "Philadelphia", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "134936", "https://a.espncdn.com/i/teamlogos/nfl/500/phi.png", "Eagles", "#004C54", "#A5ACAF", "#ACC0C6", null },
                    { 28, "WAS", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/was.png", "Washington", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "134937", "https://a.espncdn.com/i/teamlogos/nfl/500/was.png", "Commanders", "#5A1414", "#FFB612", "#FFFFFF", null },
                    { 29, "ARI", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/ari.png", "Arizona", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "134946", "https://a.espncdn.com/i/teamlogos/nfl/500/ari.png", "Cardinals", "#97233F", "#000000", "#FFB612", null },
                    { 30, "LAR", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/lar.png", "Los Angeles", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "135907", "https://a.espncdn.com/i/teamlogos/nfl/500/lar.png", "Rams", "#003594", "#FFA300", "#FFFFFF", null },
                    { 31, "SF", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/sf.png", "San Francisco", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "134948", "https://a.espncdn.com/i/teamlogos/nfl/500/sf.png", "49ers", "#AA0000", "#B3995D", "#FFFFFF", null },
                    { 32, "SEA", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/sea.png", "Seattle", 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "134949", "https://a.espncdn.com/i/teamlogos/nfl/500/sea.png", "Seahawks", "#002244", "#69BE28", "#A5ACAF", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 32);
        }
    }
}
