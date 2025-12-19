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
                BoardId = task.BoardId,
                BoardName = task.Board.Name,
                TaskAttachments = task.TaskAttachments?.Select(a => new TaskAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    FilePath = a.FilePath,
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


                 }).ToList()
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
                UploadedByUserId = userId,
                TaskItemId = taskId,
                UploadedOn = DateTime.UtcNow
            };
            await _tasksRepository.AddAttachmentAsync(attachment);
            await _tasksRepository.SaveChangesAsync();
            return new TaskAttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                FilePath = attachment.FilePath,
                UploadedByUserId=userId,
                UploadedOn = attachment.UploadedOn,
                UploadedByUserName=attachment.UploadedByUser?.Name??""
            };
        }

    }
}
