using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task.Application.DTOs;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public ProjectController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsAsync( string? filter="All")
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var isAdmin = User.IsInRole("Admin");
            Console.WriteLine($"UserId: {userId}, IsAdmin: {isAdmin}");
            var result = await _taskService.GetAllProjectsAsync(userId,isAdmin,filter);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ProjectDto>>GetProjectIdAsync(int id)
        {


            var result = await _taskService.GetProjectByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        //[Authorize(Policy = "RequireAdminRole")]
        [HttpPost]

        public async Task<ActionResult<ProjectDto>>AddAsync([FromBody]ProjectDto projectDto)
        {
            var data = await _taskService.AddProjectAsync(projectDto);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [Authorize(Policy = "RequireAdminRole")]
        
        [HttpPut("{id}")]
         public async Task <ActionResult<ProjectDto>>UpdateProjectAsync(int id, [FromBody]ProjectDto projectDto)
        {
            if (id != projectDto.Id)
                return BadRequest("ID mismatch");
            try
            {
                var data = await _taskService.UpdateProjectAsync(id, projectDto);

                if (data == null)
                {
                    return NotFound();
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>>DeleteProjectAsync(int id)
        {
            var deleted = await _taskService.DeleteProjectAsync(id);
            if (!deleted)
                return NotFound("Project not found");

            return NoContent();
        }

    }
}
