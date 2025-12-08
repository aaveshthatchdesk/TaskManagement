using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using SystemTask = System.Threading.Tasks.Task;





namespace Task.Application.Services
{
    public class AuthService : IAuthService
    {

        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration configuration;

        public AuthService(IAuthRepository authRepository,IConfiguration configuration)
        {
            _authRepository = authRepository;
            this.configuration = configuration;
        }
        public async  System.Threading.Tasks.Task<AppUserDto>RegisterUserAsync(RegisterDto registerDto)

        {

            var appUser = new AppUser
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Role = registerDto.Role,
            };
            using var hmac=new System.Security.Cryptography.HMACSHA512();
            var auth = new AppUserAuth
            {
                AppUser = appUser,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key

            };
            var createdUser=await _authRepository.RegisterUserAsync(appUser,auth);
            return new AppUserDto
            {
              
                Name = registerDto.Name,
                Email = registerDto.Email,
                Role= registerDto.Role,
            };
        }

        public async System.Threading.Tasks.Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var userAuth = await _authRepository.GetUserAuthByEmailAsync(loginDto.Email);
            if (userAuth == null)
            
                throw new Exception("Invalid email or password");

                using var hmac = new System.Security.Cryptography.HMACSHA512(userAuth.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                if (!computedHash.SequenceEqual(userAuth.PasswordHash))
                    throw new Exception("Invalid email or password");

               var userRole = userAuth.AppUser.Role;
                   if (!string.Equals(userRole, loginDto.LoginAs, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Invalid email or password");

            var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            //var tokenExpiresAt = DateTime.UtcNow.AddHours(2);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, userAuth.AppUser.Id.ToString()),
            new Claim(ClaimTypes.Name, userAuth.AppUser.Name),
            new Claim(ClaimTypes.Email,userAuth.AppUser.Email),
            new Claim(ClaimTypes.Role,userAuth.AppUser.Role)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = configuration["Jwt:Issuer"], 
                Audience = configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                //return tokenHandler.WriteToken(token);
            return new LoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                //ExpiresIn = tokenExpiresAt,
                IsTemporaryPassword = userAuth.IsTemporaryPassword,
                UserId = userAuth.AppUser.Id,
                Role = userAuth.AppUser.Role
            };
        }

        public async Task<(bool Success,string Role)>ChangePasswordAsync(ChangePasswordDto dto)
        {
            var userAuth = await _authRepository.GetUserAuthByUserIdAsync(dto.UserId);
            if (userAuth == null)
                throw new Exception("User not found");

           
            using var hmac = new System.Security.Cryptography.HMACSHA512(userAuth.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.OldPassword));
            if (!computedHash.SequenceEqual(userAuth.PasswordHash))
                throw new Exception("Old password is incorrect");

           
            using var newHmac = new System.Security.Cryptography.HMACSHA512();
            userAuth.PasswordHash = newHmac.ComputeHash(Encoding.UTF8.GetBytes(dto.NewPassword));
            userAuth.PasswordSalt = newHmac.Key;
            userAuth.IsTemporaryPassword = false;

            await _authRepository.UpdateUserAuthAsync(userAuth);
            //return true;
            return (true, userAuth.AppUser.Role);

        }
        public  Task<bool> LogoutAsync(string token)
        {
            // For stateless JWT, logout is client-side (delete token)
            //return Task.CompletedTask;
            return System.Threading.Tasks.Task.FromResult(true);
        }
    }

}

