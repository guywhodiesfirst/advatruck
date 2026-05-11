using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoadsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loads_Dispatchers_DispatcherId",
                table: "Loads");

            migrationBuilder.DropForeignKey(
                name: "FK_Loads_Drivers_DriverId",
                table: "Loads");

            migrationBuilder.DropTable(
                name: "LoadLocations");

            migrationBuilder.CreateTable(
                name: "LoadStops",
                columns: table => new
                {
                    LoadId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LoadLocationType = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    Location_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Location_Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadStops", x => new { x.LoadId, x.Id });
                    table.ForeignKey(
                        name: "FK_LoadStops_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Loads_Dispatchers_DispatcherId",
                table: "Loads",
                column: "DispatcherId",
                principalTable: "Dispatchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loads_Drivers_DriverId",
                table: "Loads",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loads_Dispatchers_DispatcherId",
                table: "Loads");

            migrationBuilder.DropForeignKey(
                name: "FK_Loads_Drivers_DriverId",
                table: "Loads");

            migrationBuilder.DropTable(
                name: "LoadStops");

            migrationBuilder.CreateTable(
                name: "LoadLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    LoadId = table.Column<Guid>(type: "uuid", nullable: true),
                    LoadLocationType = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location_Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Location_Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadLocations_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoadLocations_LoadId",
                table: "LoadLocations",
                column: "LoadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loads_Dispatchers_DispatcherId",
                table: "Loads",
                column: "DispatcherId",
                principalTable: "Dispatchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loads_Drivers_DriverId",
                table: "Loads",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
