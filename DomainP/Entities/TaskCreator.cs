using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
    public class TaskCreator
    {

        public int Id { get; set; }

        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; } = null!;

        
    }
}
