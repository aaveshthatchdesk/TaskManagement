using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var project = await _newProjectService.AddProjectAsync(dto);
            return Ok(project);
        }
    }
}
