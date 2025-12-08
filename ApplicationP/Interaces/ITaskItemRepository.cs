using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface ITaskItemRepository
    {
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<bool> SaveChangesAsync();
 
    }
}
