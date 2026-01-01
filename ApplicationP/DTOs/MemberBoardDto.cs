using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public  class MemberBoardDto
    {

        public int Id { get; set; }
        public string BoardName { get; set; }= string.Empty;
        public List<TaskItemDto> Tasks { get; set; } = new();
    }
}
