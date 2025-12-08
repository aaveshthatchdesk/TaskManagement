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

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _taskDbContext.tasks
                .Include(t => t.TaskAssignments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            await _taskDbContext.SaveChangesAsync();
            return true;
        }



    }
}
