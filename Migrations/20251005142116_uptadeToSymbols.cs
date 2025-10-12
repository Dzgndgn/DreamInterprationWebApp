using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamAI.Migrations
{
    /// <inheritdoc />
    public partial class uptadeToSymbols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Culture",
                table: "dreamSymbols");

            migrationBuilder.RenameColumn(
                name: "Desc",
                table: "dreamSymbols",
                newName: "meaning");

            migrationBuilder.AddColumn<string>(
                name: "Context",
                table: "dreamSymbols",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Context",
                table: "dreamSymbols");

            migrationBuilder.RenameColumn(
                name: "meaning",
                table: "dreamSymbols",
                newName: "Desc");

            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "dreamSymbols",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
