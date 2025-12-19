using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
    public class TaskComment
    {

        public int Id { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;
        public int CreatedByUserId { get; set; }
        public AppUser CreatedByUser { get; set; } = null!;
    }
}
