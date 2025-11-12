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
    public class SprintService:ISprintService
    {
        private readonly ISprintRepository _sprintRepository;

        public SprintService(ISprintRepository sprintRepository)
        {
            _sprintRepository = sprintRepository;
        }

        public async Task<IEnumerable<SprintDto>> GetAllAsync()
        {
            var sprints = await _sprintRepository.GetAllAsync();
            return sprints
                .Select(s => new SprintDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    TaskItems = s.TaskItems.Select(t => new TaskItemDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        DueDate = t.DueDate,
                        BoardId = t.BoardId,
                        SprintId = t.SprintId,
                        Order = t.Order,
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
                    }).ToList(),
                    Boards = s.TaskItems
                   .Where(t => t.Board != null)
                   .Select(t => t.Board!)
                    .GroupBy(b => b.Id)
                     .Select(g => new BoardDto
                     {
                         Id = g.Key,
                         Name = g.First().Name
                     }).ToList(),

                    Projects = s.TaskItems
                    .Where(t => t.Board?.Project != null)
                     .Select(t => t.Board!.Project!)
                          .GroupBy(p => p.Id)
                          .Select(g => new ProjectDto
                          {
                              Id = g.Key,
                              Name = g.First().Name
                          })
                             .ToList(),


                    ProjectName = s.TaskItems
                 .Where(t => t.Board?.Project != null)
                    .Select(t => t.Board!.Project!.Name)
    .Distinct()
    .DefaultIfEmpty("No Project")
    .Aggregate((a, b) => a + ", " + b),
                    TotalTasks = s.TaskItems.Count,
                    CompletedTasks = s.TaskItems.Count(t => t.IsCompleted),
                    AssignedUsers = s.TaskItems
            .SelectMany(t => t.TaskAssignments.Select(a => a.AppUser))
            .DistinctBy(u => u.Id)
            .Select(u => new AppUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            }).ToList(),
                    Status = s.TaskItems.All(t => t.IsCompleted) ? "Completed" : "Active"
                });


        }

        public async Task<SprintStatsDto> GetsSprintsStats()
        {
            var allSprints= await _sprintRepository.GetAllAsync();
            if (allSprints == null || !allSprints.Any())
                if (allSprints == null || !allSprints.Any())
                return new SprintStatsDto();

            var activeSprints = allSprints.Count(s => s.EndDate >= DateTime.UtcNow);
            var completedSprints = allSprints.Count(s => s.EndDate < DateTime.UtcNow);
            var tasksCompleted = allSprints.Sum(s => s.TaskItems?.Count(t => t.IsCompleted)??0);

            // Example: average velocity (you can adjust based on your logic)
            var avgVelocity = allSprints.Any() ?
                (int)Math.Round(allSprints.Average(s => s.TaskItems?.Count() ?? 0)) : 0;
            return new SprintStatsDto
            {
                ActiveSprints = activeSprints,
                CompletedSprints = completedSprints,
                TaskCompleted = tasksCompleted,
                AverageVelocity = avgVelocity
            };
        }
        public async Task<SprintDto?>GetSprintByIdAsync(int id)
        {
            var sprints = await _sprintRepository.GetSprintByIdAsync(id);
            return new SprintDto
            {
                Id = sprints.Id,
                Name = sprints.Name,
                StartDate = sprints.StartDate,
                EndDate = sprints.EndDate,
                TaskItems = sprints.TaskItems.Select(t => new TaskItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    BoardId = t.BoardId,
                    SprintId = t.SprintId,
                    Order = t.Order,
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
                }).ToList(),
                Boards=sprints.TaskItems
                   .Where(t => t.Board != null)
                   .Select(t => t.Board!)
                    .GroupBy(b => b.Id)
                     .Select(g => new BoardDto
                     {
                         Id = g.Key,
                         Name = g.First().Name
                     }).ToList(),

                       Projects = sprints.TaskItems
                    .Where(t => t.Board?.Project != null)
                     .Select(t => t.Board!.Project!)
                          .GroupBy(p => p.Id)
                          .Select(g => new ProjectDto
                           {
                           Id = g.Key,
                          Name = g.First().Name
                               })
                             .ToList()
            };

        }
        public async Task<SprintDto> AddAsync(SprintDto dto)
        {
            var sprint = new Sprint
            {

                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                //TaskItems = dto.TaskItems.Select(t => new TaskItem
                //{
                //    Id = t.Id,
                //    Title = t.Title,
                //    Description = t.Description,
                //    DueDate = t.DueDate,
                //    BoardId = t.BoardId,
                //    SprintId = t.SprintId,
                //    Order = t.Order,
                //    TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignment
                //    {
                //        TaskItemId = a.TaskItemId,
                //        AppUserId = a.AppUserId,
                //        AppUser = new AppUser
                //        {
                //            Id = a.AppUser.Id,
                //            Name = a.AppUser.Name,
                //            Email = a.AppUser.Email
                //        }
                //    }).ToList()
                //}).ToList()
            };



            var created = await _sprintRepository.AddAsync(sprint);
            return new SprintDto
            {

                Id = created.Id,
                Name = created.Name,
                StartDate = created.StartDate,
                EndDate = created.EndDate,
                //TaskItems = created.TaskItems.Select(t => new TaskItemDto
                //{
                //    Id = t.Id,
                //    Title = t.Title,
                //    Description = t.Description,
                //    DueDate = t.DueDate,
                //    BoardId = t.BoardId,
                //    SprintId = t.SprintId,
                //    Order = t.Order,
                //    TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                //    {
                //        TaskItemId = a.TaskItemId,
                //        AppUserId = a.AppUserId,
                //        AppUser = new AppUserDto
                //        {
                //            Id = a.AppUser.Id,
                //            Name = a.AppUser.Name,
                //            Email = a.AppUser.Email
                //        }
                //    }).ToList()
                //}).ToList()
          

            };
            

        }

        public async Task<SprintDto?> UpdateAsync(int id,SprintDto dto)
        {
           

            var sprint = new Sprint
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                //TaskItems = dto.TaskItems.Select(t => new TaskItem
                //{
                //    Id = t.Id,
                //    Title = t.Title,
                //    Description = t.Description,
                //    DueDate = t.DueDate,
                //    Order = t.Order,
                //    TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignment
                //    {
                //        AppUserId = a.AppUserId,
                //        TaskItemId = a.TaskItemId
                //    }).ToList()
                //}).ToList()
          
                };



           var updated= await _sprintRepository.UpdateAsync(id,sprint);
            return new SprintDto
            {

                Id = updated.Id,
                Name = updated.Name,
                StartDate = updated.StartDate,
                EndDate = updated.EndDate,
                //TaskItems = updated.TaskItems.Select(t => new TaskItemDto
                //{
                //    Id = t.Id,
                //    Title = t.Title,
                //    Description = t.Description,
                //    DueDate = t.DueDate,
                //    BoardId = t.BoardId,
                //    SprintId = t.SprintId,
                //    Order = t.Order,
                //    TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                //    {
                //        TaskItemId = a.TaskItemId,
                //        AppUserId = a.AppUserId,
                //        AppUser = new AppUserDto
                //        {
                //            Id = a.AppUser.Id,
                //            Name = a.AppUser.Name,
                //            Email = a.AppUser.Email
                //        }
                //    }).ToList()
                //}).ToList()


            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
            

           return await _sprintRepository.DeleteAsync(id);
            
        }




    }
}
