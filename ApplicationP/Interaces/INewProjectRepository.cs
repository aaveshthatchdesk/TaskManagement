using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface INewProjectRepository
    {
        Task<List<AppUser>> GetUsersByIdsAsync(List<int> ids);
        Task<Project> AddProjectAsync(Project project);
    }
}
