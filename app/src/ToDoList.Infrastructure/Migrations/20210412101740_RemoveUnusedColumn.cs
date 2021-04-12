using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Infrastructure.Migrations
{
    public partial class RemoveUnusedColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Task_Task_ParentId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_ParentId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Task");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Task",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Task_ParentId",
                table: "Task",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Task_ParentId",
                table: "Task",
                column: "ParentId",
                principalTable: "Task",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
