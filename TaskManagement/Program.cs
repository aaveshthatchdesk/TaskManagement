


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Task.Application.Interaces;
using Task.Application.Services;
using Task.Infrastructure.DbContext;
using Task.Infrastructure.Repository;

namespace TaskManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<TaskDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs")
               
               ));

            // Add services to the container.
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<ISprintService, SprintService>();
            builder.Services.AddScoped<ISprintRepository, SprintRepository>();
            builder.Services.AddScoped<IManagerService, ManagerService>();
            builder.Services.AddScoped<IManagerRepository,ManagerRepository>();
            builder.Services.AddScoped<IBoardService, BoardService>();
            builder.Services.AddScoped<IBoardRepository, BoardRepository>();
            builder.Services.AddScoped<ITaskItemService, TaskItemService>();
            builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
            builder.Services.AddScoped<IAssignMemberService, AssignMemberService>();
            builder.Services.AddScoped<IAssignMemberRepository, AssignMemberRepository>();

            builder.Services.AddScoped<IEmailService, EmailService>();


            builder.Services.AddScoped<INewProjectService, newProjectService>();
            builder.Services.AddScoped<INewProjectRepository, NewProjectRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            //builder.Services.AddControllers();
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddControllers().AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            //});
            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Task Management API",
                    Version = "v1"
                });

                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOi...\""
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            // JWT Authentication



            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    RoleClaimType = ClaimTypes.Role
                };

            });

            builder.Services.AddAuthorization(options=>
            {
                options.AddPolicy("RequireAdminRole",
       policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireManagerOrAdminRole",
                    policy => policy.RequireRole("Manager", "Admin"));
                options.AddPolicy("RequireAdminOrMangerOrMemberRole", 
                    policy => policy.RequireRole("Admin", "Member", "Manager"));
            });
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorClient",
                    policy => policy.WithOrigins("https://localhost:7142")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });
         



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();            // Generate swagger JSON
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
                    c.RoutePrefix = string.Empty;
                });
            }
            app.UseCors("AllowBlazorClient");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
