using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using CSS.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CSS.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ApplicationDbContext _db;

        public EmailSender(IOptions<EmailSettings> options, ApplicationDbContext db)
        {
            _settings = options.Value;
            _db = db;
        }

        // ==========================
        // SEND SINGLE EMAIL (existing)
        // ==========================
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        // ==========================
        // SEND EMAIL TO ALL USERS (new)
        // ==========================
        public async Task SendToAllUsers(string subject, string htmlMessage)
        {
            var emails = await _db.Users
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .Select(u => u.Email)
                .ToListAsync();

            foreach (var email in emails)
            {
                await SendEmailAsync(email, subject, htmlMessage);
            }
        }
    }
}
