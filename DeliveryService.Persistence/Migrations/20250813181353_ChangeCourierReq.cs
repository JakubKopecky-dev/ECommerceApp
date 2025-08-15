using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCourierReq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Couriers_CourierId",
                table: "Deliveries");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourierId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Couriers_CourierId",
                table: "Deliveries",
                column: "CourierId",
                principalTable: "Couriers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Couriers_CourierId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourierId",
                table: "Deliveries",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Couriers_CourierId",
                table: "Deliveries",
                column: "CourierId",
                principalTable: "Couriers",
                principalColumn: "Id");
        }
    }
}
