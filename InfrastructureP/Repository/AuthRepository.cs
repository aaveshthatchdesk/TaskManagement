using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;

namespace Task.Infrastructure.Repository
{
    public  class AuthRepository:IAuthRepository
    {
        private readonly TaskDbContext _taskDbContext;

        public AuthRepository(TaskDbContext taskDbContext)
        {
            _taskDbContext = taskDbContext;
        }
        public async Task<AppUser> RegisterUserAsync(AppUser appUser,AppUserAuth auth)
        {
            var existinguser = await _taskDbContext.appUsers.AnyAsync(u => u.Name == appUser.Name);
            if (existinguser)
            {
                throw new Exception("username already exist");
            }

           
            
            _taskDbContext.appUsers.Add(appUser);
            _taskDbContext.appUserAuths.Add(auth);
            await _taskDbContext.SaveChangesAsync();
            return appUser;
           

        }
        //public async Task<(AppUser appUser, AppUserAuth auth)?>LoginUserAsync(string email,string password)
        //{
        //    var authEntry=await _taskDbContext.appUserAuths.Include(a=>a.AppUser)
        //                                                      .FirstOrDefaultAsync(a => a.AppUser.Email==email);
        //    if (authEntry == null) 
        //        return null;

        //    if(!VerifyPassword(password, authEntry.PasswordHash, authEntry.PasswordSalt))
        //        return null;
        //    return (authEntry.AppUser, authEntry);

        //}

        public async Task<AppUserAuth?> GetUserAuthByEmailAsync(string email)
        {
            return await _taskDbContext.appUserAuths
                .Include(a => a.AppUser)
                .FirstOrDefaultAsync(a => a.AppUser.Email == email);
        }

    }
}
