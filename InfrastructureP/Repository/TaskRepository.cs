using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
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

       public async Task<IEnumerable<Project>> GetAllProjectsAsync(string currentUserId,bool isAdmin,string? filter="All")
        {

            int UserId = int.Parse(currentUserId);
            //int UserId = currentUserId;

            IQueryable<Project> query = _taskDbContext.projects
               
                .Include(x => x.Boards)
                .ThenInclude(b => b.TaskItems)
                .ThenInclude(t => t.TaskAssignments)
                .ThenInclude(a => a.AppUser);

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
                    // "All" → no filter applied
                    break;
            }
            if (!isAdmin)
            {
                query = query.Where(p =>
            
                 p.Boards.Any(b =>
                   b.TaskItems.Any(t =>
                     t.TaskAssignments.Any(a => a.AppUserId == UserId)))
);
            }

            //var data = await _taskDbContext.projects.Include(x => x.Boards)
            //    .ThenInclude(b=>b.TaskItems)
            //    .ThenInclude(t=>t.TaskAssignments)
            //    .ThenInclude(a=>a.AppUser).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            var data = await _taskDbContext.projects
                .Include(x => x.Boards)
                .ThenInclude(b => b.TaskItems)
                    .ThenInclude(t => t.TaskAssignments)
                        .ThenInclude(a => a.AppUser)
                        
                        .FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }
        public async Task<Project> AddProjectAsync(Project project)
        {
            await _taskDbContext.projects.AddAsync(project);
            await _taskDbContext.SaveChangesAsync();
            return project;
        }
        public async Task<Project> UpdateProjectAsync(int id, Project project)

        {
            var existingProject = await _taskDbContext.projects.Include(x => x.Boards)
                                                                
                                                                    .ThenInclude(b => b.TaskItems)
                                                                    .ThenInclude(t => t.TaskAssignments)
                                                                            .ThenInclude(a => a.AppUser)
                                                                      .FirstOrDefaultAsync(x => x.Id == id);
            if (existingProject == null)
            {
                throw new Exception("Project not found");
            }
                existingProject.Name = project.Name;
            existingProject.CreatedDate = project.CreatedDate;
            existingProject.Priority = project.Priority;
            existingProject.Status = project.Status;


            // Optionally handle Boards updates (if you want to replace boards)
            //existingProject.Boards = project.Boards;

            foreach (var board in project.Boards)
            {
                var existingBoard = existingProject.Boards.FirstOrDefault(b => b.Id == board.Id);
                if (existingBoard != null)
                {
                    existingBoard.Name = board.Name;
                
                foreach (var task in board.TaskItems)
                {
                    var existingTask = existingBoard.TaskItems.FirstOrDefault(t => t.Id == task.Id);

                    if (existingTask != null)
                    {
                        // Update existing task
                        existingTask.Title = task.Title;
                        existingTask.Description = task.Description;

                        existingTask.DueDate = task.DueDate;
                        existingTask.Order = task.Order;
                    }
                    else
                    {
                        // 🆕 Add new task to board
                        existingBoard.TaskItems.Add(task);
                    }
                }

                // (Optional) 🗑 Remove deleted tasks
                var tasksToRemove = existingBoard.TaskItems
                    .Where(et => !board.TaskItems.Any(nt => nt.Id == et.Id))
                    .ToList();

                foreach (var taskToRemove in tasksToRemove)
                {
                    _taskDbContext.Remove(taskToRemove);
                }

            }
                    else
                {
                    // 🆕 new board added
                    existingProject.Boards.Add(board);
                }
            }


            await _taskDbContext.SaveChangesAsync();
                return existingProject;
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
        }
    }

