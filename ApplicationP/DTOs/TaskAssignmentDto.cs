using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Task.Application.DTOs
{
      public  class TaskAssignmentDto
    {

        public int TaskItemId { get; set; }
        //public TaskItemDto TaskItem { get; set; } = null!;

        public int AppUserId { get; set; }

        //public AppUserDto? AppUser { get; set; } = null!;
    }
}
