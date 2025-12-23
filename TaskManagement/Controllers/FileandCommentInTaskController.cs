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
        [Authorize]
        [HttpPost("{taskId}/comments")]
        public async Task<IActionResult> AddComment(int taskId, [FromBody] TaskCommentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                 ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int CreatedByUserId = int.Parse(userIdClaim.Value);

            var comment = await _fileAndCommentsInTasks.AddCommentAsync(taskId, CreatedByUserId, dto);
            return Ok(comment);
        }
       
        [Authorize]
        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(
            int commentId,
            [FromBody] TaskCommentDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                   ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int userId = int.Parse(userIdClaim.Value);

            bool isAdmin = User.IsInRole("Admin");

            var updatedComment = await _fileAndCommentsInTasks
                .UpdateCommentAsync(commentId, userId, isAdmin, dto.CommentText);

            if (updatedComment == null)
                return NotFound("Comment not found");

            return Ok(updatedComment);
        }

      
        [Authorize]
        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                    ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int userId = int.Parse(userIdClaim.Value);

            bool isAdmin = User.IsInRole("Admin");

            var result = await _fileAndCommentsInTasks
                .DeleteCommentAsync(commentId, userId, isAdmin);

            if (!result)
                return NotFound("Comment not found");

            return Ok("Comment deleted successfully");
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

        [Authorize]
        [HttpDelete("attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(int attachmentId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            bool isAdmin = User.IsInRole("Admin");

            var result = await _fileAndCommentsInTasks.DeleteAttachmentAsync(attachmentId, userId,isAdmin);

            if (!result)
                return Forbid();
            return Ok("Attachment deleted successfully");
        }


    }
}
