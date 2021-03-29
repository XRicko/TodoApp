using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Infrastructure.Migrations
{
    public partial class DefaultData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "IsDone", "Name" },
                values: new object[] { 1, false, "Planned" });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "IsDone", "Name" },
                values: new object[] { 2, false, "Ongoing" });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "IsDone", "Name" },
                values: new object[] { 3, true, "Done" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
