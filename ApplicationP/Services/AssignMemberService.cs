using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;


namespace Task.Application.Services
{
    public class AssignMemberService : IAssignMemberService
    {
        private readonly IAssignMemberRepository _assignMemberRepository;
        private readonly ITaskItemRepository _taskItemRepository;

        public AssignMemberService(IAssignMemberRepository assignMemberRepository, ITaskItemRepository taskItemRepository)
        {
            _assignMemberRepository = assignMemberRepository;
            _taskItemRepository = taskItemRepository;
        }
        public async Task<bool> AssignedMemberAsync(int taskId, int userId)
        {
            var task = await _taskItemRepository.GetByIdAsync(taskId);
            if (task == null)
                return false;

            if (await _assignMemberRepository.ExistsAsync(taskId, userId))
            {
                return false;
            }
            var assignment = new Domain.Entities.TaskAssignment
            {
                TaskItemId = taskId,
                AppUserId = userId
            };
            await _assignMemberRepository.AddAsync(assignment);
            var taskitem = await _taskItemRepository.GetTaskByIdAsync(taskId);
            if (taskitem != null)
            {
                taskitem.LastUpdatedOn = DateTime.UtcNow;
                await _taskItemRepository.SaveChangesAsync();
            }

            return true;

        }
        public async Task<bool> RemoveMemberAsync(int taskId, int userId)
        {
            var assignment = await _assignMemberRepository.GetAsync(taskId, userId);
            if (assignment == null)
                return false;
            await _assignMemberRepository.RemoveAsync(assignment);
            var task = await _taskItemRepository.GetTaskByIdAsync(taskId);
            if (task != null)
            {
                task.LastUpdatedOn = DateTime.UtcNow;
                await _taskItemRepository.SaveChangesAsync();
            }

            return true;
        }
    }
}
