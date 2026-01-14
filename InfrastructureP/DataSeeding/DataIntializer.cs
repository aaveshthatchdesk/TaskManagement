using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Task.Domain.Entities;
using Task.Infrastructure.DbContext;

namespace Task.Infrastructure.DataSeeding
{
    public class DataIntializer
    {
        public static async System.Threading.Tasks.Task Seed(TaskDbContext context)
        {
            try
            {
                await context.Database.MigrateAsync();
                await SeedAppUser(context);
                await SeedAppUserAuth(context);
                //await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
               private static async System.Threading.Tasks.Task SeedAppUser(TaskDbContext context)
        {
            var appearancetypes = InitialDataGenerator.GetUser();
            foreach(var item in appearancetypes)
            {
                var existingItem = await context.appUsers
                    .FirstOrDefaultAsync(x => x.Email == item.Email);
                if (existingItem == null)
                {
                    await context.appUsers.AddAsync(item);
                }
            }
            await context.SaveChangesAsync();
        }

        private static async System.Threading.Tasks.Task SeedAppUserAuth(TaskDbContext context)
        {
            var appearancetypes = InitialDataGenerator.GetUserAuth();

            foreach(var item in appearancetypes)
            {
                var user = await context.appUsers
                   .FirstOrDefaultAsync(x => x.Id == item.AppUserId);
                if(user==null)
                {
                    continue;
                }
                var existingAuth = await context.appUserAuths
                    .FirstOrDefaultAsync(x => x.AppUserId == user.Id);
                if (existingAuth == null)
                {
                    var auth = new AppUserAuth
                    {
                        AppUserId = user.Id, 
                        PasswordHash = item.PasswordHash,
                        PasswordSalt = item.PasswordSalt,
                        IsTemporaryPassword = item.IsTemporaryPassword
                    };
                    await context.appUserAuths.AddAsync(item);
                }
            }
            await context.SaveChangesAsync();
        }

    
    }
}
