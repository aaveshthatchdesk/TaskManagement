using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSprintToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "sprints",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_sprints_ProjectId",
                table: "sprints",
                column: "ProjectId",
                unique: true,
                filter: "[ProjectId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_sprints_projects_ProjectId",
                table: "sprints",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sprints_projects_ProjectId",
                table: "sprints");

            migrationBuilder.DropIndex(
                name: "IX_sprints_ProjectId",
                table: "sprints");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "sprints");

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
    }
}
