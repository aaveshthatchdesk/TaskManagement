using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Tnef;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Application.Services;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }
        [HttpGet("ProjectsGetByManager/{managerId}")]
        public async Task<IActionResult> GetByManager(int managerId, [FromQuery] string? filter = "All")
        {
            var projects = await _managerService.GetProjectsByManagerAsync(managerId, filter);
            return Ok(projects);
        }

        [HttpGet("All")]
        public async Task<ActionResult<List<ManagerDto>>> GetAllManagers()
        {
            var managers = await _managerService.GetAllManagersAsync();
            return Ok(managers);
        }

        [HttpGet("Managers")]
        public async Task<ActionResult<PagedResult<ManagerDto>>> GetManagers(int pageNumber = 1, int pageSize = 10, string search = "")
        {


            var managers = await _managerService.GetManagersPagedAsync(pageNumber, pageSize, search);
            return Ok(managers);
        }
        [HttpPost("{projectId}/Manager/Assign")]
        public async Task<IActionResult> AssignManager(int projectId, List<int> memberIds)
        {
            var result = await _managerService.AssignManagerAsync(projectId, memberIds);

            if (!result)
                return NotFound("Project not found.");

            return Ok(new { message = "Managers assigned successfully!" });
        }

        [HttpDelete("RemoveFromProject")]
        public async Task<IActionResult> RemoveManager(int projectId, int managerId)
        {
            var success = await _managerService.RemoveManagerAsync(projectId, managerId);

            if (!success)
                return NotFound("Manager not assigned to this project.");

            return Ok("Manager removed successfully.");
        }



    }
}
