using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamAI.Migrations
{
    /// <inheritdoc />
    public partial class userId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_dreams_UserId",
                table: "dreams",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_dreams_AspNetUsers_UserId",
                table: "dreams",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dreams_AspNetUsers_UserId",
                table: "dreams");

            migrationBuilder.DropIndex(
                name: "IX_dreams_UserId",
                table: "dreams");
        }
    }
}
