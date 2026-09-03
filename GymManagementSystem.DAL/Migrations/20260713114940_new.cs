using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_MemberId_SessionId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "HealthRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MemberId_SessionId",
                table: "Bookings",
                columns: new[] { "MemberId", "SessionId" },
                unique: true,
                filter: "[IsDeleted]=0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_MemberId_SessionId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "HealthRecords");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MemberId_SessionId",
                table: "Bookings",
                columns: new[] { "MemberId", "SessionId" },
                unique: true);
        }
    }
}
