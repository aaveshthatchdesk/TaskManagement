using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface ITaskRepository
    {
        public Task<IEnumerable<Project>> GetAllProjectsAsync(string currentUserId,bool isAdmin,string? filter = "All");
        public Task<Project> GetProjectByIdAsync(int id);
        public  Task<Project> AddProjectAsync(Project project);

        public Task<Project> UpdateProjectAsync(int id, Project project);
        public Task<bool> DeleteProjectAsync(int id);
    }
}
