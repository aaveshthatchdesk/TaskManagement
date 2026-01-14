using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Infrastructure.DbContext;

namespace Task.Infrastructure.DataSeeding
{
    public  static class  DbContextExtensions
    {
       
        public static async Task<IApplicationBuilder> ConfigureDataContext(this IApplicationBuilder app)
        {
            try
            {
                using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
                var services = serviceScope.ServiceProvider;
                using (var scope = services.CreateScope())
                {
                    using (var context = scope.ServiceProvider.GetRequiredService<TaskDbContext>())
                    {
                        try
                        {
                            await DataIntializer.Seed(context);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                return app;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
