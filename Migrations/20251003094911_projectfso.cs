using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisionManagement.Migrations
{
    /// <inheritdoc />
    public partial class projectfso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FounderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WebsiteLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileAppLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartupDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartupStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartupLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectDemoVideoLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FounderPhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpotlightReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
