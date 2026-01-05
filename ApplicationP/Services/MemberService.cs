using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens.Experimental;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Application.Interaces;
using Task.Domain.Entities;

namespace Task.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public MemberService(IMemberRepository memberRepository,IEmailService emailService,IConfiguration configuration)
        {
            _memberRepository = memberRepository;
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task<IEnumerable<AppUserDto>> GetAppUsersAsync()
        {
            var member = await _memberRepository.GetAppUsersAsync();
            return member.Select(m => new AppUserDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Role = m.Role,
            });

        }
      

        public async Task<IEnumerable<AppUserDto>> GetByRoleAsync(string role)
        {
            var member = await _memberRepository.GetByRoleAsync(role);
            return member.Select(m => new AppUserDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Role = m.Role,
            });

        }
        public async Task<AppUserDto> GetByIdAsync(int id)
        {
            var member = await _memberRepository.GetByIdAsync(id);

            if (member == null)
            {
                return null;
            }
            return new AppUserDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Role = member.Role,
            };
        }
     

        public async Task<AppUserDto?> GetByEmailAsync(string email)
        {
            var member = await _memberRepository.GetByEmailAsync(email);
            if (member == null)
                return null;

            return new AppUserDto
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Role = member.Role
            };
        }

        public async Task<AppUserDto> AddMemberAsync(AppUserDto user)
        {

            var existing = await _memberRepository.GetByEmailAsync(user.Email);
            if (existing != null)
            {
                throw new InvalidOperationException($"User '{user.Email}' already exists.");
            }
            else
            {
                var newUser = new AppUser
                {
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                };
                await _memberRepository.AddMemberAsync(newUser);
                await _memberRepository.SaveChangesAsync();

                string tempPassword = _configuration["TemporaryPassword"] ?? throw new Exception("TemporaryPassword not configured");
               CreatePasswordHash(tempPassword,out byte[] hash,out byte[] salt);

                var auth = new AppUserAuth
                {
                    AppUserId = newUser.Id,
                    Password = tempPassword,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    IsTemporaryPassword = true
                };

                await _memberRepository.AddUserAuthAsync(auth);
                await _memberRepository.SaveChangesAsync();




                return new AppUserDto
                {
                    Id = newUser.Id,
                    Name = newUser.Name,
                    Email = newUser.Email,
                    Role = newUser.Role,
                    TempPassword=tempPassword
                };
            }
        }
        
        public async Task<AppUserDto>UpdateMemberAsync(int id,AppUserDto appUserDto)
        {
            //var member = new AppUser
            //{
            //    Name = appUserDto.Name,
            //    Email = appUserDto.Email,
            //    Role = appUserDto.Role,

            //};
            var existing=await _memberRepository.GetByIdAsync(id);
            if(existing == null)
            {
                return null;
            }

            existing.Name = appUserDto.Name;
            existing.Email = appUserDto.Email;
            existing.Role =appUserDto.Role;

            await _memberRepository.UpdateMemberAsync(id,existing);
            return new AppUserDto
            {
                Id = existing.Id,
                Name = existing.Name,
                Email = existing.Email,
                Role = existing.Role,
            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
           return  await _memberRepository.DeleteAsync(id);
        }
        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public async Task<bool> RemoveMemberAsync(int projectId, int memberId)
        {
            return await _memberRepository.RemoveMemberAsync(projectId, memberId);
        }

    }
}
