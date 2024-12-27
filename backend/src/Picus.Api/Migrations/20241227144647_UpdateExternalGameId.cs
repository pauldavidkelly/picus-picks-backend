using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picus.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExternalGameId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ESPNGameId",
                table: "Games",
                newName: "ExternalGameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalGameId",
                table: "Games",
                newName: "ESPNGameId");
        }
    }
}
