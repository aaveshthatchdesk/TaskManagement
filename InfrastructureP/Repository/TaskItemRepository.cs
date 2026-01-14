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

namespace Task.Infrastructure.Repository
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public TaskItemRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }

        public async Task<IEnumerable<TaskItem>>GetByProjectIdAsync(int projectId)
        {
            return await _taskDbContext.tasks
                .Include(t=>t.TaskCreators)
                  .ThenInclude(c=>c.AppUser)
                .Include(t => t.TaskAssignments)
                  .ThenInclude(a => a.AppUser)
                .Include(t => t.Board)
                  .Where(t => t.Board.ProjectId == projectId)
               .OrderBy(t => t.BoardId)
               .ThenBy(t=>t.Order)
               .ToListAsync();
        }
        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _taskDbContext.tasks
                .Include(t=>t.Board)
                .Include(t=>t.TaskCreators)
                  .ThenInclude(c=>c.AppUser)
                .Include(t => t.TaskAssignments)
                  .ThenInclude(a => a.AppUser)
                 .FirstOrDefaultAsync(t => t.Id == id);
        }
       

        public async Task<List<TaskItem>>GetTasksForMemberAsync(int memberId)
        {
            return await _taskDbContext.tasks
                .Include(t => t.Board)
                  .ThenInclude(b => b.Project)
                .Include(t=> t.TaskAssignments)
                    .ThenInclude(a=>a.AppUser)
                  .Where(t => t.TaskAssignments.Any(a => a.AppUserId == memberId))
               
               .ToListAsync();
        }
        public async Task<TaskItem> CreateAsync(TaskItem task,int createdByUserId)
        {
            _taskDbContext.tasks.Add(task);
                await _taskDbContext.SaveChangesAsync();

            var creator = new TaskCreator
            {
                TaskItemId = task.Id,
                AppUserId = createdByUserId
            };
            _taskDbContext.TaskCreators.Add(creator);
            await _taskDbContext.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            _taskDbContext.tasks.Update(task);
            await _taskDbContext.SaveChangesAsync();
            return task;
        }
        public async Task<bool> DeleteAsync(TaskItem task)
        {
            _taskDbContext.tasks.Remove(task);
            return await _taskDbContext.SaveChangesAsync() > 0;
        }
        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _taskDbContext.tasks
                .Include(t=>t.Board)
                .Include(t => t.TaskAssignments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
           return await _taskDbContext.SaveChangesAsync()>0;
           
        }

        public async Task<List<TaskItem>> GetByIdsAsync(List<int> ids)
        {
            return await _taskDbContext.tasks
                
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
        }
        public async Task<List<TaskItem>> GetByIdsWithBoardAndProjectAsync(List<int> ids)
        {
            return await _taskDbContext.tasks
                .Include(t => t.Board)
                    .ThenInclude(b => b.Project)
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
        }

        public async Task<List<TaskItem>> GetUpcomingDeadlineTasksForMemberAsync(int memberId,int take)
        {
            var today = DateTime.UtcNow.Date;

            return await _taskDbContext.tasks
                .Include(t=>t.Board)
                  .ThenInclude(b=>b.Project)
                  .Include(t=>t.TaskAssignments)
                  .Where(t=>
                 !t.IsCompleted&&
                 t.DueDate!=null &&
                 t.DueDate>=today&&
                 t.TaskAssignments.Any(a=>a.AppUserId==memberId)

                )
                  .OrderBy(t=>t.DueDate)
                  .Take(take)
                  .ToListAsync();

        }


    }
}
