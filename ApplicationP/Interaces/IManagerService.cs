using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface IManagerService
    {
        public Task<IEnumerable<ProjectDto>> GetProjectsByManagerAsync(int managerId, string? filter = "All");


        Task<List<ManagerDto>> GetAllManagersAsync();

        public Task<PagedResult<ManagerDto>> GetManagersPagedAsync(int pageNumber, int pageSize, string search);
        public Task<bool> AssignManagerAsync(int projectId, List<int> memberIds);

         public Task<bool> RemoveManagerAsync(int projectId, int managerId);

    }
}
