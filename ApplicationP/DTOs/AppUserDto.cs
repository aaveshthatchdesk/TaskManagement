using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Task.Application.DTOs
{
    public class AppUserDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be less than 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage ="Role is required")]
        public string Role { get; set; }

        public string? TempPassword { get; set; }
        public ICollection<TaskAssignmentDto> TaskAssignments { get; set; } = new List<TaskAssignmentDto>();
    }
}
