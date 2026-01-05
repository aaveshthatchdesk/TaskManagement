using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetUserIdColumnInActivityLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetUserId",
                table: "ActivityLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_TargetUserId",
                table: "ActivityLogs",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_appUsers_TargetUserId",
                table: "ActivityLogs",
                column: "TargetUserId",
                principalTable: "appUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_appUsers_TargetUserId",
                table: "ActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_TargetUserId",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "TargetUserId",
                table: "ActivityLogs");
        }
    }
}
