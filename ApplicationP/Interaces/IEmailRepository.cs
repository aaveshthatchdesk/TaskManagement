using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.Application.Interaces
{
    public interface IEmailRepository
    {
        Task<bool> SendAsync(string to, string subject, string body);
    }
}
