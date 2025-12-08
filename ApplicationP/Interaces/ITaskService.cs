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

        public Task<PagedResult<ProjectDto>> GetAllProjectsPagedAsync(string currentUserId, bool isAdmin, string? filter, int page, int pageSize, string? search, int? managerId, DateTime? createdDate, DateTime? startDate, DateTime? endDate);
        public Task<List<MemberDto>> GetProjectMembersAsync(int projectId);
        //public  Task<List<MemberDto>> GetProjectMembersAssignedAsync(int projectId);
        public Task<bool> AssignMembersAsync(int projectId, List<int> memberIds);
        public Task<List<ManagerDto>> GetManagersByProjectAsync(int projectId);

        public Task<ProjectDto> GetProjectByIdAsync(int id);
        public Task<ProjectDto> AddProjectAsync(ProjectDto projectDto);

        public Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto project);
        public Task<bool> DeleteProjectAsync(int id);
    }
}
