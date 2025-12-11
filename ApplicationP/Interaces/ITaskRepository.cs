using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface ITaskRepository
    {

        public Task<PagedResult<Project>> GetAllProjectsPagedAsync(string currentUserId, string role, string? filter, int page, int pageSize, string? search, int? managerId,int ? memberId, DateTime? createdDate, DateTime? startDate, DateTime? endDate);
        Task<List<AppUser>> GetProjectMembersAsync(int projectId);

        //public Task<List<ProjectMember>> GetProjectMembersAssignedAsync(int projectId);
        Task<bool> AssignMembersAsync(int projectId, List<int> memberIds);
        Task<List<AppUser>> GetManagersByProjectAsync(int projectId);
        public Task<Project> GetProjectByIdAsync(int id);
        public  Task<Project> AddProjectAsync(Project project);

        public Task<Project> UpdateProjectAsync(int id, Project project, ProjectDto projectDto);
        public Task<bool> DeleteProjectAsync(int id);
        Task<List<AppUser>> GetUsersByIdsAsync(List<int> ids);

    }
}
