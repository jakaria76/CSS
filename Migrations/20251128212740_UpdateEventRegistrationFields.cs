using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventRegistrationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "EventRegistrations",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "Department",
                table: "EventRegistrations",
                newName: "WhyJoin");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDateTime",
                table: "Events",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "EventRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EventRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "EventRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstituteName",
                table: "EventRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "EventRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "UserImage",
                table: "EventRegistrations",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserImageType",
                table: "EventRegistrations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "InstituteName",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "UserImage",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "UserImageType",
                table: "EventRegistrations");

            migrationBuilder.RenameColumn(
                name: "WhyJoin",
                table: "EventRegistrations",
                newName: "Department");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "EventRegistrations",
                newName: "Name");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDateTime",
                table: "Events",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
