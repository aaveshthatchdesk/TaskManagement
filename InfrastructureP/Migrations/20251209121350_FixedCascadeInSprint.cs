using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedCascadeInSprint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sprints_projects_ProjectId",
                table: "sprints");

            migrationBuilder.AddForeignKey(
                name: "FK_sprints_projects_ProjectId",
                table: "sprints",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sprints_projects_ProjectId",
                table: "sprints");

            migrationBuilder.AddForeignKey(
                name: "FK_sprints_projects_ProjectId",
                table: "sprints",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "Id");
        }
    }
}
