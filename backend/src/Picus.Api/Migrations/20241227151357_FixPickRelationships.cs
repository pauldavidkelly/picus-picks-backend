using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picus.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixPickRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Games_GameId",
                table: "Picks");

            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Games_GameId1",
                table: "Picks");

            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Teams_SelectedTeamId",
                table: "Picks");

            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Teams_TeamId",
                table: "Picks");

            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Users_UserId",
                table: "Picks");

            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Users_UserId1",
                table: "Picks");

            migrationBuilder.DropIndex(
                name: "IX_Picks_GameId1",
                table: "Picks");

            migrationBuilder.DropIndex(
                name: "IX_Picks_TeamId",
                table: "Picks");

            migrationBuilder.DropIndex(
                name: "IX_Picks_UserId1",
                table: "Picks");

            migrationBuilder.DropColumn(
                name: "GameId1",
                table: "Picks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Picks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Picks");

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Games_GameId",
                table: "Picks",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Teams_SelectedTeamId",
                table: "Picks",
                column: "SelectedTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Users_UserId",
                table: "Picks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Games_GameId",
                table: "Picks");

            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Teams_SelectedTeamId",
                table: "Picks");

            migrationBuilder.DropForeignKey(
                name: "FK_Picks_Users_UserId",
                table: "Picks");

            migrationBuilder.AddColumn<int>(
                name: "GameId1",
                table: "Picks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Picks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "Picks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Picks_GameId1",
                table: "Picks",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_Picks_TeamId",
                table: "Picks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Picks_UserId1",
                table: "Picks",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Games_GameId",
                table: "Picks",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Games_GameId1",
                table: "Picks",
                column: "GameId1",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Teams_SelectedTeamId",
                table: "Picks",
                column: "SelectedTeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Teams_TeamId",
                table: "Picks",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Users_UserId",
                table: "Picks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Picks_Users_UserId1",
                table: "Picks",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
