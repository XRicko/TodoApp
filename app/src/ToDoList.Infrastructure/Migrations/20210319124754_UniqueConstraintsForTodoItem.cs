
using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Infrastructure.Migrations
{
    public partial class UniqueConstraintsForTodoItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Task_Name_ChecklistId",
                table: "Task",
                columns: new[] { "Name", "ChecklistId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Task_Name_ChecklistId",
                table: "Task");
        }
    }
}
