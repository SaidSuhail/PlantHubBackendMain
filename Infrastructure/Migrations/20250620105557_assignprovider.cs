using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class assignprovider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ProviderId",
                table: "Bookings",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Providers_ProviderId",
                table: "Bookings",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Providers_ProviderId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ProviderId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Bookings");
        }
    }
}
