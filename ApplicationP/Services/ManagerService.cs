using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;

namespace Task.Application.Services
{
    public class ManagerService:IManagerService
    {
        private readonly IManagerRepository managerRepository;

        public ManagerService(IManagerRepository managerRepository)
        {
            this.managerRepository = managerRepository;
        }
        public async Task<IEnumerable<ProjectDto>> GetProjectsByManagerAsync(int managerId, string? filter = "All")
        {

            var projects = await managerRepository.GetProjectsByManagerAsync(managerId, filter);

            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status.ToString(),
                CreatedDate = p.CreatedDate,
                Priority = p.Priority,
                Visibility = p.Visibility,
                Progress = p.Progress,
                ManagerIds = p.Managers.Where(m=>m!=null).Select(m => m.AppUserId).ToList(),
                ManagerNames = string.Join(", ", p.Managers?.Where(m => m?.AppUser != null).Select(m => m.AppUser.Name) ?? new List<string>()),

                ManagerEmail = p.Managers?.Where(m => m?.AppUser != null).Select(m => m.AppUser.Email).FirstOrDefault()?? "N/A",

                MemberCount = p.TeamMembers?.Count(u => u != null) ?? 0,
                MemberIntials = p.TeamMembers?
                    .Where(u => u != null && !string.IsNullOrWhiteSpace(u.Name))
                    .Select(u => u.Name.Trim()[0].ToString().ToUpper())
                    .Distinct()
                    .ToList() ?? new List<string>(),

                TeamMembers = p.TeamMembers.Select(u => u.Name).ToList(),
                Tasks = p.Boards
                 .SelectMany(b => b.TaskItems)
                 .Select(t => t.Title)
                 .ToList()






            }).ToList();


        }


        public async Task<List<ManagerDto>> GetAllManagersAsync()
        {
            var managers = await managerRepository.GetAllManagersAsync();

            return managers.Select(m => new ManagerDto
            {
                Id = m.Id,
                Name = m.Name,
                Role = m.Role
            }).ToList();
        }

        public async Task<PagedResult<ManagerDto>> GetManagersPagedAsync(int pageNumber, int pageSize, string search)
        {
            var (managers, totalCount) = await managerRepository.GetManagersPagedAsync(pageNumber, pageSize, search);
            var result = new PagedResult<ManagerDto>
            {
                TotalCount = totalCount
            };

            foreach (var manager in managers)
            {
                var projectCount = await managerRepository.GetProjectCountByManagerAsync(manager.Id);
                var teamCount = await managerRepository.GetTeamCountByManagerAsync(manager.Id);

                result.Items.Add(new ManagerDto
                {
                    Id = manager.Id,
                    Name = manager.Name,
                    Role = manager.Role,
                    ProjectCount = projectCount,
                    TeamCount = teamCount
                });
            }

            return result;
        }
        public async Task<bool> AssignManagerAsync(int projectId, List<int> memberIds)
        {
            return await managerRepository.AssignManagerAsync(projectId, memberIds);
        }
        public async Task<bool> RemoveManagerAsync(int projectId, int managerId)
        {
            return await managerRepository.RemoveManagerAsync(projectId, managerId);
        }


    }
}
