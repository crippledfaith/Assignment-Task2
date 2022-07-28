using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task2.Infrastructure.Migrations
{
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "public",
                table: "Servers",
                keyColumn: "Id",
                keyValue: "62CD1744-DD07-4BB4-8EE0-FC1359E2C281",
                columns: new[] { "Password", "Url", "UserName" },
                values: new object[] { "password", "test.rebex.net", "demo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "public",
                table: "Servers",
                keyColumn: "Id",
                keyValue: "62CD1744-DD07-4BB4-8EE0-FC1359E2C281",
                columns: new[] { "Password", "Url", "UserName" },
                values: new object[] { "Test123", "192.168.50.11", "TestFtp" });
        }
    }
}
