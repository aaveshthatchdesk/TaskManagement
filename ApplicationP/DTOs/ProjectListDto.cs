using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class ProjectListDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Visibility {  get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Progress { get; set; }
        public List<string> MemberIntials { get; set; } = new();
        public string ManagerNames { get; set; } = "";
    }
}
