using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Task.Domain.Entities;
using Task.Application.DTOs;

namespace Task.Application.Interaces
{
    public interface IAuthService
    {
        //public Task<AppUserDto> RegisterUserAsync(RegisterDto registerDto);
        public Task<AppUserDto> RegisterUserAsync(RegisterDto registerDto);
        public Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync(string token);
         Task<(bool Success, string Role)> ChangePasswordAsync(ChangePasswordDto dto);
    }
}
