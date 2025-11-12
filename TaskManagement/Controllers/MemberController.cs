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
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDto>>>GetMembers()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if(userRole=="Admin")
            {
                var all=await _memberService.GetAppUsersAsync();
                return Ok(all);
            }
           else  if (userRole=="Manager")
            {
                var members=await _memberService.GetByRoleAsync("Member");
                return Ok(members);
            }
            else if(userRole=="Member")
            {
               var email=User.FindFirst(ClaimTypes.Email)?.Value;
                var all = await _memberService.GetAppUsersAsync();
                var current = all.FirstOrDefault(m => m.Email == email);
                return Ok(current == null ? new List<AppUserDto>() : new List<AppUserDto> { current });

            }
            return Forbid();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> CreateMember(AppUserDto dto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Manager" && dto.Role == "Manager")
                return Forbid(); // Manager cannot create another Manager

            try
            {
                var result = await _memberService.AddMemberAsync(dto);
                return Ok(new { message = $"User '{dto.Email}' assigned as {dto.Role} successfully." });
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> UpdateMember(int id, AppUserDto dto)
        {
            var existing = await _memberService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Manager" && existing.Role == "Manager")
                return Forbid(); // Manager can't modify another Manager

            await _memberService.UpdateMemberAsync(id, dto);
            return Ok(new { message = "Member updated successfully." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> DeleteMember(int id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
                return NotFound();

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Manager" && member.Role == "Manager")
                return Forbid(); 

            await _memberService.DeleteAsync(id);
            return Ok(new { message = "Member deleted successfully." });
        }


    }
}
