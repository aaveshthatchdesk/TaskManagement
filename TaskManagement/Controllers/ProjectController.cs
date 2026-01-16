using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Application.Services;

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
        public async Task<ActionResult<PagedResult<ProjectDto>>> GetProjectsAsync(string? filter = "All", int page = 1, int pageSize = 6, string? search = null,
                                                                                   int? managerId = null,
                                                                                   int? memberId=null,
                                                                                  DateTime? createdDate = null,
                                                                                DateTime? startDate = null,
                                                                                  DateTime? endDate = null)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            //string role = User.IsInRole("Admin");
            string role =
       User.IsInRole("Admin") ? "Admin" :
       User.IsInRole("Manager") ? "Manager" :
       "Member";
            if(role=="Manager")
            {
                managerId = null;
            }
            else if(role=="Member")
            {
                managerId = null;
                memberId = null;
            }

            //Console.WriteLine($"UserId: {userId}, IsAdmin: {isAdmin}");
            var result = await _taskService.GetAllProjectsPagedAsync(userId, role, filter, page, pageSize, search, managerId,memberId, createdDate, startDate, endDate);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{projectId}/Members")]
        public async Task<IActionResult> GetProjectMembers(int projectId)
        {
            var members = await _taskService.GetProjectMembersAsync(projectId);
            return Ok(members);
        }

        //[HttpGet("{projectId}/MembersAssigned")]
        //public async Task<IActionResult> GetProjectMembersAssigned(int projectId)
        //{
        //    var members = await _taskService.GetProjectMembersAssignedAsync(projectId);
        //    return Ok(members);
        //}


        [HttpGet("{projectId}/Managers")]
        public async Task<IActionResult> GetManagersByProject(int projectId)
        {
            var managers = await _taskService.GetManagersByProjectAsync(projectId);
            return Ok(managers);
        }



        [HttpGet("{id}")]

        public async Task<ActionResult<ProjectDto>> GetProjectIdAsync(int id)
        {


            var result = await _taskService.GetProjectByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("{projectId}/Members/Assign")]
        public async Task<IActionResult> AssignMembers(int projectId, List<int> memberIds)
        {
            var result = await _taskService.AssignMembersAsync(projectId, memberIds);

            if (!result)
                return NotFound("Project not found.");

            return Ok(new { message = "Members assigned successfully!" });
        }



        [HttpPost]

        public async Task<ActionResult<ProjectDto>> AddAsync([FromBody] ProjectDto projectDto)
        {
            var data = await _taskService.AddProjectAsync(projectDto);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }


        [Authorize]
        [HttpPost("new")]
        public async Task<IActionResult> CreateProject(ProjectDto dto)
        {

            

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = await _taskService.AddProjectAsync(dto);
            return Ok(project);
        }
        [Authorize(Policy = "RequireAdminOrMangerOrMemberRole")]

        [HttpPut("{id}")]
        public async Task<ActionResult<ProjectDto>> UpdateProjectAsync(int id, [FromBody] ProjectDto projectDto)
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
                var error = ex.InnerException?.Message ?? ex.Message;
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
        [HttpGet("projects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _taskService.GetAllProjectsAsync();
            return Ok(projects);
        }

        // POST: api/ProjectAssignment/assign-projects
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("assign-projects")]
        public async Task<IActionResult> AssignProjectsToManager(List<int> ProjectIds,int managerId)
        {
           

            await _taskService.AssignProjectsToManagerAsync(managerId, ProjectIds);
            return Ok(new { message = "Projects assigned successfully" });
        }



    }
}
