using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class ProjectCreateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        [StringLength(100, ErrorMessage = "Project name cannot exceed 200 characters")]
        public string Description { get; set; } = string.Empty;
        [Required]
        [RegularExpression("^(Public|Private)$", ErrorMessage = "Visibility must be Public or Private")]
        public string Visibility { get; set; } = "Public";

        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        public List<int> MemberIds { get; set; } = new();
        public List<int> ManagerIds { get; set; } = new();



    }
}
