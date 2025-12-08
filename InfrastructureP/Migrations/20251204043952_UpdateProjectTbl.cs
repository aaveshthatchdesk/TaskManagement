using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SprintId",
                table: "projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_projects_SprintId",
                table: "projects",
                column: "SprintId");

            migrationBuilder.AddForeignKey(
                name: "FK_projects_sprints_SprintId",
                table: "projects",
                column: "SprintId",
                principalTable: "sprints",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_projects_sprints_SprintId",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "IX_projects_SprintId",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "SprintId",
                table: "projects");
        }
    }
}
