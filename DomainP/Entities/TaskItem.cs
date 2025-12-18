using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Priority {  get; set; } = string.Empty;   
        public DateTime? DueDate { get; set; }

        public int BoardId { get; set; }

        public Board Board { get; set; } = null!;
        
        public int Order { get; set; }

        public int? SprintId {  get; set; }
        public Sprint? Sprint { get; set; } 
        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedDate { get; set; }
        //public DateTime? CreatedOn { get; set; }

        //public DateTime? LastUpdatedOn { get; set; }

  
        public ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
    }
}
