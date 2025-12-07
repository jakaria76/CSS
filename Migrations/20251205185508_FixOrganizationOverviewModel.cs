using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS.Migrations
{
    /// <inheritdoc />
    public partial class FixOrganizationOverviewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "purpose",
                table: "OrganizationOverviews",
                newName: "Purpose");

            migrationBuilder.RenameColumn(
                name: "FounderYear",
                table: "OrganizationOverviews",
                newName: "FoundedYear");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Purpose",
                table: "OrganizationOverviews",
                newName: "purpose");

            migrationBuilder.RenameColumn(
                name: "FoundedYear",
                table: "OrganizationOverviews",
                newName: "FounderYear");
        }
    }
}
