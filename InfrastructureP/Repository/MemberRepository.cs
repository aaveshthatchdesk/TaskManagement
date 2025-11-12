using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;

namespace Task.Infrastructure.Repository
{
    public class MemberRepository:IMemberRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public MemberRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }

        public async Task<IEnumerable<AppUser>>GetAppUsersAsync()
        {
            return await _taskDbContext.appUsers.ToListAsync();
        }
        public async Task<IEnumerable<AppUser>>GetByRoleAsync(string role)
        {
            return await _taskDbContext.appUsers.Where(a=>a.Role==role).ToListAsync();
        }
        public async Task<AppUser?>GetByIdAsync(int id)
        {
            return _taskDbContext.appUsers.FirstOrDefault(a=>a.Id==id);
        }
        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            return await _taskDbContext.appUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AppUser> AddMemberAsync(AppUser appUser)
        {
              await _taskDbContext.appUsers.AddAsync(appUser);
            await _taskDbContext.SaveChangesAsync();
            return appUser;
        }
        public async Task<AppUser> UpdateMemberAsync(int id,AppUser appUser)
        {
            var member = await _taskDbContext.appUsers.FindAsync(id);
            if (member != null)
            {
             _taskDbContext.appUsers.Update(member);
                
            }
            await _taskDbContext.SaveChangesAsync();
            return appUser;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var member = await _taskDbContext.appUsers.FindAsync(id);
            if (member != null)
            {
                _taskDbContext.appUsers.Remove(member);
                await _taskDbContext.SaveChangesAsync();

            }


               return true;
        }
    }
}
