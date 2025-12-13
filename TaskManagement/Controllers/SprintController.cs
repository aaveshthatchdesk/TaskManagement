using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintController : ControllerBase
    {
        private readonly ISprintService _sprintService;

        public SprintController(ISprintService sprintService)
        {
            _sprintService = sprintService;
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var result = await _sprintService.GetAllAsync();
        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<ActionResult<PagedResult<SprintDto>>> GetSprints(
    string? search = null,
    string filter = "All",
    int page = 1,
    int pageSize = 5)
        {
            return Ok(await _sprintService.GetSprintsAsync(search, filter, page, pageSize));
        }

        [HttpGet("Project/{projectId}")]
        public async Task<IActionResult> GetSprintsByProject(int projectId)
        {
            var result = await _sprintService.GetSprintsByProjectAsync(projectId);
           
            return new JsonResult(result);
        }


        [HttpGet("stats")]
        public async Task<ActionResult<SprintStatsDto>> GetSprintStats()
        {
            var stats = await _sprintService.GetsSprintsStats();
            return Ok(stats);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID(int id)

        {
            var sprint = await _sprintService.GetSprintByIdAsync(id);
            return Ok(sprint);
        }
        [HttpPost("project/{projectId}")]
        public async Task<IActionResult> Create(int projectId,SprintDto sprint)
        {
            var result = await _sprintService.AddAsync(projectId,sprint);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SprintDto sprint)
        {
            if (id != sprint.Id)
            {
                return BadRequest();
            }
            var result = await _sprintService.UpdateAsync(id, sprint);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult>Delete(int id)
        {
           var deleted= await _sprintService.DeleteAsync(id);
            if(!deleted)
            {
                return NotFound("Project not found");
            }
            return NoContent();
        }

    }
}
