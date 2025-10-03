using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisionManagement.Migrations
{
    /// <inheritdoc />
    public partial class evaluationdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    EvaluationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProblemSignificance = table.Column<int>(type: "int", nullable: false),
                    InnovationTechnical = table.Column<int>(type: "int", nullable: false),
                    MarketScalability = table.Column<int>(type: "int", nullable: false),
                    TractionImpact = table.Column<int>(type: "int", nullable: false),
                    BusinessModel = table.Column<int>(type: "int", nullable: false),
                    TeamExecution = table.Column<int>(type: "int", nullable: false),
                    EthicsEquity = table.Column<int>(type: "int", nullable: false),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weaknesses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.EvaluationId);
                    table.ForeignKey(
                        name: "FK_Evaluations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ProjectId",
                table: "Evaluations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_UserId",
                table: "Evaluations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evaluations");
        }
    }
}
