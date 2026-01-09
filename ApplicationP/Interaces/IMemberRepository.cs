using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;


namespace Task.Application.Interaces
{
    public interface IMemberRepository
    {
        //public Task<IEnumerable<AppUser>> GetAppUsersAsync();

        public Task<(List<AppUser>Users, int TotalCount)> GetAppUsersAsync(int pageNumber, int pageSize, string? search);
        public Task<IEnumerable<AppUser>> GetByRoleAsync(string role);
        public Task<(List<AppUser> Users, int TotalCount)> GetMembersForManagerAsync(int pageNumber, int pageSize, string? search);
        public Task<AppUser?> GetByIdAsync(int id);
       public  Task<AppUser?> GetByEmailAsync(string email);

        public Task<AppUser> AddMemberAsync(AppUser appUser);

        public Task<AppUser> UpdateMemberAsync(int id, AppUser appUser);
        public Task<bool> DeleteAsync(int id);
          Task<AppUserAuth> AddUserAuthAsync(AppUserAuth auth);
        Task<bool> SaveChangesAsync();

        public Task<bool> RemoveMemberAsync(int projectId, int memberId);

    }
}
