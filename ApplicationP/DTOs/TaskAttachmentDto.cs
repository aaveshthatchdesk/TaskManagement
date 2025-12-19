using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class TaskAttachmentDto
    {

        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedOn { get; set; }
        public int UploadedByUserId { get; set; }
        public string UploadedByUserName { get; set; } = string.Empty;
    }
}
