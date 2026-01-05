using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Claims;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Application.Services;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskItemService _taskItemService;

        public TaskItemController(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }
        [Authorize]
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(int taskId, [FromBody] TaskItemDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int updatedByUserId = int.Parse(userIdClaim.Value);

            var success = await _taskItemService.UpdateTaskAsync(taskId, dto,updatedByUserId);

            if (!success)
                return NotFound("Task not found");

            return Ok("Task updated successfully");
        }
        [HttpPut("/description/{taskId}")]
        public async Task<IActionResult> UpdateTaskDescription(int taskId, [FromBody] string description)
        {


            var success = await _taskItemService.UpdateTaskDescriptionAsync(taskId, description);

            if (!success)
                return NotFound("Task not found");

            return Ok("Description updated successfully");
        }

        [HttpGet("project/{projectId}/tasks")]
        public async Task<IActionResult> GetTasks(int projectId)
        {
            var tasks = await _taskItemService.GetTasksByProjectAsync(projectId);
            return Ok(tasks);
        }
        [HttpGet("tasks/{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            var task = await _taskItemService.GetTaskByIdAsync(taskId);
            if (task == null)
                return NotFound("Task not found");
            return Ok(task);
        }
        [HttpGet("member/{memberId}/grouped")]
        public async Task<IActionResult> GetMemberGroupedBoards(int memberId)
        {
            var result = await _taskItemService.GetMemberBoardsAsync(memberId);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("boards/{boardId}/tasks")]
        public async Task<IActionResult> CreateTask(int boardId, [FromBody] TaskItemDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                 ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int createdByUserId = int.Parse(userIdClaim.Value);
            dto.BoardId = boardId;
            var createdTask = await _taskItemService.CreateTaskAsync(createdByUserId,dto);
            return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTask.Id }, createdTask);
        }
        [Authorize]
        [HttpPut("tasks/{taskId}")]
        public async Task<IActionResult> UpdateTaskItem(int taskId, [FromBody] TaskItemDto dto)
        {
          
            var updatedTask = await _taskItemService.UpdateTasksAsync(taskId, dto);
            return Ok(updatedTask);
        }
        [HttpDelete("tasks/{taskId}")]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int userId = int.Parse(userIdClaim.Value);
            var success = await _taskItemService.DeleteTaskAsync(taskId,userId);
            if (!success)
                return NotFound("Task not found");
            return Ok("Task deleted successfully");
        }
        [Authorize]
        [HttpPut("tasks/reorder")]
        public async Task<IActionResult> ReorderTasks([FromBody] List<TaskReorderDto> tasks)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
               ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int userId = int.Parse(userIdClaim.Value);
            var success = await _taskItemService.ReorderTasksAsync(tasks,userId);
            if (!success)
                return BadRequest("Failed to reorder tasks");
            return Ok("Tasks reordered successfully");

        }
        [Authorize]
        [HttpPut("tasksformembers/reorder")]
        public async Task<IActionResult> ReorderTasksForMembers([FromBody] List<TaskReorderForMembersDto> tasks)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                 ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int UserId = int.Parse(userIdClaim.Value);
            var success = await _taskItemService.ReorderTaskForMembersAsync(tasks,UserId);
            if (!success)
                return BadRequest("Failed to reorder tasks");
            return Ok("Tasks reordered successfully");

        }
    }
}







