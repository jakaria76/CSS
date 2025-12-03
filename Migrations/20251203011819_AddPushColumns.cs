using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS.Migrations
{
    /// <inheritdoc />
    public partial class AddPushColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FcmToken",
                table: "AspNetUsers",
                newName: "PushP256dh");

            migrationBuilder.AddColumn<string>(
                name: "PushAuth",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PushEndpoint",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PushAuth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PushEndpoint",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "PushP256dh",
                table: "AspNetUsers",
                newName: "FcmToken");
        }
    }
}
