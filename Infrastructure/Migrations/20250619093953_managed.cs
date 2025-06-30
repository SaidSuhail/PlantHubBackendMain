using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class managed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingItems_Plants_PlantId1",
                table: "BookingItems");

            migrationBuilder.DropIndex(
                name: "IX_BookingItems_PlantId1",
                table: "BookingItems");

            migrationBuilder.DropColumn(
                name: "PlantId1",
                table: "BookingItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlantId1",
                table: "BookingItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_PlantId1",
                table: "BookingItems",
                column: "PlantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingItems_Plants_PlantId1",
                table: "BookingItems",
                column: "PlantId1",
                principalTable: "Plants",
                principalColumn: "Id");
        }
    }
}
