using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchiveController : ControllerBase
    {
        private readonly IArchiveService _archiveService;
        public ArchiveController(IArchiveService archiveService)
        {
            _archiveService = archiveService;
        }
        [HttpPost("Archive/{projectId}")]
        public async Task<IActionResult> ArchiveProject(int projectId)
        {
            var result = await _archiveService.ArchiveProjectAsync(projectId);
            if (!result)
            {
                return NotFound(new { Message = "Project not found." });
            }
            return Ok(new { Message = "Project archived successfully." });
        }
        [HttpPost("Restore/{projectId}")]
        public async Task<IActionResult> RestoreProject(int projectId)
        {
            var result = await _archiveService.RestoreProjectAsync(projectId);
            if (!result)
            {
                return NotFound(new { Message = "Project not found." });
            }
            return Ok(new { Message = "Project restored successfully." });
        }

    }
}
