using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }

        public bool IsTemporaryPassword { get; set; }
        public int UserId {  get; set; }
        public string Role {  get; set; }
    }
}
