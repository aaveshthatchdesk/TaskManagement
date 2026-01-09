using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
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

        public async Task<(List<AppUser>Users,int TotalCount)>GetAppUsersAsync(int pageNumber,int pageSize,string? search)
        {
           var query=_taskDbContext.appUsers.AsQueryable();
            if(!string.IsNullOrWhiteSpace(search))
            {
                query=query.Where(u=>
                   u.Name.Contains(search)||
                   u.Email.Contains(search)||
                   u.Role.Contains(search));
            }
               var totalCount = await query.CountAsync();
            var users=await query
                  .OrderBy(u=>u.Id)
                  .Skip((pageNumber-1)*pageSize)
                  .Take(pageSize)
                     .ToListAsync();

            return (users, totalCount);
        }
       
        public async Task<IEnumerable<AppUser>>GetByRoleAsync(string role)
        {
            return await _taskDbContext.appUsers.Where(a=>a.Role==role).ToListAsync();
        }
        public async Task<(List<AppUser> Users, int TotalCount)> GetMembersForManagerAsync(int pageNumber=1, int pageSize=5, string? search=null)
        {
            var query = _taskDbContext.appUsers
       .Where(u => u.Role == "Member").AsQueryable(); // ONLY members

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search) ||
                    u.Email.Contains(search));

            }
            var totalCount = await query.CountAsync();
            var users = await query
                  .OrderBy(u => u.Id)
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                     .ToListAsync();

            return (users, totalCount);


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
        public async Task<AppUserAuth> AddUserAuthAsync(AppUserAuth auth)
        {
            await _taskDbContext.appUserAuths.AddAsync(auth);
            return auth;
        }

        public async Task<bool> SaveChangesAsync()
        {
            await _taskDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMemberAsync(int projectId, int memberId)
        {
            var projectMember = await _taskDbContext.ProjectMembers
                .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.AppUserId == memberId);

            if (projectMember == null)
                return false;

            _taskDbContext.ProjectMembers.Remove(projectMember);
            await _taskDbContext.SaveChangesAsync();

            return true;
        }

    }
}
