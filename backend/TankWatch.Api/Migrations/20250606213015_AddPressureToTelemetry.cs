using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankWatch.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPressureToTelemetry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Pressure",
                table: "Telemetry",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pressure",
                table: "Telemetry");
        }
    }
}
