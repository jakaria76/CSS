using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS.Migrations   // <-- যদি অন্য namespace থাকে, শুধু এই লাইনটা adjust করো
{
    public partial class FixPreviousPresidentColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ LegacyNote column rename (safe – conditionally)
            migrationBuilder.Sql(@"
IF COL_LENGTH('PreviousPresidents', 'LegancyNote') IS NOT NULL
    EXEC sp_rename 'PreviousPresidents.LegancyNote', 'LegacyNote', 'COLUMN';
");

            // 2️⃣ নতুন temp datetime2 column add
            migrationBuilder.AddColumn<DateTime>(
                name: "TenureStartTemp",
                table: "PreviousPresidents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TenureEndTemp",
                table: "PreviousPresidents",
                type: "datetime2",
                nullable: true);

            // 3️⃣ int → datetime2 conversion (YEAR → date)
            migrationBuilder.Sql(@"
UPDATE PreviousPresidents
SET TenureStartTemp = TRY_CONVERT(datetime2, CONCAT(TenureStart, '-01-01')),
    TenureEndTemp   = TRY_CONVERT(datetime2, CONCAT(TenureEnd,   '-12-31'));
");

            // 4️⃣ পুরোনো int কলাম drop
            migrationBuilder.DropColumn(
                name: "TenureStart",
                table: "PreviousPresidents");

            migrationBuilder.DropColumn(
                name: "TenureEnd",
                table: "PreviousPresidents");

            // 5️⃣ Temp কলাম rename করে main নাম দেয়া
            migrationBuilder.RenameColumn(
                name: "TenureStartTemp",
                table: "PreviousPresidents",
                newName: "TenureStart");

            migrationBuilder.RenameColumn(
                name: "TenureEndTemp",
                table: "PreviousPresidents",
                newName: "TenureEnd");

            // 6️⃣ NOT NULL করে দেয়া (model অনুযায়ী)
            migrationBuilder.AlterColumn<DateTime>(
                name: "TenureStart",
                table: "PreviousPresidents",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TenureEnd",
                table: "PreviousPresidents",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 🔁 যদি কখনো rollback করতে চাও, পুরোনো int এ ফিরে যাওয়ার logic

            // 1️⃣ নতুন int কলাম যোগ
            migrationBuilder.AddColumn<int>(
                name: "TenureStartInt",
                table: "PreviousPresidents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenureEndInt",
                table: "PreviousPresidents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 2️⃣ date থেকে year এ convert
            migrationBuilder.Sql(@"
UPDATE PreviousPresidents
SET TenureStartInt = YEAR(TenureStart),
    TenureEndInt   = YEAR(TenureEnd);
");

            // 3️⃣ datetime কলাম drop
            migrationBuilder.DropColumn(
                name: "TenureStart",
                table: "PreviousPresidents");

            migrationBuilder.DropColumn(
                name: "TenureEnd",
                table: "PreviousPresidents");

            // 4️⃣ int কলামকে পুরোনো নাম দেয়া
            migrationBuilder.RenameColumn(
                name: "TenureStartInt",
                table: "PreviousPresidents",
                newName: "TenureStart");

            migrationBuilder.RenameColumn(
                name: "TenureEndInt",
                table: "PreviousPresidents",
                newName: "TenureEnd");

            // 5️⃣ LegacyNote আবার LegancyNote এ ফিরিয়ে দেয়া
            migrationBuilder.Sql(@"
IF COL_LENGTH('PreviousPresidents', 'LegacyNote') IS NOT NULL
    EXEC sp_rename 'PreviousPresidents.LegacyNote', 'LegancyNote', 'COLUMN';
");
        }
    }
}
