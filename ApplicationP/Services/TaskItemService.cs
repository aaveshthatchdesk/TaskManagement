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
           
            task.Order = dto.Order;


            return await _taskItemRepository.SaveChangesAsync();


        }
    }
}
