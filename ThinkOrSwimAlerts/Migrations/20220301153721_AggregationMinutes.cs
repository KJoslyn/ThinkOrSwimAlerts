using Microsoft.EntityFrameworkCore.Migrations;

namespace ThinkOrSwimAlerts.Migrations
{
    public partial class AggregationMinutes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AggregationMinutes",
                table: "Positions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AggregationMinutes",
                table: "Positions");
        }
    }
}
