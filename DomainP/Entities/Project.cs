using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
    public class Project
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public string Description { get; set; }
       
        
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;

        public DateTime? CreatedDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public string Visibility { get; set; } = "Public";

        //public List<AppUser> Managers { get; set; } = new();

        public ICollection<ProjectManager> Managers { get; set; } = new List<ProjectManager>();

        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<Board> Boards { get; set; } = new List<Board>();
        //public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
        public Sprint Sprint { get; set; }
        //public int? SprintId { get; set; }

        public int Progress
        {
            get
            {
                var allTasks = Boards.SelectMany(b => b.TaskItems).ToList();
                if (!allTasks.Any())
                    return 0;

                var completed = allTasks.Count(t => t.IsCompleted);
                return (int)Math.Round((double)completed / allTasks.Count * 100);
            }
        }
        public List<AppUser> TeamMembers =>
          Boards.SelectMany(b => b.TaskItems)
                .SelectMany(t => t.TaskAssignments)
                .Select(a => a.AppUser)
                .Distinct()
                .ToList();
    }
    public enum ProjectStatus
    {
        Active,
        Completed,
        Archeived
    }
}
