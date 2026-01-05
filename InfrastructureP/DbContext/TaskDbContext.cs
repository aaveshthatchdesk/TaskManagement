using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;

namespace Task.Infrastructure.DbContext
{
    public  class TaskDbContext:Microsoft.EntityFrameworkCore.DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) 
            : base(options) 
        {  
        }

        public DbSet<Project> projects { get; set; }

        public DbSet<Board> boards { get; set; }
        public DbSet<TaskItem> tasks {  get; set; }

        public DbSet<AppUser> appUsers { get; set; }

        public DbSet<AppUserAuth>appUserAuths { get; set; }

        public DbSet<TaskAssignment> taskAssignments { get; set; }
        public DbSet<Sprint> sprints { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }

        public DbSet<ProjectManager> ProjectManagers { get; set; } 

        public DbSet<TaskAttachment> TaskAttachments { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }

        public DbSet<TaskCreator> TaskCreators { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many: TaskItem ↔ AppUser
            modelBuilder.Entity<TaskAssignment>()
                .HasKey(ta => new { ta.TaskItemId, ta.AppUserId });

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(ta => ta.TaskItem)
                .WithMany(t => t.TaskAssignments)
                .HasForeignKey(ta => ta.TaskItemId);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(ta => ta.AppUser)
                .WithMany(u => u.TaskAssignments)
                .HasForeignKey(ta => ta.AppUserId);

            modelBuilder.Entity<Board>()
                .HasMany(b => b.TaskItems)
                .WithOne(t => t.Board)
                .HasForeignKey(t => t.BoardId)
                .OnDelete(DeleteBehavior.Cascade);  // When a board is deleted, delete its tasks

            // Sprint → TaskItem (One-to-Many)
            modelBuilder.Entity<Sprint>()
                .HasMany(s => s.TaskItems)
                .WithOne(t => t.Sprint)
                .HasForeignKey(t => t.SprintId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TaskAttachment>()
                .HasOne(ta => ta.TaskItem)
                .WithMany(t => t.TaskAttachments)
                 .HasForeignKey(ta => ta.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskComment>()
                .HasOne(tc => tc.TaskItem)
                .WithMany(t => t.TaskComments)
                .HasForeignKey(tc => tc.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskAttachment>()
                .HasOne(ta => ta.UploadedByUser)
                .WithMany()
                .HasForeignKey(ta => ta.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskComment>()
                .HasOne(tc => tc.CreatedByUser)
                .WithMany()
                .HasForeignKey(tc => tc.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            //      modelBuilder.Entity<Project>()
            //.HasOne(s => s.Sprint)
            //.WithOne(p => p.Project)
            //.HasForeignKey<Sprint>(s => s.ProjectId)
            //.OnDelete(DeleteBehavior.Cascade);
            //     modelBuilder.Entity<Project>()
            //.HasOne(p => p.Sprint)
            //.WithOne(s => s.Project)
            //.HasForeignKey<Sprint>(s => s.ProjectId);
            modelBuilder.Entity<Project>()
       .HasOne(p => p.Sprint)
       .WithOne(s => s.Project)
       .HasForeignKey<Sprint>(s => s.ProjectId)
       .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ProjectManager>()
        .HasKey(pm => new { pm.ProjectId, pm.AppUserId });

            modelBuilder.Entity<ProjectManager>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.Managers)
                .HasForeignKey(pm => pm.ProjectId);

            modelBuilder.Entity<ProjectManager>()
                .HasOne(pm => pm.AppUser)
                .WithMany(u => u.ManagedProjects)
                .HasForeignKey(pm => pm.AppUserId);



            modelBuilder.Entity<ProjectMember>()
       .HasKey(pm => new { pm.ProjectId, pm.AppUserId });

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.AppUser)
                .WithMany(u => u.ProjectMembers)
                .HasForeignKey(pm => pm.AppUserId);


            modelBuilder.Entity<TaskCreator>()
    .HasOne(tc => tc.TaskItem)
    .WithMany(t => t.TaskCreators)
    .HasForeignKey(tc => tc.TaskItemId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskCreator>()
                .HasOne(tc => tc.AppUser)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(tc => tc.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Project>().Ignore(p => p.Progress);
            modelBuilder.Entity<Project>().Ignore(p => p.TeamMembers);

           


        }
    }
}
