using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Infrastructure.Migrations
{
    public partial class RenameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToknen_User_UserId",
                table: "RefreshToknen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToknen",
                table: "RefreshToknen");

            migrationBuilder.RenameTable(
                name: "RefreshToknen",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToknen_UserId",
                table: "RefreshToken",
                newName: "IX_RefreshToken_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToken_User_UserId",
                table: "RefreshToken",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshToken_User_UserId",
                table: "RefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "RefreshToknen");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToknen",
                newName: "IX_RefreshToknen_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToknen",
                table: "RefreshToknen",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshToknen_User_UserId",
                table: "RefreshToknen",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
