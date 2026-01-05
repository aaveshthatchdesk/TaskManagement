using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        [Authorize]
        [HttpPost("{taskId}/assign")]
        public async Task<IActionResult> AssignMember(int taskId, [FromBody] int appUserId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
               ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int createduserId = int.Parse(userIdClaim.Value);
            var result = await _assignMemberService.AssignedMemberAsync(taskId, appUserId,createduserId);
            return result ? Ok() : NotFound();
        }
        [Authorize]
        [HttpDelete("{taskId}/remove/{userId}")]
        public async Task<IActionResult> RemoveMember(int taskId, int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
               ?? User.FindFirst("UserId");

            if (userIdClaim == null)
                return Unauthorized("UserId not found in token");

            int createduserId = int.Parse(userIdClaim.Value);
            var result = await _assignMemberService.RemoveMemberAsync(taskId, userId,createduserId);
            return result ? NoContent() : NotFound();
        }
    }
}
