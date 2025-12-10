using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Infrastructure.DbContext;

namespace Task.Infrastructure.Repository
{
    public  class DashboardRepository:IDashboardRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public DashboardRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }

        public async Task<int> GetTotalProjectsAsync()
        {
            return await _taskDbContext.projects.CountAsync();
        }

        public async Task<int> GetActiveProjectsAsync()
        {

            return await _taskDbContext.projects.CountAsync(p => p.IsActive);


        }
        public async Task<int> GetTotalMembersAsync()
        {
            return await _taskDbContext.appUsers.CountAsync();
        }

        public async Task<int> GetCompletedTasksAsync()
        {
            return await _taskDbContext.tasks.CountAsync(t => t.IsCompleted);
        }

        public async Task<int> GetActiveTasksAsync()
        {
            return await _taskDbContext.tasks.CountAsync(t => !t.IsCompleted);
        }

        public async Task<int> GetActiveProjectByManagersAsync(int managerId)
        {
            return await _taskDbContext.projects
                .Where(p=>p.Managers.Any(m=>m.AppUserId == managerId))
                .CountAsync();
        }

        public async Task<int> GetCompletedTasksByManagerAsync(int managerId)
        {
            return await _taskDbContext.tasks
                .Where(t =>
                    t.IsCompleted &&
                    t.Board.Project.Managers.Any(pm => pm.AppUserId == managerId)
                )
                .CountAsync();
        }

        public async Task<int> GetActiveTasksByManagerAsync(int managerId)
        {
            return await _taskDbContext.tasks
                .Where(t =>
                    !t.IsCompleted &&
                    t.Board.Project.Managers.Any(pm => pm.AppUserId == managerId)
                )
                .CountAsync();
        }

        public async Task<int> GetTotalMembersByManagerAsync(int managerId)
        {
            return await _taskDbContext.taskAssignments
                .Where(a =>
                    a.TaskItem.Board.Project.Managers.Any(pm => pm.AppUserId == managerId)
                )
                .Select(a => a.AppUserId)
                .Distinct()
                .CountAsync();
        }

    }
}
