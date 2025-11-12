using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Task.Application.DTOs
{
    public  class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public int BoardId { get; set; }

        //public BoardDto BoardDto { get; set; } = null!;
        
        public int? SprintId { get; set; }


        //public SprintDto? Sprint { get; set; }
        public string Status {  get; set; } = string.Empty; 

        public int Order { get; set; }

        public ICollection<TaskAssignmentDto> TaskAssignments { get; set; } = new List<TaskAssignmentDto>();
    }
}
