using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
    public class Sprint
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<TaskItem>TaskItems { get; set; }=new List<TaskItem>();   
    }
}
