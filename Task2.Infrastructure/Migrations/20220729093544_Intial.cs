using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task2.Infrastructure.Migrations
{
    public partial class Intial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Files",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ServerType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ServerType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Servers",
                columns: new[] { "Id", "Name", "Password", "Port", "ServerType", "Url", "UserName" },
                values: new object[,]
                {
                    { "62CD1744-DD07-4BB4-8EE0-FC1359E2C278", "LocalFile1Server", "", 0, 0, "D:\\testFTP", "" },
                    { "62CD1744-DD07-4BB4-8EE0-FC1359E2C279", "LocalFile2Server", "", 0, 0, "D:\\output", "" },
                    { "62CD1744-DD07-4BB4-8EE0-FC1359E2C280", "FTPFileServer", "Test123", 21, 1, "192.168.50.11", "TestFtp" },
                    { "62CD1744-DD07-4BB4-8EE0-FC1359E2C281", "SFTPFileServer", "password", 22, 2, "test.rebex.net", "demo" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Servers",
                schema: "public");
        }
    }
}
