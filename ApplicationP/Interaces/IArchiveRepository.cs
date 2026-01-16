using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface IArchiveRepository
    {
        Task<Project?> GetByIdAsync(int id);
        System.Threading.Tasks.Task UpdateAsync(Project project);
        Task<int> GetProjectProgressAsync(int projectId);
    }
}
