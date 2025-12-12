using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface ISprintRepository
    {
        Task<IEnumerable<Sprint>> GetAllAsync();
        public Task<IEnumerable<Sprint>> GetSprintsStats();
        Task<IEnumerable<Sprint>> GetAllSprintsOnly();
        Task<Sprint> GetSprintsByProjectAsync(int projectId);
        Task<Sprint?> GetSprintByIdAsync(int id);
        Task<Sprint> AddAsync(Sprint sprint);
        Task<Sprint?> UpdateAsync(int id,Sprint sprint);
        Task<bool> DeleteAsync(int id);
    }
}
