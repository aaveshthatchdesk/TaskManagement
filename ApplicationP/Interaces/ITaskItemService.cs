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
        Task<bool> UpdateTaskAsync(int taskId,TaskItemDto dto,int updateByUserId);

        public Task<bool> UpdateTaskDescriptionAsync(int taskId, string description );
        Task<IEnumerable<TaskItemDto>> GetTasksByProjectAsync(int projectID);
        Task<TaskItemDto?> GetTaskByIdAsync(int taskId);

        Task<List<MemberBoardDto>> GetMemberBoardsAsync(int memberId);

        Task<TaskItemDto> CreateTaskAsync(int createdByUserId,TaskItemDto dto);
        Task<TaskItemDto> UpdateTasksAsync(int taskId, TaskItemDto dto);
        Task<bool> DeleteTaskAsync(int taskId,int userId);
        Task<bool> ReorderTasksAsync(List<TaskReorderDto> tasks,int userId);
        Task<bool> ReorderTaskForMembersAsync(List<TaskReorderForMembersDto> tasks,int UserId);
        Task<List<UpcomingDeadlinesDto>> GetUpcomingDeadlinesAsync(int memberId, int take = 5);
    }
}
