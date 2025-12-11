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
    public class NewProjectRepository:INewProjectRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public NewProjectRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }
        public async Task<List<AppUser>> GetUsersByIdsAsync(List<int> ids)
        {
            return await _taskDbContext.appUsers
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }

       
        public async Task<Project> AddProjectAsync(Project project)
        {
            _taskDbContext.projects.Add(project);
            await _taskDbContext.SaveChangesAsync();
            return project;
        }

    }
}
