using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOurStoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "OurStories");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "OurStories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "OurStories");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "OurStories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
