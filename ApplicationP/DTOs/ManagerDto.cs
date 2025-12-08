using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public  class ManagerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role {  get; set; }
        public int ProjectCount {  get; set; }
        public int TeamCount {  get; set; }
    }
}
