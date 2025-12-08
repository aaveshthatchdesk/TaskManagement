using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Task.Application.Interaces;
using MailKit.Net.Smtp;
using MimeKit;
using Task.Application.Interaces;
using Microsoft.Extensions.Configuration;

namespace Task.Application.Services
{
    public class EmailService : IEmailService
    {
   
        private readonly IConfiguration _config;

        public EmailService(IConfiguration configuration)
        {
           
            _config = configuration;
        }

        public async Task<bool> SendAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Task System", _config["Email:From"]));
                email.To.Add(new MailboxAddress("", to));
                email.Subject = subject;

                email.Body = new TextPart("html") { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_config["Email:Host"], int.Parse(_config["Email:Port"]), MailKit.Security.SecureSocketOptions.StartTls );
                await smtp.AuthenticateAsync(_config["Email:From"], _config["Email:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Email failed: " + ex.Message);
                return false;
            }
        }
    }
}
