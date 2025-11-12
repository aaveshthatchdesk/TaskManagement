using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task.Application.Interaces;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public AccountController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Invalid token: email not found");
                }
                var user = await _memberService.GetByEmailAsync(email);
                if (user == null)
                {
                    return NotFound();
                }

                string initials = string.Empty;
                if (!string.IsNullOrWhiteSpace(user?.Name))
                {
                    initials = string.Join("", user.Name
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(n => n[0]))
                        .ToUpper();
                }

                return Ok(new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role,
                    Initials = initials
                });



            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }


    }
}
