using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
    public  class ActivityLog
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? BoardId { get; set; }

        public int? TaskItemId {  get; set; }
        public TaskItem? TaskItem { get; set; }

        
        public int PerformedByUserId { get; set; }
        public AppUser PerformedByUser { get; set; } = null!;

        public int? TargetUserId { get; set; }
        public AppUser? TargetUser { get; set; }

        public string ActionType { get; set; }=string.Empty;
        public string Description { get; set; }=string.Empty;
        public DateTime CreatedOn { get; set; }=DateTime.UtcNow;

    }
}
