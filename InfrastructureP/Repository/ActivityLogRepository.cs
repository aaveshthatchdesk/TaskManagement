using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Task.Infrastructure.Repository
{
  public  class ActivityLogRepository:IActivityLogRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public ActivityLogRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }
        public async  System.Threading.Tasks.Task AddAsync(ActivityLog log)
        {
            await _taskDbContext.ActivityLogs.AddAsync(log);
            await _taskDbContext.SaveChangesAsync();
            
        }
        //public async Task<List<ActivityLog>> GetRecentByProjectAsync(int projectId, int take)
        //{
        //    return await _taskDbContext.ActivityLogs
        //        .Include(a => a.PerformedByUser)
        //        .Where(a => a.ProjectId == projectId)
        //        .OrderByDescending(a => a.CreatedOn)
        //        .Take(take)
        //        .ToListAsync();
        //}


        public async Task<List<ActivityLogDto>> GetActivitiesForMemberAsync(
          
            int memberId,
            int take = 5)
        {

            var memberProjectIds = await _taskDbContext.ProjectMembers
       .Where(pm => pm.AppUserId == memberId)
       .Select(pm => pm.ProjectId)
       .ToListAsync();

            return await _taskDbContext.ActivityLogs
         .Include(a=>a.Project)
        .Include(a => a.PerformedByUser)
          .Include(a => a.TargetUser)
        .Include(a => a.TaskItem)
            .ThenInclude(t => t.TaskAssignments)
        .Where(a =>
            memberProjectIds.Contains(a.ProjectId) &&
            (
                a.PerformedByUserId == memberId
                ||
                (a.TaskItem != null &&
                 a.TaskItem.TaskAssignments.Any(x => x.AppUserId == memberId ))
            )
        )
        .OrderByDescending(a => a.CreatedOn)
        .Take(take)
        .Select(a => new ActivityLogDto
        {
            ProjectId = a.ProjectId,
            TaskItemId = a.TaskItemId,
            ActionType = a.ActionType,
            Description = a.Description,
            TargetUserId=a.TargetUserId,
            TargetUserName=a.TargetUser!=null?a.TargetUser.Name:null,
            CreatedOn = a.CreatedOn,
            PerformedByUserId = a.PerformedByUserId,
            PerformedByUserName = a.PerformedByUser.Name
        })
        .ToListAsync();
        }
    }
}
