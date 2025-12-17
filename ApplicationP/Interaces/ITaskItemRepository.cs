using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface ITaskItemRepository
    {
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<bool> SaveChangesAsync();


        Task<IEnumerable<TaskItem>> GetByProjectIdAsync(int projectId);
        Task<TaskItem?> GetByIdAsync(int id);

        Task<TaskItem> CreateAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(TaskItem task);
        Task<List<TaskItem>> GetByIdsAsync(List<int> ids);
        
    }
}
