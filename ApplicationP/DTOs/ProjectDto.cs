using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;


namespace Task.Application.DTOs
{
   public  class ProjectDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Project name is required")]
        [StringLength(100,ErrorMessage="Project name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage ="Priority is required")]
        [RegularExpression("^(Low|Medium|High)$",ErrorMessage ="Priority must be Low,Medium, or High")]
        public string Priority { get; set; } = "Medium";

        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }

        public int Progress { get; set; }
        [Required]
        [RegularExpression("^(Public|Private)$", ErrorMessage = "Visibility must be Public or Private")] 
        public string Visibility { get; set; } = "Public";
        public int MemberCount { get; set; }
        public List<string> MemberIntials { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public ICollection<BoardDto> Boards { get; set; } = new List<BoardDto>();
    }
 
}
