using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisionManagement.Migrations
{
    /// <inheritdoc />
    public partial class UPDATEDPROJECTMODELWITHIMAGESANDVIDEO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultVideo",
                table: "Projects",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image1",
                table: "Projects",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image2",
                table: "Projects",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image3",
                table: "Projects",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PitchVideo",
                table: "Projects",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultVideo",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Image1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Image2",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Image3",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PitchVideo",
                table: "Projects");
        }
    }
}
