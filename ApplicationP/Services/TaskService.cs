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
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        public async Task<PagedResult<ProjectDto>> GetAllProjectsPagedAsync(string currentUserId, bool isAdmin, string? filter, int page, int pageSize, string? search, int? managerId, DateTime? createdDate, DateTime? startDate, DateTime? endDate)
        {
            var result = await taskRepository.GetAllProjectsPagedAsync(currentUserId, isAdmin, filter, page, pageSize, search, managerId, createdDate, startDate, endDate);

            return new PagedResult<ProjectDto>
            {
                TotalCount = result.TotalCount,
                Items = result.Items.Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status.ToString(),
                    CreatedDate = p.CreatedDate,
                    Priority = p.Priority,
                    Visibility = p.Visibility,
                    Progress = p.Progress,
                    MemberCount = p.ProjectMembers?.Count(u => u.AppUser != null) ?? 0,
                    MemberIntials = p.ProjectMembers?
    .Where(u => u != null && !string.IsNullOrWhiteSpace(u.AppUser.Name))
    .Select(u => u.AppUser.Name.Trim()[0].ToString().ToUpper())
    .Distinct()
    .ToList() ?? new List<string>(),


                    ManagerIds = p.Managers?.Where(m => m != null).Select(m => m.AppUserId).ToList() ?? new List<int>(),
                    ManagerNames = string.Join(", ", p.Managers?.Where(m => m?.AppUser != null).Select(m => m.AppUser.Name) ?? new List<string>()),
                    ManagerEmail = p.Managers?.Where(m => m?.AppUser != null)
                                .Select(m => m.AppUser.Email).FirstOrDefault() ?? "",

                    Boards = p.Boards.Select(b => new BoardDto
                    {
                        Id = b.Id,
                        Name = b.Name,
                        ProjectId = b.ProjectId,
                        TaskItems = b.TaskItems.Select(t => new TaskItemDto
                        {
                            Id = t.Id,
                            Title = t.Title,
                            Description = t.Description,
                            DueDate = t.DueDate,
                            BoardId = t.BoardId,
                            Order = t.Order,
                            SprintId = t.SprintId,
                            TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                            {
                                TaskItemId = a.TaskItemId,
                                AppUserId = a.AppUserId,

                            }).ToList()
                        }).ToList()
                    }).ToList()


                }).ToList()
            };
        }
        //public async Task<List<MemberDto>> GetProjectMembersAssignedAsync(int projectId)
        //{
        //    var users = await taskRepository.GetProjectMembersAsync(projectId);

        //    return users.Select(u => new MemberDto
        //    {
        //        Id = u.Id,
        //        Name = u.Name,
            
        //    })
        //    .ToList();

       
        //}

        public async Task<List<MemberDto>> GetProjectMembersAsync(int projectId)
        {
            var users = await taskRepository.GetProjectMembersAsync(projectId);

            return users.Select(u => new MemberDto
            {
                Id = u.Id,
                Name = u.Name
            }).ToList();
        }

        public async Task<bool> AssignMembersAsync(int projectId, List<int> memberIds)
        {
            return await taskRepository.AssignMembersAsync(projectId, memberIds);
        }

        public async Task<List<ManagerDto>> GetManagersByProjectAsync(int projectId)
        {
            var managers = await taskRepository.GetManagersByProjectAsync(projectId);

            return managers.Select(m => new ManagerDto
            {
                Id = m.Id,
                Name = m.Name,
                Role = m.Role
            }).ToList();
        }



        public async Task<ProjectDto> GetProjectByIdAsync(int id)
        {
            var p = await taskRepository.GetProjectByIdAsync(id);
            if (p == null)
            {
                return null;
            }
            return new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Priority = p.Priority,
                CreatedDate = p.CreatedDate,
                Progress = p.Progress,
                ManagerIds = p.Managers?.Select(m => m.AppUserId).ToList() ?? new List<int>(),
                ManagerNames = string.Join(", ", p.Managers?.Where(m=>m?.AppUser!=null).Select(m => m.AppUser.Name) ?? new List<string>()),
                ManagerEmail = p.Managers?.Where(m=>m?.AppUser!=null).Select(m => m.AppUser.Email).FirstOrDefault() ?? "",
                MemberCount = p.TeamMembers?.Count(u => u != null) ?? 0,
                MemberIntials = p.TeamMembers?
    .Where(u => u != null && !string.IsNullOrWhiteSpace(u.Name))
    .Select(u => u.Name.Trim()[0].ToString().ToUpper())
    .Distinct()
    .ToList() ?? new List<string>(),

                Status = p.Status.ToString(),
                Boards = p.Boards.Select(b => new BoardDto
                {
                    Id = b.Id,
                    Name = b.Name,

                    ProjectId = b.ProjectId,
                    TaskItems = b.TaskItems.Select(t => new TaskItemDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        BoardId = t.BoardId,
                        Priority=t.Priority,
                        IsCompleted = t.IsCompleted,
                        CompletedDate = t.CompletedDate,
                        Order = t.Order,
                        SprintId = t.SprintId,
                        TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                        {
                            TaskItemId = a.TaskItemId,
                            AppUserId = a.AppUserId,

                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }
        public async Task<ProjectDto> AddProjectAsync(ProjectDto projectDto)
        {


            var project = new Project
            {

                Name = projectDto.Name,
                Description = projectDto.Description,
                Priority = projectDto.Priority,
                Visibility = projectDto.Visibility,
                CreatedDate = DateTime.Now,
                Status = Enum.TryParse<ProjectStatus>(projectDto.Status, true, out var status)
                    ? status
                    : ProjectStatus.Active,

                //Managers = new List<ProjectManager>(),
                Boards = projectDto.Boards.Select(b => new Board
                {
                    Name = b.Name,

                    TaskItems = b.TaskItems.Select(t => new TaskItem
                    {
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Order = t.Order,
                        SprintId = t.SprintId,
                        TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignment
                        {
                            AppUserId = a.AppUserId
                        }).ToList()
                    }).ToList()
                }).ToList()??new List<Board>(),

                    Managers = projectDto.ManagerIds?.Select(id => new ProjectManager
                    {
                        AppUserId = id,
                        AppUser = null    // IMPORTANT: prevents EF from inserting new AppUser
                    }).ToList() ?? new List<ProjectManager>()
            };
            if (projectDto.ManagerIds != null && projectDto.ManagerIds.Any())
            {
                //var managers = await taskRepository.GetUsersByIdsAsync(projectDto.ManagerIds);

                //foreach (var manager in managers)
                //{
                //    project.Managers.Add(manager);
                //    manager.ManagedProjects.Add(project);
                //}
                project.Managers = projectDto.ManagerIds.Select(id => new ProjectManager
                {
                    AppUserId = id
                }).ToList();
            }


            var created = await taskRepository.AddProjectAsync(project);

            return new ProjectDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description,
                Priority = created.Priority,
                CreatedDate = created.CreatedDate,
                Visibility = created.Visibility,
                Progress = created.Progress,
                MemberCount = created.TeamMembers?.Count(u => u != null) ?? 0,
                MemberIntials = created.TeamMembers?
    .Where(u => u != null && !string.IsNullOrWhiteSpace(u.Name))
    .Select(u => u.Name.Trim()[0].ToString().ToUpper())
    .Distinct()
    .ToList() ?? new List<string>(),
                ManagerIds = created.Managers.Select(m => m.AppUserId).ToList(),
                ManagerNames = string.Join(", ", created.Managers.Select(m => m.AppUser.Name)),

                Status = created.Status.ToString(),

                Boards = created.Boards.Select(b => new BoardDto
                {
                    Id = b.Id,

                    Name = b.Name,
                    ProjectId = b.ProjectId,
                    TaskItems = b.TaskItems.Select(t => new TaskItemDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        BoardId = t.BoardId,
                        Order = t.Order,
                        SprintId = t.SprintId,
                        TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                        {
                            TaskItemId = a.TaskItemId,
                            AppUserId = a.AppUserId,


                        }).ToList()
                    }).ToList()
                }).ToList()



            };


        }

        //public async Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto projectDto)
        //{
        //    var project = new Project
        //    {
        //        Id = id,
        //        Name = projectDto.Name,
        //        Description = projectDto.Description,
        //        Priority = projectDto.Priority,
        //        Visibility = projectDto.Visibility,

        //        Status = Enum.TryParse<ProjectStatus>(projectDto.Status, true, out var status)
        //            ? status
        //            : ProjectStatus.Active,
        //        Managers = new List<AppUser>()


        //    };

        //    var updated = await taskRepository.UpdateProjectAsync(id, project, projectDto);
        //    return new ProjectDto
        //    {
        //        Id = updated.Id,
        //        Name = updated.Name,
        //        Description = updated.Description,
        //        Priority = updated.Priority,
        //        Visibility = updated.Visibility,
        //        CreatedDate = updated.CreatedDate,
        //        Progress = updated.Progress,
        //        MemberCount = updated.TeamMembers.Count,
        //        MemberIntials = updated.TeamMembers
        //            .Select(u => string.Join("", u.Name.Split(' ')
        //                .Select(n => n[0].ToString().ToUpper())))
        //            .Distinct()
        //            .ToList(),
        //        Status = updated.Status.ToString(),
        //        ManagerIds = updated.Managers.Select(m => m.AppUserId).ToList(),
        //        ManagerNames = string.Join(", ", updated.Managers.Select(m => m.AppUser.Name)),
        //        Boards = updated.Boards.Select(b => new BoardDto
        //        {
        //            Id = b.Id,
        //            Name = b.Name,
        //            ProjectId = b.ProjectId,
        //            TaskItems = b.TaskItems.Select(t => new TaskItemDto
        //            {
        //                Id = t.Id,
        //                Title = t.Title,
        //                Description = t.Description,
        //                DueDate = t.DueDate,
        //                BoardId = t.BoardId,
        //                Order = t.Order,
        //                SprintId = t.SprintId,
        //                TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
        //                {
        //                    TaskItemId = a.TaskItemId,
        //                    AppUserId = a.AppUserId,

        //                }).ToList()
        //            }).ToList()
        //        }).ToList()
        //    };
        //}


        public async Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto projectDto)
        {


            var existingProject = await taskRepository.GetProjectByIdAsync(id);

            if (existingProject == null)
                throw new Exception("Project not found");


            existingProject.Name = projectDto.Name;
            existingProject.Description = projectDto.Description;
            existingProject.Priority = projectDto.Priority;
            existingProject.Visibility = projectDto.Visibility;

            existingProject.Status = Enum.TryParse<ProjectStatus>(projectDto.Status, true, out var status)
                ? status
                : ProjectStatus.Active;


            var updated = await taskRepository.UpdateProjectAsync(id, existingProject, projectDto);
           
            return new ProjectDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                Priority = updated.Priority,
                Visibility = updated.Visibility,
                CreatedDate = updated.CreatedDate,
                
                Progress = updated.Progress,
                MemberCount = updated.ProjectMembers.Count,
                MemberIntials = updated.ProjectMembers
    .Where(u => !string.IsNullOrWhiteSpace(u.AppUser.Name))
    .Select(u =>
    {
        var parts = u.AppUser.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(p => char.ToUpper(p[0])));
    })
    .Distinct()
    .ToList(),

                Status = updated.Status.ToString(),
                ManagerIds = updated.Managers.Select(m => m.AppUserId).ToList(),
                ManagerNames = string.Join(", ", updated.Managers.Select(m => m.AppUser.Name)),

                Boards = updated.Boards.Select(b => new BoardDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    ProjectId = b.ProjectId,

                    TaskItems = b.TaskItems.Select(t => new TaskItemDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate ?? DateTime.UtcNow.AddDays(1),
                        BoardId = t.BoardId,
                        Order = t.Order,
                        IsCompleted = t.IsCompleted,
                        CompletedDate = t.CompletedDate,
                        Priority = string.IsNullOrWhiteSpace(t.Priority) ? "Low" : t.Priority,
                        SprintId = t.SprintId,

                        TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                        {
                            AppUserId = a.AppUserId,
                            TaskItemId = a.TaskItemId
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {

            return await taskRepository.DeleteProjectAsync(id);
        }



        }
}

