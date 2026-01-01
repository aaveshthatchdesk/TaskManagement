using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class TaskReorderForMembersDto
    {

        public int TaskId { get; set; }
        public string TargetBoardName { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
