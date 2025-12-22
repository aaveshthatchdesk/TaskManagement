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
    public class TaskItemService:ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public TaskItemService(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<bool> UpdateTaskAsync(int taskId,TaskItemDto dto)
        {
            var task = await _taskItemRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                return false;

            
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.LastUpdatedOn = DateTime.UtcNow;

            task.Order = dto.Order;


            return await _taskItemRepository.SaveChangesAsync();


        }
        public async Task<bool> UpdateTaskDescriptionAsync(int taskId, string description)
        {
            var task = await _taskItemRepository.GetTaskByIdAsync(taskId);
            if (task == null)
                return false;


      
            task.Description = description;
            task.LastUpdatedOn = DateTime.UtcNow;


            return await _taskItemRepository.SaveChangesAsync();


        }
        public async Task<IEnumerable<TaskItemDto>> GetTasksByProjectAsync(int projectId)
        {
            var tasks = await _taskItemRepository.GetByProjectIdAsync(projectId);
            return tasks.Select(MapToDto);
          
        }
        public async Task<TaskItemDto?> GetTaskByIdAsync(int taskId)
        {
            var task = await _taskItemRepository.GetByIdAsync(taskId);
            if (task == null)
                return null;
            return MapToDto(task);
        }
        public async Task<TaskItemDto> CreateTaskAsync(TaskItemDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                BoardId = dto.BoardId,
                CreatedOn= DateTime.UtcNow,
                LastUpdatedOn = DateTime.UtcNow,
                Order = dto.Order,
                IsCompleted = false,
            };
            var created = await _taskItemRepository.CreateAsync(task);
            return new TaskItemDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                Priority = created.Priority,
                DueDate = created.DueDate,
                BoardId = created.BoardId,
                CreatedOn= created.CreatedOn,
                LastUpdatedOn = created.LastUpdatedOn,
                IsCompleted = false,
                TaskAssignments = new List<TaskAssignmentDto>() 
            };
        }
        public async Task<TaskItemDto> UpdateTasksAsync(int taskId, TaskItemDto dto)
        {
            var task = await _taskItemRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new Exception("Task not found");
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.Order = dto.Order;
            task.IsCompleted = dto.IsCompleted;
            task.LastUpdatedOn = DateTime.UtcNow;
            task.CompletedDate = dto.IsCompleted ? DateTime.UtcNow : null;
            var updated = await _taskItemRepository.UpdateAsync(task);
            return new TaskItemDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Description = updated.Description,
                Priority = updated.Priority,
                DueDate = updated.DueDate,
                BoardId = updated.BoardId,
                IsCompleted = updated.IsCompleted,
                LastUpdatedOn= updated.LastUpdatedOn,
                CompletedDate = updated.CompletedDate,
                TaskAssignments = new List<TaskAssignmentDto>() 


            };
        }
        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await _taskItemRepository.GetByIdAsync(taskId);
            if (task == null)
                return false;
            return await _taskItemRepository.DeleteAsync(task);
        }
        private static TaskItemDto MapToDto(TaskItem t)
        {
            return new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                DueDate = t.DueDate,
                BoardId = t.BoardId,
                SprintId = t.SprintId,
                LastUpdatedOn = DateTime.UtcNow,
                Order = t.Order,
                IsCompleted = t.IsCompleted,
                CompletedDate = t.CompletedDate,
                TaskAssignments = t.TaskAssignments.Select(a => new TaskAssignmentDto
                {
                    TaskItemId = a.TaskItemId,
                    AppUserId = a.AppUserId,
                    AppUserName = a.AppUser.Name
                }).ToList()
            };
        }
        public async Task<bool> ReorderTasksAsync(List<TaskReorderDto> tasks)
        {
           var taskIds = tasks.Select(t => t.TaskId).ToList();
            var taskItems = await _taskItemRepository.GetByIdsAsync(taskIds);
            foreach (var task in taskItems)
            {
                var dto = tasks.First(t => t.TaskId == task.Id);

                task.BoardId = dto.BoardId;
                task.Order = dto.Order;
                if (dto.IsDoneBoard)
                {
                   
                        task.IsCompleted = true;
                        task.CompletedDate = DateTime.UtcNow;
                    
                }
                else
                {
                    task.IsCompleted = false;
                    task.CompletedDate = null;

                }

                task.LastUpdatedOn = DateTime.UtcNow;
            }
            return await _taskItemRepository.SaveChangesAsync();
        }
    }
}
