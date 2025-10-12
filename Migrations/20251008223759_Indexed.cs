using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamAI.Migrations
{
    /// <inheritdoc />
    public partial class Indexed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "dreamSymbols",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_dreamSymbols_Name",
                table: "dreamSymbols",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dreamSymbols_Name",
                table: "dreamSymbols");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "dreamSymbols",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
