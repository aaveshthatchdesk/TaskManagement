using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;

namespace Task.Application.Services
{
    public class DashboardService:IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }
        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var totalProjects = await _dashboardRepository.GetTotalProjectsAsync();
            var activeProjects = await _dashboardRepository.GetActiveProjectsAsync();
            var totalMembers = await _dashboardRepository.GetTotalMembersAsync();
            var completedTasks = await _dashboardRepository.GetCompletedTasksAsync();
            var activeTasks = await _dashboardRepository.GetActiveTasksAsync();

            return new DashboardSummaryDto
            {
                TotalProjects = totalProjects,
                ActiveProjects = activeProjects,
                TotalMembers = totalMembers,
                CompletedTasks = completedTasks,
                ActiveTasks = activeTasks
            };
        }
}
}
