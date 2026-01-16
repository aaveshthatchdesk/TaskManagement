using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Task.Infrastructure.Repository
{
    public class ManagerRepository:IManagerRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public ManagerRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }
        public async Task<IEnumerable<Project>> GetProjectsByManagerAsync(int managerId, string? filter = "All")
        {
            IQueryable<Project> query = _taskDbContext.projects
                .Where(p=>p.Managers.Any(m=>m.AppUserId==managerId))
                
                .Include(p => p.Managers)
                    .ThenInclude(m=>m.AppUser)
                .Include(p => p.Boards)
                    .ThenInclude(b => b.TaskItems)
                    .ThenInclude(t => t.TaskAssignments)
                    .ThenInclude(a => a.AppUser);

            ;


            switch (filter?.ToLower())
            {
                case "active":
                    query = query.Where(p => p.Status == ProjectStatus.Active);
                    break;
                case "completed":
                    query = query.Where(p => p.Status == ProjectStatus.Completed);
                    break;
                case "archived":
                    query = query.Where(p => p.Status == ProjectStatus.Archived);
                    break;
            }



            query = query.Where(p => p.Managers.Any(m => m.AppUserId == managerId));


            return await query.ToListAsync();
        }


        public async Task<List<AppUser>> GetAllManagersAsync()
        {
            return await _taskDbContext.appUsers
                .Where(u => u.Role == "Manager")
                .ToListAsync();
        }

        public async Task<(List<AppUser> Managers,int TotalCount)> GetManagersPagedAsync(int pageNumber,int pageSize,string search)
        {
            

            var query = _taskDbContext.appUsers
                 .Where(u => u.Role.ToLower() == "manager");
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u => u.Name.Contains(search) );

            var totalCount = await query.CountAsync();
            var managers = await query
        .OrderByDescending(u => u.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

            return (managers, totalCount);

        }
        public async Task<int> GetProjectCountByManagerAsync(int managerId)
        {
            return await _taskDbContext.projects
       .Where(p => p.Managers.Any(m => m.AppUserId == managerId))
       .CountAsync();
        }
        public async Task<int> GetTeamCountByManagerAsync(int managerId)
        {


            return await _taskDbContext.projects
     .Where(p => p.Managers.Any(m => m.AppUserId == managerId))
     .SelectMany(p => p.Boards)
     .SelectMany(b => b.TaskItems)
     .SelectMany(t => t.TaskAssignments)
     .Select(a => a.AppUserId)
     .Distinct()
     .CountAsync();
        }


        public async Task<bool>AssignManagerAsync(int projectId,List<int>managerIds)
        {
            var project=await _taskDbContext.projects
                .Include(p=>p.Managers)
                  .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new Exception("project not found");

            var users=await _taskDbContext.appUsers
                     .Where(u => managerIds.Contains(u.Id))
                      .ToListAsync();

            foreach (var user in users)
            {
               
                if (!project.Managers.Any(pm => pm.AppUserId == user.Id))
                {
                    project.Managers.Add(new ProjectManager
                    {
                        ProjectId = project.Id,
                        AppUserId = user.Id,
                        AppUser = user
                    });
                }
            }

            await _taskDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveManagerAsync(int projectId, int managerId)
        {
            var projectManager = await _taskDbContext.ProjectManagers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.AppUserId == managerId);

            if (projectManager == null)
                return false;

            _taskDbContext.ProjectManagers.Remove(projectManager);
            await _taskDbContext.SaveChangesAsync();

            return true;
        }


   




    }

}
