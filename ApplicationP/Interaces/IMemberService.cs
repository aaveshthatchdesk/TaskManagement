using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;


namespace Task.Application.Interaces
{
   public interface IMemberService
    {
        //public Task<IEnumerable<AppUserDto>> GetAppUsersAsync();
        public Task<PagedResult<AppUserDto>> GetAppUsersAsync(int pageNumber, int pageSize, string? search);
        public Task<IEnumerable<AppUserDto>> GetByRoleAsync(string role);

        public  Task<PagedResult<AppUserDto>> GetMembersForManagerAsync(int pageNumber, int pageSize, string? search);
        public Task<AppUserDto?> GetByIdAsync(int id);
       
        public  Task<AppUserDto?> GetByEmailAsync(string email);

        public Task<AppUserDto> AddMemberAsync(AppUserDto appUser);

        public Task<AppUserDto> UpdateMemberAsync(int id, AppUserDto appUser);
        public Task<bool> DeleteAsync(int id);

        public Task<bool> RemoveMemberAsync(int projectId, int memberId);
    }
}
