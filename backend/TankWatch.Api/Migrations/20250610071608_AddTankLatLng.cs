using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankWatch.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTankLatLng : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Tanks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Tanks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Tanks");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Tanks");
        }
    }
}
