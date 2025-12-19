using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class TaskCommentDto
    {
        public int Id { get; set; }
      
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public int CreatedByUserId { get; set; }
        public  string CreatedByUserName { get; set; } = string.Empty;
    }
}
