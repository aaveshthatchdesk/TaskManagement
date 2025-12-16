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

        //    public async Task<IEnumerable<SprintDto>> GetAllAsync()
        //    {
        //        var sprints = await _sprintRepository.GetAllAsync();
        //        return sprints
        //            .Select(s => new SprintDto
        //            {
        //                Id = s.Id,
        //                Name = s.Name,
        //                StartDate = s.StartDate,
        //                EndDate = s.EndDate,
        //                TaskItems = s.TaskItems.Select(t => new TaskItemDto
        //                {
        //                    Id = t.Id,
        //                    Title = t.Title,
        //                    Description = t.Description,
        //                    DueDate = t.DueDate,
        //                    BoardId = t.BoardId,
        //                    SprintId = t.SprintId,
        //                    Order = t.Order,
        //                    TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
        //                    {
        //                        TaskItemId = a.TaskItemId,
        //                        AppUserId = a.AppUserId,

        //                    }).ToList()
        //                }).ToList(),

        //                TotalTasks = s.TaskItems.Count,
        //                CompletedTasks = s.TaskItems.Count(t => t.IsCompleted),



        //                Status =
        //s.TaskItems.Count(t => t.IsCompleted) == 0
        //    ? "Planned"
        //    : s.TaskItems.All(t => t.IsCompleted)
        //        ? "Completed"
        //        : "Active"

        //            });




        //    }


        public async Task<PagedResult<SprintDto>> GetSprintsAsync(
    string? search,
    string filter,
    int page,
    int pageSize)
        {
            var (sprints, totalCount) =
                await _sprintRepository.GetSprintsAsync(search, filter, page, pageSize);

            var sprintDtos = sprints.Select(s => new SprintDto
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
                        AppUserId = a.AppUserId
                    }).ToList()
                }).ToList(),

                TotalTasks = s.TaskItems.Count,
                CompletedTasks = s.TaskItems.Count(t => t.IsCompleted),

                Status =
                    s.TaskItems.Count(t => t.IsCompleted) == 0
                        ? "Planned"
                        : s.TaskItems.All(t => t.IsCompleted)
                            ? "Completed"
                            : "Active"
            }).ToList();

            return new PagedResult<SprintDto>
            {
                Items = sprintDtos,
                TotalCount = totalCount,
              
            };
        }

        public async Task<IEnumerable<SprintDto>> GetAllSprintsOnly()
        {
            var sprints = await _sprintRepository.GetAllSprintsOnly();
            return sprints.Select(s=>new SprintDto
            {
                Id = s.Id,
                Name = s.Name,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            }).ToList();
        }


        public async Task<SprintDto?> GetSprintsByProjectAsync(int projectId)
        {
            var sprint = await _sprintRepository.GetSprintsByProjectAsync(projectId);
            if (sprint == null)
                return null;

            return new SprintDto
            {
                Id = sprint.Id,
                Name = sprint.Name,
                StartDate = sprint.StartDate,
                EndDate = sprint.EndDate
            }; ;
        }
        //private string GetSprintStatus(Sprint s)
        //{
        //    var today = DateTime.Today;

        //    if (today < s.StartDate)
        //        return "Planning";

        //    if (today > s.EndDate)
        //        return "Completed";

        //    return "Active";
        //}

        public async Task<SprintStatsDto> GetsSprintsStats()
        {
            var allSprints = await _sprintRepository.GetSprintsStats();
            
                if (allSprints == null || !allSprints.Any())
                    return new SprintStatsDto();

            int activeSprints = 0;
            int completedSprints = 0;
            //var activeSprints = allSprints.Count(s => s.EndDate >= DateTime.UtcNow);
            //var completedSprints = allSprints.Count(s => s.EndDate < DateTime.Now);
            var tasksCompleted = allSprints.Sum(s => s.TaskItems?.Count(t => t.IsCompleted) ?? 0);

            foreach(var sprint in allSprints)
            {
                var totalTasks = sprint.TaskItems?.Count() ?? 0;
                var doneTasks = sprint.TaskItems?.Count(t => t.IsCompleted) ?? 0;

                // ✔ Sprint is completed only if it has tasks AND all tasks are completed
                bool isCompletedSprint = totalTasks > 0 && doneTasks == totalTasks;

                if (isCompletedSprint)
                    completedSprints++;
                else
                    activeSprints++;
            }
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
        public async Task<SprintDto?> GetSprintByIdAsync(int id)
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

                    }).ToList()
                }).ToList(),
                Boards = sprints.TaskItems
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
        public async Task<SprintDto> AddAsync(int projectId,SprintDto dto)
        {
            var sprint = new Sprint
            {
               
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ProjectId = projectId

            };



            var created = await _sprintRepository.AddAsync(sprint);
          
            return new SprintDto
            {

                Id = created.Id,
                
                Name = created.Name,
                StartDate = created.StartDate,
                EndDate = created.EndDate,
               
          

            };
            

        }

        public async Task<SprintDto?> UpdateAsync(int id,SprintDto dto)
        {
           

            var sprint = new Sprint
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
               
          
                };



           var updated= await _sprintRepository.UpdateAsync(id,sprint);
            return new SprintDto
            {

                Id = updated.Id,
                Name = updated.Name,
                StartDate = updated.StartDate,
                EndDate = updated.EndDate,
              


            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
            

           return await _sprintRepository.DeleteAsync(id);
            
        }




    }
}
