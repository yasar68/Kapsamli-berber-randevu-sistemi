using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BerberApp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReportDetailedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CompletedRevenue",
                table: "Reports",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ConfirmedAppointments",
                table: "Reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfirmedRevenue",
                table: "Reports",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PendingAppointments",
                table: "Reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedRevenue",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ConfirmedAppointments",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ConfirmedRevenue",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "PendingAppointments",
                table: "Reports");
        }
    }
}
