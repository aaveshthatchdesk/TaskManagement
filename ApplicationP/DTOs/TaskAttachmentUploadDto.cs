using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class TaskAttachmentUploadDto
    {

        public IFormFile File { get; set; } = null!;
    }
}
