using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Drivers_DriverId",
                table: "Bids");

            migrationBuilder.RenameColumn(
                name: "DriverId",
                table: "Bids",
                newName: "DriverCreatedId");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_DriverId",
                table: "Bids",
                newName: "IX_Bids_DriverCreatedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Drivers_DriverCreatedId",
                table: "Bids",
                column: "DriverCreatedId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Drivers_DriverCreatedId",
                table: "Bids");

            migrationBuilder.RenameColumn(
                name: "DriverCreatedId",
                table: "Bids",
                newName: "DriverId");

            migrationBuilder.RenameIndex(
                name: "IX_Bids_DriverCreatedId",
                table: "Bids",
                newName: "IX_Bids_DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Drivers_DriverId",
                table: "Bids",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
