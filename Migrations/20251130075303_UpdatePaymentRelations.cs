using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_EventRegistrations_RegistrationId",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "PayerMobile",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PayerFullName",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PayerEmail",
                table: "PaymentTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_EventRegistrations_RegistrationId",
                table: "PaymentTransactions",
                column: "RegistrationId",
                principalTable: "EventRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_EventRegistrations_RegistrationId",
                table: "PaymentTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "PayerMobile",
                table: "PaymentTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PayerFullName",
                table: "PaymentTransactions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PayerEmail",
                table: "PaymentTransactions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_EventRegistrations_RegistrationId",
                table: "PaymentTransactions",
                column: "RegistrationId",
                principalTable: "EventRegistrations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
