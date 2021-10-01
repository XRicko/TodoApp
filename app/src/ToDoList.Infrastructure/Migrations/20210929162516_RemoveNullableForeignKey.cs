using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Infrastructure.Migrations
{
    public partial class RemoveNullableForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Checklist_Name_ProjectId",
                table: "Checklist");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Checklist",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_Name_ProjectId",
                table: "Checklist",
                columns: new[] { "Name", "ProjectId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Checklist_Name_ProjectId",
                table: "Checklist");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Checklist",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_Name_ProjectId",
                table: "Checklist",
                columns: new[] { "Name", "ProjectId" },
                unique: true,
                filter: "[Name] IS NOT NULL AND [ProjectId] IS NOT NULL");
        }
    }
}
