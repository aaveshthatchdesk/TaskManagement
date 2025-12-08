using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface ISprintService
    {
        //Task<IEnumerable<SprintDto>> GetAllAsync();

        //Task<IEnumerable<SprintDto>> GetAllSprintsOnly();
        Task<SprintDto?> GetSprintsByProjectAsync(int projectId);

        //Task<SprintStatsDto> GetsSprintsStats();
        //Task<SprintDto?> GetSprintByIdAsync(int id);
        Task<SprintDto> AddAsync(int ProjectId,SprintDto dto);
        Task<SprintDto?> UpdateAsync(int id, SprintDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
