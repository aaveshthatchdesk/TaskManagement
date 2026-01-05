using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Domain.Entities;

namespace Task.Application.Services
{
    public  class ActivityLogService:IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogService(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }
        public async System.Threading.Tasks.Task LogAsync(int projectId,
        int userId,
        string actionType,
        string description,
        int? taskId = null,
        int? boardId = null, int? targetUserId = null)
        {
            await _activityLogRepository.AddAsync( new ActivityLog
            {
                ProjectId = projectId,
                TaskItemId = taskId,
                BoardId = boardId,
                PerformedByUserId = userId,
                TargetUserId= targetUserId,
                ActionType = actionType,
                Description = description
            });


        }

        public async Task<List<ActivityLogDto>> GetActivitiesForMemberAsync(
       
       int memberId,
       int take = 5)
        {
            // 🔒 future: permission checks
            return await _activityLogRepository
                .GetActivitiesForMemberAsync( memberId, take);
        }
    }
}
