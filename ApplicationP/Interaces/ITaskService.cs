using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;


namespace Task.Application.Interaces
{
    public interface ITaskService
    {

        public Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(string currentUserId,bool isAdmin,string? filter = "All");

        public Task<ProjectDto> GetProjectByIdAsync(int id);
        public Task<ProjectDto> AddProjectAsync(ProjectDto projectDto);

        public Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto project);
        public Task<bool> DeleteProjectAsync(int id);
    }
}
