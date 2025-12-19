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
        Task<string>AddAttachmentAsync( TaskAttachment attachment);
        Task<bool> SaveChangesAsync();
    }
}
