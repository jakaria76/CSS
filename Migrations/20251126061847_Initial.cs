using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CGPA",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EventParticipationCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VolunteerHours",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "AspNetUsers",
                newName: "NextAvailableDonationDate");

            migrationBuilder.RenameColumn(
                name: "Skills",
                table: "AspNetUsers",
                newName: "WhyJoined");

            migrationBuilder.RenameColumn(
                name: "Session",
                table: "AspNetUsers",
                newName: "WhatsAppNumber");

            migrationBuilder.RenameColumn(
                name: "ProfileImage",
                table: "AspNetUsers",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "MemberPositionDetails",
                table: "AspNetUsers",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "MemberPosition",
                table: "AspNetUsers",
                newName: "ShortBio");

            migrationBuilder.RenameColumn(
                name: "ExtraCertificates",
                table: "AspNetUsers",
                newName: "ProfileImagePath");

            migrationBuilder.RenameColumn(
                name: "EmergencyContactNumber",
                table: "AspNetUsers",
                newName: "PreferredDonationLocation");

            migrationBuilder.RenameColumn(
                name: "EmergencyContactName",
                table: "AspNetUsers",
                newName: "PortfolioWebsite");

            migrationBuilder.RenameColumn(
                name: "Degree",
                table: "AspNetUsers",
                newName: "MemberType");

            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlternativeMobile",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommitteePosition",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DonationEligibility",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FutureGoals",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hobbies",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MemberSince",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AlternativeMobile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CommitteePosition",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DonationEligibility",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FutureGoals",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Hobbies",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MemberSince",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "WhyJoined",
                table: "AspNetUsers",
                newName: "Skills");

            migrationBuilder.RenameColumn(
                name: "WhatsAppNumber",
                table: "AspNetUsers",
                newName: "Session");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "AspNetUsers",
                newName: "ProfileImage");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "AspNetUsers",
                newName: "MemberPositionDetails");

            migrationBuilder.RenameColumn(
                name: "ShortBio",
                table: "AspNetUsers",
                newName: "MemberPosition");

            migrationBuilder.RenameColumn(
                name: "ProfileImagePath",
                table: "AspNetUsers",
                newName: "ExtraCertificates");

            migrationBuilder.RenameColumn(
                name: "PreferredDonationLocation",
                table: "AspNetUsers",
                newName: "EmergencyContactNumber");

            migrationBuilder.RenameColumn(
                name: "PortfolioWebsite",
                table: "AspNetUsers",
                newName: "EmergencyContactName");

            migrationBuilder.RenameColumn(
                name: "NextAvailableDonationDate",
                table: "AspNetUsers",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "MemberType",
                table: "AspNetUsers",
                newName: "Degree");

            migrationBuilder.AddColumn<double>(
                name: "CGPA",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventParticipationCount",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VolunteerHours",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
