using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Infrastructure.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddCompositeUniqueConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Checklist_Name",
                table: "Checklist");

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_Name_UserId",
                table: "Checklist",
                columns: new[] { "Name", "UserId" },
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Checklist_Name_UserId",
                table: "Checklist");

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_Name",
                table: "Checklist",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
