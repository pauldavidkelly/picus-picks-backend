using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picus.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesToPick : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Picks",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Picks");
        }
    }
}
