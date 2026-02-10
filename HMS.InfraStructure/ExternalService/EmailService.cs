using HMS.ServiceAbstraction;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace HMS.InfraStructure.ExternalService
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // comine message
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_settings.Value.From),
                Subject = subject
            };

            mail.To.Add(MailboxAddress.Parse(to));
            mail.From.Add(new MailboxAddress(_settings.Value.DisplayName, _settings.Value.From));

            var builder = new BodyBuilder();

            builder.TextBody = body;

            mail.Body = builder.ToMessageBody(); 

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(_settings.Value.SmtpServer
                , _settings.Value.Port
                , MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_settings.Value.From, _settings.Value.Password);

            await smtp.SendAsync(mail);
                
            await smtp.DisconnectAsync(true);


        }
    }
}
