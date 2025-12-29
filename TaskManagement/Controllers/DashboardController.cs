using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardService.GetSummaryAsync();
            return Ok(summary);
        }
        [HttpGet("SummaryForManager/{managerId}")]
        public async Task<IActionResult> GetSummaryForManager(int managerId)
        {
            var summary = await _dashboardService.GetSummaryForManagerAsync(managerId);
            return Ok(summary);
        }
        [HttpGet("SummaryForMember/{memberId}")]
        public async Task<IActionResult> GetSummaryForMember(int memberId)
        {
            var summary = await _dashboardService.GetSummaryForMemberAsync(memberId);
            return Ok(summary);
        }

    }
}
