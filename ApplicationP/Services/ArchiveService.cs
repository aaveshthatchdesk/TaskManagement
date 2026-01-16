using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;

namespace Task.Application.Services
{
    public  class ArchiveService:IArchiveService
    {
        private readonly IArchiveRepository _archiveRepository;

        public ArchiveService(IArchiveRepository archiveRepository)
        {
            _archiveRepository = archiveRepository;
        }

        public async Task<bool> ArchiveProjectAsync(int projectId)
        {
            var project=await _archiveRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return false; // Project not found
            }
            project.Status = ProjectStatus.Archived;
            await _archiveRepository.UpdateAsync(project);
            return true;

        }
        public async Task<bool> RestoreProjectAsync(int projectId)
        {
            var project = await _archiveRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                return false; // Project not found
            }
            var progress = await _archiveRepository.GetProjectProgressAsync(projectId);
            project.Status = progress==100? ProjectStatus.Completed : ProjectStatus.Active;
            await _archiveRepository.UpdateAsync(project);
            return true;
        }
    }
}
