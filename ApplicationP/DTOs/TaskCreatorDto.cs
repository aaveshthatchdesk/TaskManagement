using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public  class TaskCreatorDto
    {
        public int TaskItemId { get; set; }
        public int CreatedByUserId { get; set; }
        public string? CreatedByUserName { get; set; }
    }
}
