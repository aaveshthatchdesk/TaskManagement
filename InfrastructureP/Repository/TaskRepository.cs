using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;

namespace Task.Infrastructure.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public TaskRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }

        public async Task<PagedResult<Project>> GetAllProjectsPagedAsync(string currentUserId, string role, string? filter, int page, int pageSize, string? search, int? managerId,int? memberId, DateTime? createdDate, DateTime? startDate, DateTime? endDate)
        {

            int UserId = int.Parse(currentUserId);


            IQueryable<Project> query = _taskDbContext.projects
                .Include(p => p.Managers)
                   .ThenInclude(m => m.AppUser)
                 .Include(p => p.ProjectMembers)
                     .ThenInclude(m => m.AppUser);
                //.Include(x => x.Boards)
                //.ThenInclude(b => b.TaskItems)
                //.ThenInclude(t => t.TaskAssignments)
                //.ThenInclude(a => a.AppUser);

            switch (filter?.ToLower())
            {
                case "active":
                    query = query.Where(p => p.Status == ProjectStatus.Active);
                    break;
                case "completed":
                    query = query.Where(p => p.Status == ProjectStatus.Completed);
                    break;
                case "archived":
                    query = query.Where(p => p.Status == ProjectStatus.Archeived);
                    break;
                default:

                    break;
            }
            if (role == "Admin")
            {


                if (managerId.HasValue)
                    query = query.Where(p => p.Managers.Any(m => m.AppUserId == managerId.Value));
            }
            else if (role == "Manager")
            {
               
                if (memberId.HasValue)
                    query = query.Where(p => p.ProjectMembers.Any(m => m.AppUserId == memberId.Value));

                
                query = query.Where(p => p.Managers.Any(m => m.AppUserId == UserId));
            }
            else if (role == "Member")
            {
                // Members cannot filter by ID → no managerId or memberId accepted
                // They should only see projects they are part of
                query = query.Where(p => p.ProjectMembers.Any(m => m.AppUserId == UserId));
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(s) ||
                  p.Description.ToLower().Contains(s));
            }

            if (createdDate.HasValue)
            {
                DateTime date = createdDate.Value.Date;
                query = query.Where(p => p.CreatedDate.Value.Date == date);
            }

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedDate.Value.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedDate.Value.Date <= endDate.Value.Date);

      //      if (!isAdmin)
      //      {

      //          query = query.Where(p => p.ProjectMembers.Any(m => m.AppUserId == UserId));

      //          query = query.Where(p =>
      //    p.Managers.Any(m => m.AppUserId == UserId) ||
      //    p.ProjectMembers.Any(m => m.AppUserId == UserId)
      //);
               

      //      }        
            int totalCount = await query.CountAsync();

            var items = await query.OrderByDescending(p => p.CreatedDate)
                                                  .ThenByDescending(p => p.Id)
                                                  .Skip((page - 1) * pageSize)
                                                  .Take(pageSize)
                                                  .ToListAsync();


            return new PagedResult<Project>
            {
                Items = items,
                TotalCount = totalCount,
            };

        }

        public async Task<bool> AssignMembersAsync(int projectId,List<int>memberIds)
        {
            var project = await _taskDbContext.projects
           .Include(p => p.ProjectMembers)
           .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new Exception("Project not found");

            //// Clear old member list
            //project.ProjectMembers.Clear();

            // Get all users to add
            var users = await _taskDbContext.appUsers
                .Where(u => memberIds.Contains(u.Id))
                .ToListAsync();

            // Add new members
            foreach (var user in users)
            {
                project.ProjectMembers.Add(
                  new ProjectMember
                  {
                      ProjectId = project.Id,
                      AppUserId = user.Id,
                      AppUser = user
                  }
                    );
            }

            await _taskDbContext.SaveChangesAsync();
            return true;
        }
       // public async Task<List<ProjectMember>> GetProjectMembersAssignedAsync(int projectId)
       // {
       //     return await _taskDbContext.ProjectMembers
       //.Where(pm => pm.ProjectId == projectId)
       //.Include(pm => pm.AppUser)  

       //.ToListAsync();
       // }


        public async Task<Project> GetProjectByIdAsync(int id)
        {
            var data = await _taskDbContext.projects
                .Include(p => p.Managers)
                  .ThenInclude(m=>m.AppUser)
                .Include(x => x.Boards)
                .ThenInclude(b => b.TaskItems)
                    .ThenInclude(t => t.TaskAssignments)
                        .ThenInclude(a => a.AppUser)
                        
                        .FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        //public async Task<List<AppUser>> GetProjectMembersAsync(int projectId)
        //{
        //    return await _taskDbContext.tasks
        //        .Where(t => t.Board.ProjectId == projectId)
        //        .SelectMany(t => t.TaskAssignments)
        //        .Select(a => a.AppUser)
        //        .Where(u => u.Role != "Manager") 
        //        .Distinct()
        //        .ToListAsync();
        //}
        public async Task<List<AppUser>> GetProjectMembersAsync(int projectId)
        {
            return await _taskDbContext.ProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Include(pm => pm.AppUser)
                .Select(pm => pm.AppUser)
                .ToListAsync();
        }

        public async Task<List<AppUser>> GetManagersByProjectAsync(int projectId)
        {
            //return await _taskDbContext.projects
            //    .Where(p => p.Id == projectId)
            //    .Include(p => p.Managers)
            //    .SelectMany(p => p.Managers)
            //    .ToListAsync();
            return await _taskDbContext.projects
       .Where(p => p.Id == projectId)
       .Include(p => p.Managers)
           .ThenInclude(pm => pm.AppUser)   
       .SelectMany(p => p.Managers)
       .Select(pm => pm.AppUser)            
       .ToListAsync();
        }


        public async Task<Project> AddProjectAsync(Project project)
        {
            //foreach (var manager in project.Managers)
            //{
            //    _taskDbContext.Attach(manager);
            //}
            try
            {
                foreach (var manager in project.Managers)
                {
                    _taskDbContext.Entry(new AppUser { Id = manager.AppUserId })
                        .State = EntityState.Unchanged;
                }
                _taskDbContext.projects.Add(project);
                await _taskDbContext.SaveChangesAsync();
                return project;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Error: " + ex);
                throw;
            }
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            _taskDbContext.projects.Add(project);
            await _taskDbContext.SaveChangesAsync();
            return project;
        }

        //public async Task<Project> UpdateProjectAsync(int id, Project project, ProjectDto projectDto)
        //{
        //    var existingProject = await _taskDbContext.projects
        //        .Include(p => p.Managers)
        //        .Include(p => p.Boards)
        //            .ThenInclude(b => b.TaskItems)
        //                .ThenInclude(t => t.TaskAssignments)
        //        .Include(p => p.Boards)
        //            .ThenInclude(b => b.TaskItems)
        //                .ThenInclude(t => t.Sprint)
        //        .FirstOrDefaultAsync(x => x.Id == id);

        //    if (existingProject == null)
        //        throw new Exception("Project not found");


        //    existingProject.Name = project.Name;
        //    existingProject.Description = project.Description;
        //    existingProject.Visibility = project.Visibility;
        //    existingProject.Priority = project.Priority;
        //    existingProject.Status = project.Status;


        //    existingProject.Managers.Clear();

        //    if (projectDto.ManagerIds != null && projectDto.ManagerIds.Any())
        //    {
        //        existingProject.Managers = projectDto.ManagerIds
        //            .Select(id => new ProjectManager
        //            {
        //                ProjectId = existingProject.Id,
        //                AppUserId = id
        //            })
        //            .ToList();
        //    }
        //   existingProject.Managers.Add(m);


        //    foreach (var boardDto in projectDto.Boards ?? new List<BoardDto>())
        //    {
        //        var board = existingProject.Boards.FirstOrDefault(b => b.Id == boardDto.Id);


        //        if (board == null)
        //        {
        //            board = new Board
        //            {
        //                Name = boardDto.Name,
        //                ProjectId = existingProject.Id,
        //                TaskItems = new List<TaskItem>()
        //            };

        //            existingProject.Boards.Add(board);
        //            continue;
        //        }


        //        board.Name = boardDto.Name;


        //        var incomingTaskIds = boardDto.TaskItems?.Where(t => t.Id != 0).Select(t => t.Id).ToList()
        //                                ?? new List<int>();


        //        var tasksToRemove = board.TaskItems
        //            .Where(et => !incomingTaskIds.Contains(et.Id))
        //            .ToList();

        //        foreach (var tRemove in tasksToRemove)
        //        {
        //            _taskDbContext.taskAssignments.RemoveRange(tRemove.TaskAssignments);
        //            _taskDbContext.tasks.Remove(tRemove);
        //        }


        //        foreach (var taskDto in boardDto.TaskItems ?? new List<TaskItemDto>())
        //        {
        //            var existingTask = board.TaskItems.FirstOrDefault(t => t.Id == taskDto.Id);


        //            if (existingTask == null)
        //            {
        //                existingTask = new TaskItem
        //                {
        //                    Title = taskDto.Title,
        //                    Description = taskDto.Description,
        //                    DueDate = taskDto.DueDate,
        //                    SprintId = taskDto.SprintId,
        //                    Order = taskDto.Order,
        //                    Priority=taskDto.Priority??"Medium",

        //                    BoardId = board.Id,
        //                    TaskAssignments = new List<TaskAssignment>()
        //                };
        //                _taskDbContext.tasks.Add(existingTask);
        //                board.TaskItems.Add(existingTask);
        //            }


        //            existingTask.Title = taskDto.Title;
        //            existingTask.Description = taskDto.Description;
        //            existingTask.DueDate = taskDto.DueDate;
        //            existingTask.Order = taskDto.Order;
        //            existingTask.SprintId = taskDto.SprintId;
        //            existingTask.Priority = taskDto.Priority ?? existingTask.Priority;
        //            existingTask.BoardId = board.Id;


        //            _taskDbContext.taskAssignments.RemoveRange(existingTask.TaskAssignments);
        //            existingTask.TaskAssignments.Clear();

        //            foreach (var assignDto in taskDto.TaskAssignments ?? new List<TaskAssignmentDto>())
        //            {
        //                existingTask.TaskAssignments.Add(new TaskAssignment
        //                {
        //                    AppUserId = assignDto.AppUserId

        //                });
        //            }
        //        }
        //    }

        //    await _taskDbContext.SaveChangesAsync();
        //    return existingProject;
        //}
        public async Task<Project> UpdateProjectAsync(int id, Project project, ProjectDto projectDto)
        {
            var existingProject = await _taskDbContext.projects
                .Include(p=>p.Sprint)
                .Include(p => p.Managers)
                .Include(p => p.Boards)
                    .ThenInclude(b => b.TaskItems)
                        .ThenInclude(t => t.TaskAssignments)
                          
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingProject == null)
                throw new Exception("Project not found");


            //     var projectSprint = await _taskDbContext.projects
            //.Where(p => p.Id == existingProject.Id)
            //.Include(p => p.Sprint)
            //.Select(p => p.Sprint)
            //.FirstOrDefaultAsync();
            var projectSprint = existingProject.Sprint;


            // -----------------------------
            // Update basic project fields
            // -----------------------------
            existingProject.Name = project.Name;
            existingProject.Description = project.Description;
            existingProject.Priority = project.Priority;
            existingProject.Visibility = project.Visibility;
            existingProject.Status = project.Status;

            // -----------------------------
            // Update Managers
            // -----------------------------
            existingProject.Managers.Clear();

            if (projectDto.ManagerIds != null)
            {
                foreach (var managerId in projectDto.ManagerIds)
                {
                    existingProject.Managers.Add(new ProjectManager
                    {
                        ProjectId = existingProject.Id,
                        AppUserId = managerId
                    });
                }
            }
            var incomingBoardIds = projectDto.Boards?
    .Where(b => b.Id != 0)
    .Select(b => b.Id)
    .ToList() ?? new();

            var boardsToRemove = existingProject.Boards
                .Where(b => !incomingBoardIds.Contains(b.Id))
                .ToList();

            // Delete entire boards + their tasks
            foreach (var board in boardsToRemove)
            {
                foreach (var task in board.TaskItems.ToList())
                {
                    // Remove task assignments
                    _taskDbContext.taskAssignments.RemoveRange(task.TaskAssignments);

                    // Remove task
                    _taskDbContext.tasks.Remove(task);
                }

                _taskDbContext.boards.Remove(board);
            }

            // -----------------------------
            // Update Boards & Tasks
            // -----------------------------
            foreach (var boardDto in projectDto.Boards ?? new List<BoardDto>())
            {
                var board = existingProject.Boards.FirstOrDefault(b => b.Id == boardDto.Id);

                // Create new board
                if (board == null)
                {
                    board = new Board
                    {
                        Name = boardDto.Name,
                        ProjectId = existingProject.Id,
                        TaskItems = new List<TaskItem>()
                    };

                    _taskDbContext.boards.Add(board);
                    existingProject.Boards.Add(board);
                    //await _taskDbContext.SaveChangesAsync();
                }

                // Update board name
                board.Name = boardDto.Name;

                // -----------------------------
                // Remove deleted tasks
                // -----------------------------
                var incomingIds = boardDto.TaskItems?.Where(t => t.Id != 0).Select(t => t.Id).ToList() ?? new();

                var tasksToRemove = board.TaskItems.Where(t => !incomingIds.Contains(t.Id)).ToList();

                foreach (var removeTask in tasksToRemove)
                {
                    _taskDbContext.taskAssignments.RemoveRange(removeTask.TaskAssignments);
                    _taskDbContext.tasks.Remove(removeTask);
                }

                // -----------------------------
                // Add or update tasks
                // -----------------------------
                foreach (var taskDto in boardDto.TaskItems ?? new List<TaskItemDto>())
                {
                    TaskItem existingTask = null;
                    existingTask = board.TaskItems.FirstOrDefault(t => t.Id == taskDto.Id);
                    bool isNewTask=false;
                    //if (taskDto.Id != 0)
                    //{
                    //    existingTask = board.TaskItems.FirstOrDefault(t => t.Id == taskDto.Id);
                        
                    //}

                    // Insert new task
                    if (existingTask == null)
                    {
                        existingTask = new TaskItem
                        {
                            Title = taskDto.Title,
                            Description = taskDto.Description,
                            DueDate = taskDto.DueDate,
                            SprintId = projectSprint?.Id,
                            Order = taskDto.Order,
                            
                            Priority = taskDto.Priority ?? "Low",
                            BoardId = board.Id,
                            TaskAssignments = new List<TaskAssignment>()
                        };

                        _taskDbContext.tasks.Add(existingTask);
                        board.TaskItems.Add(existingTask);
                        isNewTask= true;
                    }

                    // Update task
                    existingTask.Title = taskDto.Title;
                    existingTask.Description = taskDto.Description;
                    existingTask.DueDate = taskDto.DueDate ?? DateTime.UtcNow.AddDays(1);
                    existingTask.Order = taskDto.Order;
                    existingTask.SprintId = projectSprint?.Id;
                    existingTask.IsCompleted = taskDto.IsCompleted;
                    existingTask.CompletedDate=taskDto.CompletedDate;
                    existingTask.Priority = taskDto.Priority ?? existingTask.Priority;
                    existingTask.BoardId = board.Id;

                    if (isNewTask)
                        await _taskDbContext.SaveChangesAsync();

                    // -----------------------------
                    // Update TaskAssignments
                    // -----------------------------


                    _taskDbContext.taskAssignments.RemoveRange(existingTask.TaskAssignments);
                        existingTask.TaskAssignments.Clear();
                    
                    if(existingTask.Id==0)
                    {
                        await _taskDbContext.SaveChangesAsync();
                    }

                    foreach (var assignment in taskDto.TaskAssignments ?? new List<TaskAssignmentDto>())
                    {
                        existingTask.TaskAssignments.Add(new TaskAssignment
                        {
                            AppUserId = assignment.AppUserId,
                            TaskItemId = existingTask.Id
                        });

                        //var taskAssignment = new TaskAssignment
                        //{
                        //    AppUserId = assignment.AppUserId,
                        //    TaskItemId = existingTask.Id
                        //};
                        //_taskDbContext.taskAssignments.Add(taskAssignment);

                    }
                }
            }

            // -----------------------------
            // Save all changes
            // -----------------------------
            await _taskDbContext.SaveChangesAsync();

            return await _taskDbContext.projects
    .Include(p => p.Managers)
        .ThenInclude(m => m.AppUser)
    .Include(p => p.ProjectMembers)
        .ThenInclude(pm => pm.AppUser)
    .Include(p => p.Boards)
        .ThenInclude(b => b.TaskItems)
            
            .ThenInclude(t => t.TaskAssignments)
           
    .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<bool> DeleteProjectAsync(int id)
        {

            var existingProject = await _taskDbContext.projects.Include(x => x.Boards)
                                                                    .Include(p => p.Boards)
                                                                      .ThenInclude(b => b.TaskItems)
                                                                          .FirstOrDefaultAsync(x => x.Id == id);
            if (existingProject == null)
            {
                throw new Exception("Project not found");
            }
            _taskDbContext.Remove(existingProject);
            await _taskDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<AppUser>> GetUsersByIdsAsync(List<int> ids)
        {
            return await _taskDbContext.appUsers
                .Where(u => ids.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Project>> GetAllProjects()
        {
            return await _taskDbContext.projects
                .Select(p => new Project { Id = p.Id, Name = p.Name })
                .ToListAsync();
        }
        public async Task<bool> AssignProjectsToManagerAsync(int managerId, List<int> projectIds)
        {
            var managerExists = await _taskDbContext.appUsers
                .AnyAsync(u => u.Id == managerId);

            if (!managerExists)
                throw new Exception("Manager not found");

            var existingLinks = await _taskDbContext.ProjectManagers
                .Where(pm => pm.AppUserId == managerId && projectIds.Contains(pm.ProjectId))
                .Select(pm => pm.ProjectId)
                .ToListAsync();

            var newLinks = projectIds
                .Except(existingLinks)
                .Select(pid => new ProjectManager
                {
                    AppUserId = managerId,
                    ProjectId = pid
                });

            await _taskDbContext.ProjectManagers.AddRangeAsync(newLinks);
            await _taskDbContext.SaveChangesAsync();

            return true;
        }


    }
}

