using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Application.Services;
using Task.Infrastructure.Repository;

namespace TaskManagementServerAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly IEmailService _emailService;

        public MemberController(IMemberService memberService,IEmailService emailService)
        {
            _memberService = memberService;
            _emailService = emailService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetMembers(int pageNumber=1,int pageSize=5,string? search=null)
        {
            //var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            //if (userRole == "Admin")
            //{
            //    var all = await _memberService.GetAppUsersAsync(pageNumber,pageSize,search);
            //    return Ok(all);
            //}
            var all = await _memberService.GetAppUsersAsync(pageNumber, pageSize, search);
            return Ok(all);
            //else if (userRole == "Manager")
            //{
            //    var members = await _memberService.GetByRoleAsync("Member");
            //    return Ok(members);
            //}
            //else if (userRole == "Member")
            //{
            //    var email = User.FindFirst(ClaimTypes.Email)?.Value;
            //    var all = await _memberService.GetAppUsersAsync(pageNumber,pageSize,search);
            //    var current = all.FirstOrDefault(m => m.Email == email);
            //    return Ok(current == null ? new List<AppUserDto>() : new List<AppUserDto> { current });

            //}
            
        }
        [HttpGet("ForManager")]
        public async Task<ActionResult<IEnumerable<AppUserDto>>>GetMembersForManagerAsync(int pageNumber,int pageSize,string? search)
        {
            var all = await _memberService.GetMembersForManagerAsync(pageNumber, pageSize, search);
            return Ok(all);
        }

        [HttpGet("memberslist")]
        public async Task<IActionResult> GetOnlyMembers()
        {
            var members = await _memberService.GetByRoleAsync("Member");
            return Ok(members);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AppUserDto>>GetMemberById(int id)
        {
            var result = await _memberService.GetByIdAsync(id);
            return Ok(result);
        }
      

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> CreateMember([FromBody] AppUserDto dto)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "Manager" && dto.Role == "Manager")
                return Forbid(); // Manager cannot create another Manager

            try
            {
                var result = await _memberService.AddMemberAsync(dto);

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var roleBasedUrl = $"https://localhost:7142/?role={dto.Role}";

                    string subject = "Login Credentials";
                    string body = $@"
        <p>Hello <strong>{result.Name}</strong>,</p>

        <p>Your account has been created successfully.</p>

        <p>
            <strong>Temporary Password:</strong>
            <span style='font-size: 16px; color: #0d6efd;'><b>{result.TempPassword}</b></span>
        </p>

        <p>Please log in and change your password .</p>

        <p>
            Click here to login:  
            <a href='{roleBasedUrl}' style='color:#0d6efd; font-weight:bold;'>
                Go to Website
            </a>
        </p>

        <br>
      <p>Regards,<br/>Task Management Team</p> ";

                
                    _ = System.Threading.Tasks.Task.Run(() =>
                        _emailService.SendAsync(dto.Email, subject, body)
                    );
                }
                    
                //return Ok(new { message = $"User '{dto.Email}' assigned as {dto.Role} successfully." });
                return Ok(result);
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

        [HttpDelete("RemoveFromProject")]
        public async Task<IActionResult> RemoveMember(int projectId, int memberId)
        {
            var success = await _memberService.RemoveMemberAsync(projectId, memberId);

            if (!success)
                return NotFound("Member not assigned to this project.");

            return Ok("Member removed successfully.");
        }


    }
}



