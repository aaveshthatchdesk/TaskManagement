using Org.BouncyCastle.Crypto.Fpe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

using Task.Application.DTOs;
using Task.Application.Interaces;
using Microsoft.AspNetCore.Http;

namespace Task.Application.Services
{
    public  class FileAndCommentsInTasksService:IFileAndCommentsInTasksService
    {
        private readonly IFileAndCommentsInTasksRepository _tasksRepository;
        //private readonly IWebHostEnvironment _env;

        public FileAndCommentsInTasksService(IFileAndCommentsInTasksRepository tasksRepository)
        {
            _tasksRepository = tasksRepository;
            //_env = env;
        }
        public async Task<TaskItemDto?> GetTaskDetailByIdAsync(int taskId)
        {
            var task = await _tasksRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                return null;
            return new TaskItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                DueDate = task.DueDate,
                Order = task.Order,
                CreatedOn = task.CreatedOn,
                LastUpdatedOn = task.LastUpdatedOn,
                BoardId = task.BoardId,
                BoardName = task.Board.Name,
                TaskAttachments = task.TaskAttachments?.Select(a => new TaskAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FilePath = a.FilePath,
                    ContentType = a.ContentType,
                    UploadedByUserId = a.UploadedByUserId,
                    UploadedOn = a.UploadedOn,
                    UploadedByUserName = a.UploadedByUser.Name
                }).ToList(),
                TaskComments = task.TaskComments?.Select(c => new TaskCommentDto
                {
                    Id = c.Id,
                    CommentText = c.CommentText,
                    CreatedByUserId = c.CreatedByUserId,
                    CreatedOn = c.CreatedOn,
                    CreatedByUserName = c.CreatedByUser.Name


                }).ToList(),
                TaskAssignments = task.TaskAssignments?.Select(a => new TaskAssignmentDto
                {
                    TaskItemId = a.TaskItemId,
                    AppUserId = a.AppUserId,
                }).ToList() ?? new List<TaskAssignmentDto>()
            };
        }

        public  async Task<TaskCommentDto> AddCommentAsync(int taskId,int userId,TaskCommentDto commentDto)
        {
            var comment = new Domain.Entities.TaskComment
            {
                CommentText = commentDto.CommentText,
                CreatedByUserId = userId,
                TaskItemId = taskId,
                CreatedOn = DateTime.UtcNow
            };
            await _tasksRepository.AddCommentToTaskAsync(comment);

            var taskItem = await _tasksRepository.GetTaskByIdAsync(taskId);
            if (taskItem != null)
            {
                taskItem.LastUpdatedOn = DateTime.UtcNow;
            }


            await _tasksRepository.SaveChangesAsync();
            return new TaskCommentDto
            {
                Id = comment.Id,
                CommentText = comment.CommentText,
                CreatedByUserId=userId,
                CreatedOn = comment.CreatedOn,
                CreatedByUserName=comment.CreatedByUser?.Name??""
            };
        }
        public async Task<TaskCommentDto?> UpdateCommentAsync(int commentId,int userId,bool isAdmin,string updatedText)
        {
            var comment=await _tasksRepository.GetCommentByIdAsync(commentId);
            if (comment == null) 
                return null;
            if (!isAdmin && comment.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("Yor are not allowed to edit this comment");

            comment.CommentText= updatedText;

            var task = await _tasksRepository.GetTaskByIdAsync(comment.TaskItemId);
            if (task != null)
                task.LastUpdatedOn = DateTime.UtcNow;

            await _tasksRepository.SaveChangesAsync();

            return new TaskCommentDto
            {
                Id = comment.Id,
                CommentText = comment.CommentText,
                CreatedByUserId = comment.CreatedByUserId,
                CreatedOn = comment.CreatedOn,
                CreatedByUserName = comment.CreatedByUser?.Name ?? ""
            };

        }
        public async Task<bool> DeleteCommentAsync(int commentId, int userId, bool isAdmin)
        {
            var comment = await _tasksRepository.GetCommentByIdAsync(commentId);
            if (comment == null)
                return false;

            if (!isAdmin && comment.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to delete this comment");

            await _tasksRepository.DeleteCommentAsync(comment);

            var task = await _tasksRepository.GetTaskByIdAsync(comment.TaskItemId);
            if (task != null)
                task.LastUpdatedOn = DateTime.UtcNow;

            await _tasksRepository.SaveChangesAsync();
            return true;
        }

        public async Task<TaskAttachmentDto> AddAttachmentAsync(int taskId,int userId,IFormFile file)
        {

            var folder = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Task-attachment");
            Directory.CreateDirectory(folder);

            var savedName = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(folder, savedName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            var attachment = new Domain.Entities.TaskAttachment
            {
                FileName = file.FileName,
                FilePath = $"/Task-attachment/{savedName}",
                ContentType = file.ContentType,
                UploadedByUserId = userId,
                TaskItemId = taskId,
                UploadedOn = DateTime.UtcNow
            };
            await _tasksRepository.AddAttachmentAsync(attachment);
            var taskItem = await _tasksRepository.GetTaskByIdAsync(taskId);
            if (taskItem != null)
            {
                taskItem.LastUpdatedOn = DateTime.UtcNow;
            }


            await _tasksRepository.SaveChangesAsync();
            return new TaskAttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FilePath = attachment.FilePath,
                ContentType = attachment.ContentType,
                UploadedByUserId=userId,
                UploadedOn = attachment.UploadedOn,
                UploadedByUserName=attachment.UploadedByUser?.Name??""
            };
        }
        public async Task<bool> DeleteAttachmentAsync(int attachmentId, int userId,bool isAdmin)
        {
            var attachment = await _tasksRepository.GetAttachmentByIdAsync(attachmentId);
            if (attachment == null)
                return false;

            // OPTIONAL: allow only uploader or admin later
            if (!isAdmin && attachment.UploadedByUserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to delete this attachment");

            // 1️⃣ Delete physical file
            var physicalPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                attachment.FilePath.TrimStart('/'));

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            // 2️⃣ Delete DB record
            await _tasksRepository.DeleteAttachmentAsync(attachment);
         


            await _tasksRepository.SaveChangesAsync();

            return true;
        }


    }
}
