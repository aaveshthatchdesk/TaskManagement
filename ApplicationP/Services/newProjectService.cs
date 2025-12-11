using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Domain.Entities;

namespace Task.Application.Services
{
    public class newProjectService:INewProjectService
    {
        private readonly INewProjectRepository _newProjectRepository;

        public newProjectService(INewProjectRepository newProjectRepository)
        {
            _newProjectRepository = newProjectRepository;
        }
        //public async Task<ProjectDto> AddProjectAsync(ProjectDto projectDto)
        //{
        //    var project = new Project
        //    {
        //        Name = projectDto.Name,
        //        Description = projectDto.Description,
        //        Priority = projectDto.Priority,
        //        Visibility = projectDto.Visibility,
        //        CreatedDate = DateTime.Now,
        //        Status = Enum.TryParse<ProjectStatus>(projectDto.Status, true, out var status)
        //            ? status
        //            : ProjectStatus.Active,
        //        Boards = new List<Board>(),
        //        Managers=new List<ProjectManager>()
        //    };



        //    if (projectDto.ManagerIds != null && projectDto.ManagerIds.Any())
        //    {
        //        project.Managers = projectDto.ManagerIds.Select(id => new ProjectManager
        //        {
        //            AppUserId = id
        //        }).ToList();
        //    }

        //    var created = await _newProjectRepository.AddProjectAsync(project);

        //    return new ProjectDto
        //    {
        //        Id = created.Id,
        //        Name = created.Name,
        //        Description = created.Description,
        //        Priority = created.Priority,
        //        CreatedDate = created.CreatedDate,
        //        Visibility = created.Visibility,
        //        Progress = created.Progress,
        //        MemberCount = created.TeamMembers?.Count ?? 0,
        //        MemberIntials = created.TeamMembers?
        //            .Where(u => u != null)
        //            .Select(u => u.Name[0].ToString().ToUpper())
        //            .Distinct()
        //            .ToList(),
        //        ManagerIds = created.Managers.Select(m => m.AppUserId).ToList(),
        //        ManagerNames = string.Join(", ", created.Managers.Select(m => m.AppUser.Name)),
        //        Status = created.Status.ToString(),
        //        Boards = new List<BoardDto>()
        //    };
        //}

        public async Task<ProjectCreateDto> AddProjectAsync(ProjectCreateDto projectdto,int createdByUserId)
        {
            var project=new Project
            {
                Name = projectdto.Name,
                Description = projectdto.Description,
                Visibility = projectdto.Visibility,
                CreatedDate = DateTime.Now,
            };
            foreach(var memberId in projectdto.MemberIds)
            {
                project.ProjectMembers.Add(new ProjectMember
                {
                    AppUserId = memberId
                });
               
            }
            project.Managers.Add(new ProjectManager
            {
                AppUserId = createdByUserId
            });
            var created= await _newProjectRepository.AddProjectAsync(project);
            return new ProjectCreateDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Visibility = created.Visibility,
                CreatedDate = created.CreatedDate,
                MemberIds = created.ProjectMembers.Select(pm => pm.AppUserId).ToList(),
                ManagerIds = created.Managers.Select(pm => pm.AppUserId).ToList()
            };
        }

    }
}
