using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public  class TaskReorderDto
    {
        public int TaskId { get; set; }
        public int BoardId { get; set; }
        public int Order { get; set; }
    }
}
