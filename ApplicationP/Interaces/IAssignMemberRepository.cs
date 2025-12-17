using TaskAsync = System.Threading.Tasks.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;



namespace Task.Application.Interaces
{
     public interface IAssignMemberRepository
    {
        Task<bool> ExistsAsync(int taskId, int userId);
        TaskAsync AddAsync(TaskAssignment assignment);
        TaskAsync RemoveAsync(TaskAssignment assignment);
        Task<TaskAssignment?> GetAsync(int taskId, int userId);
        TaskAsync SaveChangesAsync();
    }
}
