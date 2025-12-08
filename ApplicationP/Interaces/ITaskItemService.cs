using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;

namespace Task.Application.Interaces
{
    public interface ITaskItemService
    {
        Task<bool> UpdateTaskAsync(int taskId,TaskItemDto dto);
    }
}
