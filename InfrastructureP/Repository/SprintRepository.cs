using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Task.Infrastructure.Repository
{
     public class SprintRepository:ISprintRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public SprintRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }
        public async Task<IEnumerable<Sprint>> GetAllAsync()
        {
            return await _taskDbContext.sprints.Include(s => s.TaskItems)
              
                   
                .ThenInclude(t => t.TaskAssignments)
                    .ThenInclude(a => a.AppUser).ToListAsync();
        }
        public async Task<IEnumerable<Sprint>> GetSprintsStats(int userId,string role)
        {
            IQueryable<Sprint> query = _taskDbContext.sprints
        .Include(s => s.TaskItems)
            .ThenInclude(t => t.TaskAssignments)
        .Include(s => s.Project)
            .ThenInclude(p => p.Managers);

            if (role == "Manager")
            {
                query = query.Where(s =>
                    s.Project.Managers.Any(m => m.AppUserId == userId));
            }
            else if (role == "Member")
            {
                query = query.Where(s =>
                    s.TaskItems.Any(t =>
                        t.TaskAssignments.Any(a => a.AppUserId == userId)));
            }

            //return await _taskDbContext.sprints.Include(s => s.TaskItems)


            //   .ThenInclude(t => t.TaskAssignments)
            //       .ThenInclude(a => a.AppUser).ToListAsync();

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Sprint>> GetAllSprintsOnly()
        {
            return await _taskDbContext.sprints.ToListAsync();
        }
        public async Task<Sprint> GetSprintsByProjectAsync(int projectId)
        {
            return await _taskDbContext.projects
         .Where(p => p.Id == projectId)
         .Select(p => p.Sprint)
         .FirstOrDefaultAsync();


        }

        public async Task<(List<Sprint> Sprints,int TotalCount)>GetSprintsAsync(int userId,string role,string ?search ,string filter,int page,int pageSize)
        {
            IQueryable<Sprint> query = _taskDbContext.sprints.Include(s => s.TaskItems)
                    .ThenInclude(t => t.TaskAssignments)
                        .ThenInclude(a => a.AppUser)
                    .Include(s => s.TaskItems)
                        .ThenInclude(t => t.Board)
                            .ThenInclude(b => b.Project)
                               .ThenInclude(t => t.Managers);


            if(role=="Manager")
            {
                query=query.Where(s => s.Project.Managers.Any(m => m.AppUserId == userId));
            }
            else if(role=="Member")
            {
                query=query.Where(s=> s.TaskItems.Any(t => t.TaskAssignments.Any(a => a.AppUserId == userId)) );
            }
                query = filter switch
                {
                    "Completed" => query.Where(s => s.TaskItems.Any() && s.TaskItems.All(t => t.IsCompleted)),
                    "Active" => query.Where(s => s.TaskItems.Any(t => t.IsCompleted)),
                    "Planning" => query.Where(s => !s.TaskItems.Any(t => t.IsCompleted)),
                    _ => query
                };

            if (!string.IsNullOrEmpty(search))
            {
                query=query.Where(s=>s.Name.Contains(search)||s.TaskItems.Any(t=>t.Board!.Project!.Name.Contains(search)));
            }
            int totalCount = await query.CountAsync();
            var sprints=await query.OrderByDescending(s=>s.StartDate).Skip((page-1)*pageSize).Take(pageSize).ToListAsync();
            return (sprints, totalCount);
        }
        public async Task<IEnumerable<Sprint>> GetAllWithTasksAsync()
        {
            return await _taskDbContext.sprints
                .Include(s => s.TaskItems)
                    .ThenInclude(t => t.TaskAssignments)
                .ToListAsync();
        }

        public async Task<Sprint?> GetSprintByIdAsync(int id)
        {
            return await _taskDbContext.sprints
             .Include(s => s.TaskItems)
                    .ThenInclude(t => t.Board)
                          .ThenInclude(b => b.Project)

             .Include(s => s.TaskItems)
                .ThenInclude(t => t.TaskAssignments)
                    .ThenInclude(a => a.AppUser)
             .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Sprint> AddAsync(Sprint sprint)
        {
            _taskDbContext.sprints.Add(sprint);
            await _taskDbContext.SaveChangesAsync();
            return sprint;
        }
        public  async Task<Sprint> UpdateAsync(int id,Sprint sprint)
        {
            var data = _taskDbContext.sprints.Include(s => s.TaskItems).FirstOrDefault(s => s.Id==id);
            if (data == null)
            {
                return null;
            }
            data.Name = sprint.Name;
            data.StartDate = sprint.StartDate;
            data.EndDate = sprint.EndDate;

            _taskDbContext.sprints.Update(data);
            await _taskDbContext.SaveChangesAsync();


            return data;
        }
        public async Task<bool>DeleteAsync(int id)
        {
            var data = _taskDbContext.sprints.FirstOrDefault(s => s.Id == id);
            if (data != null)
            {
                _taskDbContext.sprints.Remove(data);

                await _taskDbContext.SaveChangesAsync();
            }
            return true;
        }
        
    }
}
