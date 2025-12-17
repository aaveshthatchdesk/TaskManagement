using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task.Application.DTOs;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignMemberController : ControllerBase
    {
        private readonly IAssignMemberService _assignMemberService;

        public AssignMemberController(IAssignMemberService assignMemberService)
        {
            _assignMemberService = assignMemberService;
        }

        [HttpPost("{taskId}/assign")]
        public async Task<IActionResult> AssignMember(int taskId, [FromBody] int appUserId)
        {
            var result = await _assignMemberService.AssignedMemberAsync(taskId, appUserId);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{taskId}/assign/{userId}")]
        public async Task<IActionResult> RemoveMember(int taskId, int userId)
        {
            var result = await _assignMemberService.RemoveMemberAsync(taskId, userId);
            return result ? NoContent() : NotFound();
        }
    }
}
