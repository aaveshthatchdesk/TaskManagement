using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public  class SprintDto
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Sprint name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        
        public DateTime EndDate { get; set; }


        //public ICollection<TaskItemDto> TaskItems { get; set; } = new List<TaskItemDto>();
        //public List<BoardDto> Boards { get; set; }=new List<BoardDto>();
        //public List<ProjectDto> Projects { get; set; } = new List<ProjectDto>();



        //public string? ProjectName { get; set; }
        public string Status { get; set; } = "Pending";
        //public int CompletedTasks { get; set; }
        //public int TotalTasks { get; set; }
        //public List<AppUserDto> AssignedUsers { get; set; } = new();
    }
}
