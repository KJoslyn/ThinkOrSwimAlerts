using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ThinkOrSwimAlerts.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<long>(type: "bigint", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Underlying = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PutOrCall = table.Column<int>(type: "int", nullable: false),
                    FirstBuy = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FinalSell = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Indicator = table.Column<int>(type: "int", nullable: false),
                    IndicatorVersion = table.Column<short>(type: "smallint", nullable: false),
                    HighPrice = table.Column<float>(type: "real", nullable: false),
                    LowPrice = table.Column<float>(type: "real", nullable: false),
                    MaxQuantity = table.Column<short>(type: "smallint", nullable: false),
                    CurrentQuantity = table.Column<short>(type: "smallint", nullable: false),
                    GainOrLoss = table.Column<float>(type: "real", nullable: false),
                    AvgBuyPrice = table.Column<float>(type: "real", nullable: false)
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
                    PositionId = table.Column<long>(type: "bigint", nullable: false),
                    SecondsAfterFirstBuy = table.Column<int>(type: "int", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    PurchaseId = table.Column<long>(type: "bigint", nullable: false),
                    PositionId = table.Column<long>(type: "bigint", nullable: false),
                    BuyPrice = table.Column<float>(type: "real", nullable: false),
                    SecondsAfterFirstBuy = table.Column<int>(type: "int", nullable: false),
                    Bought15MinuteInterval = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK_Purchases_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
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
