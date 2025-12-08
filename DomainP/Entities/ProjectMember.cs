using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
    public class ProjectMember
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
