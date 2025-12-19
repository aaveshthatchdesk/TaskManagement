using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Domain.Entities
{
    public class TaskAttachment
    {

        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;
        public int UploadedByUserId { get; set; }
        public AppUser UploadedByUser { get; set; } = null!;
    }
}
