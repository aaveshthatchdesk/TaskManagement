using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;

namespace Task.Infrastructure.Repository
{
    public class FileAndCommentsInTasksRepository : IFileAndCommentsInTasksRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public FileAndCommentsInTasksRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int TaskId)
        {
            //return await _taskDbContext.tasks
            //     .Include(t=>t.TaskAttachments)
            //         .ThenInclude(a=>a.UploadedByUser)
            //     .Include(t=>t.TaskComments)
            //          .ThenInclude(c=>c.CreatedByUser)

            //     .FirstOrDefaultAsync(t => t.Id == TaskId);

            return await _taskDbContext.tasks
                .Include(t => t.Board)
            .Include(t=>t.TaskCreators)
                .ThenInclude(tc=>tc.AppUser)
           .Include(t => t.TaskAttachments)
               .ThenInclude(a => a.UploadedByUser)
           .Include(t => t.TaskComments)
               .ThenInclude(c => c.CreatedByUser)
           .Include(t => t.TaskAssignments)
                .ThenInclude(ta => ta.AppUser)
           .FirstOrDefaultAsync(t => t.Id == TaskId);


        }
        public async Task<TaskComment?> GetCommentByIdAsync(int commentId)
        {
            return await _taskDbContext.TaskComments
                .Include(c=>c.CreatedByUser)
                .FirstOrDefaultAsync(c=>c.Id == commentId);
        }
        public async Task<string> AddCommentToTaskAsync(TaskComment comment)
        {
            await _taskDbContext.TaskComments.AddAsync(comment);
            return "Comment added successfully";
        }
        public async Task<string> DeleteCommentAsync(TaskComment comment)
        {
            _taskDbContext.TaskComments.Remove(comment);
            return "Comment deleted successfully";
        }
        public async Task<string> AddAttachmentAsync(TaskAttachment attachment)
        {
            await _taskDbContext.TaskAttachments.AddAsync(attachment);
            return "Attachment added successfully";
        }
        public async Task<TaskAttachment?> GetAttachmentByIdAsync(int attachmentId)
        {
            return await _taskDbContext.TaskAttachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId);
        }
        public async Task<string> DeleteAttachmentAsync(TaskAttachment attachment)
        {
            _taskDbContext.TaskAttachments.Remove(attachment);
            return "Attachment deleted successfully";
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _taskDbContext.SaveChangesAsync() > 0;
        }

    }
    }
