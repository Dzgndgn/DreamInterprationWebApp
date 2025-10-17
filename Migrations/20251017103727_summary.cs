using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DreamAI.Migrations
{
    /// <inheritdoc />
    public partial class summary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chatSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    sumContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatSummaries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_messageRecords_Id",
                table: "messageRecords",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chatSummaries");

            migrationBuilder.DropIndex(
                name: "IX_messageRecords_Id",
                table: "messageRecords");
        }
    }
}
