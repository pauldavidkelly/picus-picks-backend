using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Picus.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Abbreviation", "BannerUrl", "City", "Conference", "CreatedAt", "Division", "ExternalTeamId", "IconUrl", "Name", "PrimaryColor", "SecondaryColor", "TertiaryColor", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "BAL", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/bal.png", "Baltimore", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "134922", "https://a.espncdn.com/i/teamlogos/nfl/500/bal.png", "Ravens", "#241773", "#000000", "#9E7C0C", null },
                    { 2, "CIN", "https://a.espncdn.com/i/teamlogos/nfl/500-dark/cin.png", "Cincinnati", 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "134923", "https://a.espncdn.com/i/teamlogos/nfl/500/cin.png", "Bengals", "#FB4F14", "#000000", "#FFFFFF", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
