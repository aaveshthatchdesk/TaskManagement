using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.Interaces
{
    public interface IArchiveService
    {
        Task<bool> ArchiveProjectAsync(int projectId);

        Task<bool> RestoreProjectAsync(int projectId);
    }
}
