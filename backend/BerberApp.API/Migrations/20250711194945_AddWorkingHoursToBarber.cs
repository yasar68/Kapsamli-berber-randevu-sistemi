using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BerberApp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingHoursToBarber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarberId",
                table: "WorkingHours",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHours_BarberId",
                table: "WorkingHours",
                column: "BarberId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkingHours_Barbers_BarberId",
                table: "WorkingHours",
                column: "BarberId",
                principalTable: "Barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkingHours_Barbers_BarberId",
                table: "WorkingHours");

            migrationBuilder.DropIndex(
                name: "IX_WorkingHours_BarberId",
                table: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "BarberId",
                table: "WorkingHours");
        }
    }
}
