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

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(string currentUserId, bool isAdmin,string? filter = "All")
        {
            var project = await taskRepository.GetAllProjectsAsync(currentUserId,isAdmin,filter);

            return project.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Status = p.Status.ToString(),
                CreatedDate=p.CreatedDate,
                Priority=p.Priority,
                Visibility=p.Visibility,
                Progress=p.Progress,
                MemberCount = p.TeamMembers?.Count(u => u != null) ?? 0,
                MemberIntials = p.TeamMembers?
    .Where(u => u != null && !string.IsNullOrWhiteSpace(u.Name))
    .Select(u => u.Name.Trim()[0].ToString().ToUpper())
    .Distinct()
    .ToList() ?? new List<string>(),
                //MemberCount=p.TeamMembers.Count,
                //MemberIntials = p.TeamMembers
                //    .Where(u => !string.IsNullOrWhiteSpace(u.Name))
                //    .Select(u => string.Join("", u.Name.Split(' ')
                //        .Where(n => !string.IsNullOrWhiteSpace(n))
                //        .Select(n => n[0].ToString().ToUpper())))
                //    .Distinct()
                //    .Take(3) // optional: limit display to 3 initials
                //    .ToList(),

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
                            AppUser = new AppUserDto
                            {
                                Id = a.AppUser.Id,
                                Name = a.AppUser.Name,
                                Email = a.AppUser.Email
                            }
                        }).ToList()
                    }).ToList()
                }).ToList()


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
                Priority = p.Priority,
                CreatedDate = p.CreatedDate,
                Progress = p.Progress,
                MemberCount = p.TeamMembers?.Count(u => u != null) ?? 0,
                MemberIntials = p.TeamMembers?
    .Where(u => u != null && !string.IsNullOrWhiteSpace(u.Name))
    .Select(u => u.Name.Trim()[0].ToString().ToUpper())
    .Distinct()
    .ToList() ?? new List<string>(),
                //MemberCount = p.TeamMembers.Count,
                //MemberIntials = p.TeamMembers
                //    .Where(u => !string.IsNullOrWhiteSpace(u.Name))
                //    .Select(u => string.Join("", u.Name.Split(' ')
                //        .Where(n => !string.IsNullOrWhiteSpace(n))
                //        .Select(n => n[0].ToString().ToUpper())))
                //    .Distinct()
                //    .ToList(),
                Status = p.Status.ToString(),
                Boards = p.Boards.Select(b => new BoardDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    ProjectId=b.ProjectId,
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
                            AppUser = new AppUserDto
                            {
                                Id = a.AppUser.Id,
                                Name = a.AppUser.Name,
                                Email = a.AppUser.Email
                            }
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
                Priority = projectDto.Priority,
                Visibility = projectDto.Visibility,
                CreatedDate = DateTime.Now,
                Status = Enum.TryParse<ProjectStatus>(projectDto.Status, true, out var status)
                    ? status
                    : ProjectStatus.Active,


                Boards = projectDto.Boards.Select(b => new Board
                {
                    Name = b.Name,

                    TaskItems = b.TaskItems.Select(t => new TaskItem
                    {
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Order = t.Order,
                        SprintId=t.SprintId,
                        TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignment
                        {
                            AppUserId = a.AppUserId
                        }).ToList()
                    }).ToList()
                }).ToList()

            };
            var created = await taskRepository.AddProjectAsync(project);

            return new ProjectDto
            {
                Id = created.Id,
                Name = created.Name,
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
                            AppUser = a.AppUser == null ? null : new AppUserDto
                            {
                                Id = a.AppUser.Id,
                                Name = a.AppUser.Name,
                                Email = a.AppUser.Email
                            }

                        }).ToList()
                    }).ToList()
                }).ToList()



            };


        }

        public async Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto projectDto)
        {
            var project = new Project
            {
                Id = id,
                Name = projectDto.Name,
                Priority = projectDto.Priority,
                CreatedDate = projectDto.CreatedDate,
                Status = Enum.TryParse<ProjectStatus>(projectDto.Status, true, out var status)
                    ? status
                    : ProjectStatus.Active,
                Boards = projectDto.Boards.Select(b => new Board
                {
                    Id = b.Id,
                    Name = b.Name,
                    TaskItems = b.TaskItems.Select(t => new TaskItem
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        Order = t.Order,
                        SprintId=t.SprintId,
                        TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignment
                        {
                            AppUserId = a.AppUserId,
                            TaskItemId = a.TaskItemId
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
            var updated = await taskRepository.UpdateProjectAsync(id, project);
            return new ProjectDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Priority = updated.Priority,
                CreatedDate = updated.CreatedDate,
                Progress = updated.Progress,
                MemberCount = updated.TeamMembers.Count,
                MemberIntials = updated.TeamMembers
                    .Select(u => string.Join("", u.Name.Split(' ')
                        .Select(n => n[0].ToString().ToUpper())))
                    .Distinct()
                    .ToList(),
                Status = updated.Status.ToString(),
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
                        DueDate = t.DueDate,
                        BoardId = t.BoardId,
                        Order = t.Order,
                        SprintId = t.SprintId,
                        TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                        {
                            TaskItemId = a.TaskItemId,
                            AppUserId = a.AppUserId,
                            AppUser = a.AppUser != null ? new AppUserDto
                            {
                                Id = a.AppUser.Id,
                                Name = a.AppUser.Name,
                                Email = a.AppUser.Email
                            } : null
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

