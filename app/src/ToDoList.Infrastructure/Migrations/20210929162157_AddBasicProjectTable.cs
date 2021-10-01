using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Infrastructure.Migrations
{
    public partial class AddBasicProjectTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checklist_User_UserId",
                table: "Checklist");

            migrationBuilder.DropIndex(
                name: "IX_Checklist_Name_UserId",
                table: "Checklist");

            migrationBuilder.DropIndex(
                name: "IX_Checklist_UserId",
                table: "Checklist");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Checklist");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Checklist",
                type: "nvarchar(75)",
                maxLength: 75,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(75)",
                oldMaxLength: 75,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Checklist",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(175)", maxLength: 175, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_Name_ProjectId",
                table: "Checklist",
                columns: new[] { "Name", "ProjectId" },
                unique: true,
                filter: "[Name] IS NOT NULL AND [ProjectId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_ProjectId",
                table: "Checklist",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Name_UserId",
                table: "Project",
                columns: new[] { "Name", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_UserId",
                table: "Project",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checklist_Project_ProjectId",
                table: "Checklist",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checklist_Project_ProjectId",
                table: "Checklist");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Checklist_Name_ProjectId",
                table: "Checklist");

            migrationBuilder.DropIndex(
                name: "IX_Checklist_ProjectId",
                table: "Checklist");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Checklist");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Checklist",
                type: "nvarchar(75)",
                maxLength: 75,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(75)",
                oldMaxLength: 75);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Checklist",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_Name_UserId",
                table: "Checklist",
                columns: new[] { "Name", "UserId" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Checklist_UserId",
                table: "Checklist",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checklist_User_UserId",
                table: "Checklist",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
