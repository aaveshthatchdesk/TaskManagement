using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;

namespace Task.Application.Interaces
{
    public interface IActivityLogService
    {

        System.Threading.Tasks.Task LogAsync(int projectId,
        int userId,
        string actionType,
        string description,
        int? taskId = null,
        int? boardId = null, 
        int? targetUserId = null);

        Task<List<ActivityLogDto>> GetActivitiesForMemberAsync(
    
      int memberId,
      int take = 5);
    }
    
}
