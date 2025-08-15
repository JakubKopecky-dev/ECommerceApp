using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NoteDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Carts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
