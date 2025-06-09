using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TankWatch.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTankEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TankId",
                table: "Telemetry",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tanks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Telemetry_TankId",
                table: "Telemetry",
                column: "TankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Telemetry_Tanks_TankId",
                table: "Telemetry",
                column: "TankId",
                principalTable: "Tanks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Telemetry_Tanks_TankId",
                table: "Telemetry");

            migrationBuilder.DropTable(
                name: "Tanks");

            migrationBuilder.DropIndex(
                name: "IX_Telemetry_TankId",
                table: "Telemetry");

            migrationBuilder.DropColumn(
                name: "TankId",
                table: "Telemetry");
        }
    }
}
