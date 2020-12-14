using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentManagement_DataBase.Migrations
{
    public partial class SeedStudentData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "ID", "ClassName", "Email", "Name" },
                values: new object[] { 1, 4, "MoshangPengyou@hotmail.com", "朱超" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "ID",
                keyValue: 1);
        }
    }
}
