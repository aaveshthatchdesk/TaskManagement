using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Infrastructure.DataSeeding
{
    public class InitialDataGenerator
    {
        public static IEnumerable<AppUser> GetUser()
        {
            var now = DateTime.UtcNow;
            return new List<AppUser>
            {
                new AppUser
                {
                    //Id=1,
                    Name="Admin User",
                    Email="admin@taskmanagement.com",
                    Role="Admin"
        }
            };
        }
        public static IEnumerable<AppUserAuth> GetUserAuth()
        {
            var now = DateTime.UtcNow;
            string tempPassword = "Admin@123";

            CreatePasswordHash(tempPassword, out byte[] hash, out byte[] salt);
          
            return new List<AppUserAuth>
            {
                new AppUserAuth
                {
                    //Id=1,
                    AppUserId=1,
                    PasswordHash=hash,
                    PasswordSalt=salt,
                    IsTemporaryPassword = true
                }
            };
        }
        static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
