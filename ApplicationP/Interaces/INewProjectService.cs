using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface INewProjectService
    {
        Task<ProjectCreateDto> AddProjectAsync(ProjectCreateDto projectdto,int createByUserId);
    }
}
