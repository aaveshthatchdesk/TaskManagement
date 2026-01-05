using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface IActivityLogRepository
    {
       System.Threading.Tasks.Task AddAsync(ActivityLog log);
        //Task<List<ActivityLog>>GetRecentByProjectAsync(int projectId, int take);

        Task<List<ActivityLogDto>> GetActivitiesForMemberAsync(
       
           int memberId,
           int take =5);
    }
}
