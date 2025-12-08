using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Task.Application.DTOs
{
    public class BoardDto
    {

        public int Id { get; set; }
        [Required(ErrorMessage="Board name is required")]
        public string Name { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        //public ProjectDto ProjectDto { get; set; } = null!;
        public ICollection<TaskItemDto> TaskItems { get; set; } = new List<TaskItemDto>();
    }
}

