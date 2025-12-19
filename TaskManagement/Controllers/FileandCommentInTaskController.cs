using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task.Application.DTOs;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileandCommentInTaskController : ControllerBase
    {
        private readonly IFileAndCommentsInTasksService _fileAndCommentsInTasks;

        public FileandCommentInTaskController(IFileAndCommentsInTasksService fileAndCommentsInTasks)
        {
            _fileAndCommentsInTasks = fileAndCommentsInTasks;
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskDetailById(int taskId)
        {
           var task =await _fileAndCommentsInTasks.GetTaskDetailByIdAsync(taskId);
            if (task == null)
                return NotFound();
            return Ok(task);
        }
        [HttpPost("{taskId}/comments")]
        public async Task<IActionResult> AddComment(int taskId, [FromBody] TaskCommentDto dto)
        {    
           
            var comment = await _fileAndCommentsInTasks.AddCommentAsync(taskId, dto.CreatedByUserId, dto);
            return Ok(comment);
        }
        [Authorize]
        [HttpPost("{taskId}/attachments")]
        [Consumes("multipart/form-data")]
        

        public async Task<IActionResult> AddAttachment(int taskId, [FromForm] TaskAttachmentUploadDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                   ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int userId = int.Parse(userIdClaim.Value);
            var attachment = await _fileAndCommentsInTasks.AddAttachmentAsync(taskId, userId, dto.File);
            return Ok(attachment);
        }

    }
}
