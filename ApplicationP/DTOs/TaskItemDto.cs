using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.Domain.Entities;


namespace Task.Application.DTOs
{
    public  class TaskItemDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required")]
        [RegularExpression("Low|Medium|High", ErrorMessage = "Priority must be Low, Medium, or High")]
        public string? Priority {  get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Due date must be a future date")]
        public DateTime? DueDate { get; set; }

        public int BoardId { get; set; }

        //public BoardDto BoardDto { get; set; } = null!;
        public string BoardName { get; set; } = string.Empty;

        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }

        public int? SprintId { get; set; }


        //public SprintDto? Sprint { get; set; }
        public string? Status {  get; set; }

        public int Order { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CreatedOn { get; set; }

        public DateTime? LastUpdatedOn { get; set; }
       

        public List<TaskAttachmentDto>? TaskAttachments { get; set; } = new List<TaskAttachmentDto>();
        public List<TaskCommentDto>? TaskComments { get; set; } = new List<TaskCommentDto>();

        public ICollection<TaskAssignmentDto> TaskAssignments { get; set; } = new List<TaskAssignmentDto>();
        public ICollection<TaskCreatorDto> TaskCreators { get; set; } = new List<TaskCreatorDto>();
    }
}
