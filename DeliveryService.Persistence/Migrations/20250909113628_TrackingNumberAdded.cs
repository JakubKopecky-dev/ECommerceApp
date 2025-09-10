using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TrackingNumberAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "Deliveries");
        }
    }
}
