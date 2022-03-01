﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThinkOrSwimAlerts.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Underlying = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PutOrCall = table.Column<int>(type: "int", nullable: false),
                    FirstBuy = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FinalSell = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Indicator = table.Column<int>(type: "int", nullable: false),
                    IndicatorVersion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                });

            migrationBuilder.CreateTable(
                name: "PositionUpdates",
                columns: table => new
                {
                    PositionUpdateId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<long>(type: "bigint", nullable: true),
                    SecondsAfterPurchase = table.Column<int>(type: "int", nullable: false),
                    Mark = table.Column<float>(type: "real", nullable: false),
                    GainOrLossPct = table.Column<float>(type: "real", nullable: false),
                    IsNewHigh = table.Column<bool>(type: "bit", nullable: false),
                    IsNewLow = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionUpdates", x => x.PositionUpdateId);
                    table.ForeignKey(
                        name: "FK_PositionUpdates_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    PurhaseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionId = table.Column<long>(type: "bigint", nullable: true),
                    BuyPrice = table.Column<float>(type: "real", nullable: false),
                    Bought = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Bought15MinuteInterval = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.PurhaseId);
                    table.ForeignKey(
                        name: "FK_Purchases_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionUpdates_PositionId",
                table: "PositionUpdates",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PositionId",
                table: "Purchases",
                column: "PositionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionUpdates");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Positions");
        }
    }
}
