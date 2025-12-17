using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;

namespace Task.Application.Interaces
{
    public interface ITaskItemService
    {
        Task<bool> UpdateTaskAsync(int taskId,TaskItemDto dto);

        Task<IEnumerable<TaskItemDto>> GetTasksByProjectAsync(int projectID);
        Task<TaskItemDto?> GetTaskByIdAsync(int taskId);

        Task<TaskItemDto> CreateTaskAsync(TaskItemDto dto);
        Task<TaskItemDto> UpdateTasksAsync(int taskId, TaskItemDto dto);
        Task<bool> DeleteTaskAsync(int taskId);
        Task<bool> ReorderTasksAsync(List<TaskReorderDto> tasks);
    }
}
