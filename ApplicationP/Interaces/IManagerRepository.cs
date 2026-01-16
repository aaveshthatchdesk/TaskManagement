using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface IManagerRepository
    {
        public Task<IEnumerable<Project>> GetProjectsByManagerAsync(int managerId, string? filter = "All");

        public Task<List<AppUser>> GetAllManagersAsync();
        public Task<(List<AppUser>Managers,int TotalCount)> GetManagersPagedAsync(int pageNumber, int pageSize, string search);
        public Task<int> GetProjectCountByManagerAsync(int managerId);

        public Task<int> GetTeamCountByManagerAsync(int managerId);
        public Task<bool> AssignManagerAsync(int projectId, List<int> memberIds);
        public Task<bool> RemoveManagerAsync(int projectId, int managerId);
       

    }
}
