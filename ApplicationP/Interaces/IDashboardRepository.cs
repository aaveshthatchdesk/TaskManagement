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
    }
}
