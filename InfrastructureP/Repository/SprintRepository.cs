using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;

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
            return await _taskDbContext.sprints.Include(s=>s.TaskItems)
                .Include(s => s.TaskItems)
                    .ThenInclude(t => t.Board)
                          .ThenInclude(b => b.Project)

             .Include(s => s.TaskItems)
                .ThenInclude(t => t.TaskAssignments)
                    .ThenInclude(a => a.AppUser).ToListAsync();
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
            return await  _taskDbContext.sprints
             .Include(s=>s.TaskItems)
                    .ThenInclude(t=>t.Board)
                          .ThenInclude(b=>b.Project)
               
             .Include(s=>s.TaskItems)
                .ThenInclude(t=>t.TaskAssignments)
                    .ThenInclude(a=>a.AppUser)
             .FirstOrDefaultAsync(s=>s.Id == id);
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
            var data = _taskDbContext.sprints.Include(s => s.TaskItems).FirstOrDefault(s => s.Id == id);
            if (data != null)
            {
                _taskDbContext.sprints.Remove(data);

                await _taskDbContext.SaveChangesAsync();
            }
            return true;
        }
        
    }
}
