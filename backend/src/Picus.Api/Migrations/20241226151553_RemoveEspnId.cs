using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picus.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEspnId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ESPNTeamId",
                table: "Teams",
                newName: "ExternalTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalTeamId",
                table: "Teams",
                newName: "ESPNTeamId");
        }
    }
}
