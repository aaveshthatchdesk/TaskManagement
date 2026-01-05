using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogController : ControllerBase
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;


        }

        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetMemberActivity(
       
       int memberId,
       [FromQuery] int take = 5)
        {
            var activities = await _activityLogService
                .GetActivitiesForMemberAsync( memberId, take);

            return Ok(activities);
        }
    }
}
