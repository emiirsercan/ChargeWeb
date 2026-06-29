using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChargingStations.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOcmIdAndSyncFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OcmId",
                table: "stations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_stations_ocm_id_unique",
                table: "stations",
                column: "OcmId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stations_ocm_id_unique",
                table: "stations");

            migrationBuilder.DropColumn(
                name: "OcmId",
                table: "stations");
        }
    }
}
