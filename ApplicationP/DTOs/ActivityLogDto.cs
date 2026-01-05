using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public  class ActivityLogDto
    {

        public int ProjectId { get; set; }
        public int? TaskItemId { get; set; }
        public int? BoardId { get; set; }

        public string ProjetName { get; set; }=string.Empty;
        public int PerformedByUserId { get; set; }


        public int? TargetUserId { get; set; }
        public string? TargetUserName { get; set; }
        public string PerformedByUserName { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string ActionType { get; set; } = string.Empty;
    }
}
