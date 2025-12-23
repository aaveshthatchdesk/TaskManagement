using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface IFileAndCommentsInTasksRepository
    {
        Task<TaskItem?>GetTaskByIdAsync(int TaskId);
        Task<string>AddCommentToTaskAsync(TaskComment comment);
        Task<TaskComment?> GetCommentByIdAsync(int commnetId);

        Task<string>AddAttachmentAsync( TaskAttachment attachment);

        Task<string> DeleteCommentAsync(TaskComment commnet);
        Task<TaskAttachment?> GetAttachmentByIdAsync(int attachmentId);
        Task<string> DeleteAttachmentAsync(TaskAttachment attachment);
        Task<bool> SaveChangesAsync();
    }
}
