using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Application.DTOs;
using Task.Domain.Entities;

namespace Task.Application.Interaces
{
    public interface IAuthRepository
    {
        //public Task<AppUser> RegisterUserAsync(Register register);
        public Task<AppUser> RegisterUserAsync(AppUser appUser,AppUserAuth auth);
        public  Task<AppUserAuth?> GetUserAuthByEmailAsync(string email);
    }
}
