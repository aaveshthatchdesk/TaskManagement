using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
   public  class Board
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}
