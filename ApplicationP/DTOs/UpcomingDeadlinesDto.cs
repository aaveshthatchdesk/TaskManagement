using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public  class UpcomingDeadlinesDto
    {
        public int TaskId { get; set; }
        public string Title { get; set; }=string.Empty;
        public DateTime DueDate { get; set; }

        public string Priority { get; set; } = string.Empty;

        public int ProjectId { get; set; }
        public string ProjectName {  get; set; } = string.Empty;

        public string BoardName {  get; set; } = string.Empty;
    }
}
