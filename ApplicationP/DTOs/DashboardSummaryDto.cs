using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int TotalMembers { get; set; }
        public int CompletedTasks { get; set; }
        public int ActiveTasks { get; set; }

        public int OverdueTasks { get; set; }
    }
}
