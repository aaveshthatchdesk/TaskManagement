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

        Task<List<TaskItem>> GetTasksForMemberAsync(int memberId);

        Task<TaskItem> CreateAsync(TaskItem task,int createdByUserId);
        Task<TaskItem> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(TaskItem task);
        Task<List<TaskItem>> GetByIdsAsync(List<int> ids);
        Task<List<TaskItem>> GetByIdsWithBoardAndProjectAsync(List<int> ids);

        Task<List<TaskItem>> GetUpcomingDeadlineTasksForMemberAsync(int memberId, int take);
        System.Threading.Tasks.Task UpdateProjectStatusAsync(int projectId);


    }
}
