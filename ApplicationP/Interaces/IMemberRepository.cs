using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface IMemberRepository
    {
        public Task<IEnumerable<AppUser>> GetAppUsersAsync();
        public Task<IEnumerable<AppUser>> GetByRoleAsync(string role);
        public Task<AppUser?> GetByIdAsync(int id);
       public  Task<AppUser?> GetByEmailAsync(string email);

        public Task<AppUser> AddMemberAsync(AppUser appUser);

        public Task<AppUser> UpdateMemberAsync(int id, AppUser appUser);
        public Task<bool> DeleteAsync(int id);
    }
}
