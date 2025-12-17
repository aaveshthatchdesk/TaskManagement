using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;


namespace Task.Infrastructure.Repository
{
    public class AssignMemberRepository : IAssignMemberRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public AssignMemberRepository(TaskDbContext taskDbContext) 
        {
            _taskDbContext = taskDbContext;
        }

        public async Task<bool> ExistsAsync(int taskId, int userId)
        {
            return await _taskDbContext.taskAssignments
                .AnyAsync(ta => ta.TaskItemId == taskId && ta.AppUserId == userId);
        }
        public async Task<TaskAssignment?> GetAsync(int taskId, int userId)
        {
            return await _taskDbContext.taskAssignments
                .FirstOrDefaultAsync(ta => ta.TaskItemId == taskId && ta.AppUserId == userId);
        }
        public async System.Threading.Tasks.Task AddAsync(TaskAssignment assignment)
        {
            _taskDbContext.taskAssignments.Add(assignment);
            await _taskDbContext.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task RemoveAsync(TaskAssignment assignment)
        {
            _taskDbContext.taskAssignments.Remove(assignment);
            await _taskDbContext.SaveChangesAsync();
        }
        public async System.Threading.Tasks.Task SaveChangesAsync()
        {
            await _taskDbContext.SaveChangesAsync();
        }
    }
}
