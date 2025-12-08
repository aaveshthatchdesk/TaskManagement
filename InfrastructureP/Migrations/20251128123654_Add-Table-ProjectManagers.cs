using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableProjectManagers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");


            migrationBuilder.AddColumn<bool>(
                name: "IsTemporaryPassword",
                table: "appUserAuths",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProjectManagers",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectManagers", x => new { x.ProjectId, x.AppUserId });
                    table.ForeignKey(
                        name: "FK_ProjectManagers_appUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectManagers_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMembers",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    AppUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembers", x => new { x.ProjectId, x.AppUserId });
                    table.ForeignKey(
                        name: "FK_ProjectMembers_appUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "appUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectMembers_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectManagers_AppUserId",
                table: "ProjectManagers",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMembers_AppUserId",
                table: "ProjectMembers",
                column: "AppUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectManagers");

            migrationBuilder.DropTable(
                name: "ProjectMembers");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "IsTemporaryPassword",
                table: "appUserAuths");
        }
    }
}
