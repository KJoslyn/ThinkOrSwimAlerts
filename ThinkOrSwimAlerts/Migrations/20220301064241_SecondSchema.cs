using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThinkOrSwimAlerts.Migrations
{
    public partial class SecondSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PurhaseId",
                table: "Purchases",
                newName: "PurchaseId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "FinalSell",
                table: "Positions",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddColumn<float>(
                name: "HighPrice",
                table: "Positions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "LowPrice",
                table: "Positions",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighPrice",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "LowPrice",
                table: "Positions");

            migrationBuilder.RenameColumn(
                name: "PurchaseId",
                table: "Purchases",
                newName: "PurhaseId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "FinalSell",
                table: "Positions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);
        }
    }
}
