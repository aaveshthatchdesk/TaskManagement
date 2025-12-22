using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;

namespace Task.Application.Interaces
{
    public interface IFileAndCommentsInTasksService
    {

        Task<TaskItemDto?> GetTaskDetailByIdAsync(int taskId);
        Task<TaskCommentDto> AddCommentAsync(int taskId,  int userId, TaskCommentDto dto);
        Task<bool> DeleteAttachmentAsync(int attachmentId, int userId,bool isAdmin);
        Task<TaskAttachmentDto> AddAttachmentAsync(int taskId, int userId,IFormFile file);
    }
}
