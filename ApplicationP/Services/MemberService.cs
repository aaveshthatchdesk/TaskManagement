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

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
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

        public async Task<AppUserDto>AddMemberAsync(AppUserDto user)
        {
            //var member = new AppUser
            //{
            //    Name = user.Name,
            //    Email = user.Email,
            //    Role = user.Role,

            //};
            var existing = await _memberRepository.GetByEmailAsync(user.Email);
            if (existing == null)
            {
                throw new InvalidOperationException("User not found.Please Ask to user to register first.");
            }
            if(existing.Role!=user.Role)
            {
                existing.Role=user.Role;
                await _memberRepository.UpdateMemberAsync(existing.Id,existing);
            }
          
            return new AppUserDto
            {
                Id = existing.Id,
                Name = existing.Name,
                Email = existing.Email,
                Role = existing.Role,
            };
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
    }
}
