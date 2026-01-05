using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskItemNavToActivityLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_TaskItemId",
                table: "ActivityLogs",
                column: "TaskItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_tasks_TaskItemId",
                table: "ActivityLogs",
                column: "TaskItemId",
                principalTable: "tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_tasks_TaskItemId",
                table: "ActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_TaskItemId",
                table: "ActivityLogs");
        }
    }
}
