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
    public class ArchiveRepository:IArchiveRepository
    {

        private readonly TaskDbContext _context;

        public ArchiveRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetProjectProgressAsync(int projectId)
        {
            var totalTasks = await _context.tasks.Where(t => t.Board.ProjectId == projectId).CountAsync();
            if(totalTasks==0)
                return 0;
            var completedTasks=await _context.tasks.Where(t=>t.Board.ProjectId == projectId&&t.IsCompleted).CountAsync();
            return (int)Math.Round((double)completedTasks/totalTasks*100);
        }
        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.projects
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Project project)
        {
            _context.projects.Update(project);
            await _context.SaveChangesAsync();
        }
    }
}
