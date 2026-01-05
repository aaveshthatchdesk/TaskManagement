using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;


namespace Task.Application.Services
{
    public class AssignMemberService : IAssignMemberService
    {
        private readonly IAssignMemberRepository _assignMemberRepository;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IActivityLogService _activityLogService;
        private readonly IMemberRepository memberRepository;

        public AssignMemberService(IAssignMemberRepository assignMemberRepository, ITaskItemRepository taskItemRepository,IActivityLogService activityLogService,IMemberRepository memberRepository)
        {
            _assignMemberRepository = assignMemberRepository;
            _taskItemRepository = taskItemRepository;
            _activityLogService = activityLogService;
            this.memberRepository = memberRepository;
        }
        public async Task<bool> AssignedMemberAsync(int taskId, int userId,int createdUserId)
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
                await _activityLogService.LogAsync(
         projectId: taskitem.Board.ProjectId,
         userId: createdUserId, 
         actionType: "MemberAssigned",
       
         description: $"assigned to task '{taskitem.Title}'",
         taskId: taskitem.Id,
         boardId: taskitem.BoardId,
         targetUserId:userId
         
     );
                await _taskItemRepository.SaveChangesAsync();
            }

            return true;

        }
        public async Task<bool> RemoveMemberAsync(int taskId, int userId,int createdUserId)
        {
            var assignment = await _assignMemberRepository.GetAsync(taskId, userId);
            if (assignment == null)
                return false;
            await _assignMemberRepository.RemoveAsync(assignment);
            var task = await _taskItemRepository.GetTaskByIdAsync(taskId);
        
            if (task != null)
            {
                task.LastUpdatedOn = DateTime.UtcNow;
                

                await _activityLogService.LogAsync(
        projectId: task.Board.ProjectId,
        userId: createdUserId,
        actionType: "MemberUnassigned",
       
        description: $"removed  from task '{task.Title}'",
        taskId: task.Id,
        boardId: task.BoardId,
        targetUserId:userId
        
    );
                await _taskItemRepository.SaveChangesAsync();
            }

            return true;
        }
    }
}
