using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picus.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubmissionTimeToPick : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionTime",
                table: "Picks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: DateTime.UtcNow);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionTime",
                table: "Picks");
        }
    }
}
