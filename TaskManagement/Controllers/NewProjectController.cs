using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Application.Services;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewProjectController : ControllerBase
    {
        private readonly INewProjectService _newProjectService;

        public NewProjectController(INewProjectService newProjectService)
        {
            _newProjectService = newProjectService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectCreateDto dto)
        {

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = await _newProjectService.AddProjectAsync(dto,userId);
            return Ok(project);
        }
    }
}
