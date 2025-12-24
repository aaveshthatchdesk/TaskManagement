using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removecreatedonfromTaskCreatorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCreator_appUsers_AppUserId",
                table: "TaskCreator");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCreator_tasks_TaskItemId",
                table: "TaskCreator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCreator",
                table: "TaskCreator");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TaskCreator");

            migrationBuilder.RenameTable(
                name: "TaskCreator",
                newName: "TaskCreators");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCreator_TaskItemId",
                table: "TaskCreators",
                newName: "IX_TaskCreators_TaskItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCreator_AppUserId",
                table: "TaskCreators",
                newName: "IX_TaskCreators_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCreators",
                table: "TaskCreators",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCreators_appUsers_AppUserId",
                table: "TaskCreators",
                column: "AppUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCreators_tasks_TaskItemId",
                table: "TaskCreators",
                column: "TaskItemId",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCreators_appUsers_AppUserId",
                table: "TaskCreators");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCreators_tasks_TaskItemId",
                table: "TaskCreators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskCreators",
                table: "TaskCreators");

            migrationBuilder.RenameTable(
                name: "TaskCreators",
                newName: "TaskCreator");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCreators_TaskItemId",
                table: "TaskCreator",
                newName: "IX_TaskCreator_TaskItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskCreators_AppUserId",
                table: "TaskCreator",
                newName: "IX_TaskCreator_AppUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TaskCreator",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskCreator",
                table: "TaskCreator",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCreator_appUsers_AppUserId",
                table: "TaskCreator",
                column: "AppUserId",
                principalTable: "appUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCreator_tasks_TaskItemId",
                table: "TaskCreator",
                column: "TaskItemId",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
