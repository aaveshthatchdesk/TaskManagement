using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.Interaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalProjectsAsync();
        Task<int> GetActiveProjectsAsync();
        Task<int> GetTotalMembersAsync();
        Task<int> GetCompletedTasksAsync();
        Task<int> GetActiveTasksAsync();
        Task<int> GetActiveProjectByManagersAsync(int managerId);
        Task<int> GetActiveTasksByManagerAsync(int managerId);
        Task<int> GetTotalMembersByManagerAsync(int managerId);
        Task<int> GetCompletedTasksByManagerAsync(int managerId);
    }
}
