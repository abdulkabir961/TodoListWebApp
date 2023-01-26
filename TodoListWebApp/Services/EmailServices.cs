using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListWebApp.Interface;
using TodoListWebApp.Models;

namespace TodoListWebApp.Services
{
    public class EmailServices
    {
        public class EmailService : IEmailService
        {
            private readonly EmailServerConfig _emailConfig;
            public EmailService(EmailServerConfig emailConfig)
            {
                _emailConfig = emailConfig;
            }
            public async Task SendEmailAsync(EmailMessageContent message)
            {
                var emailMessage = CreateEmailMessage(message);
                await SendAsync(emailMessage);
            }

            private MimeMessage CreateEmailMessage(EmailMessageContent message)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("INTERNS TODO ALERT", _emailConfig.From));
                emailMessage.To.AddRange(message.To);
                emailMessage.Subject = message.Subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<p>{0}</p>", message.Content) };
                return emailMessage;
            }
            private async Task SendAsync(MimeMessage mailMessage)
            {
                using (var client = new SmtpClient())
                {
                    try
                    {
                        await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                        await client.SendAsync(mailMessage);
                    }
                    catch
                    {
                        //log an error message or throw an exception or both.
                        throw;
                    }
                    finally
                    {
                        client.Disconnect(true);
                        client.Dispose();
                    }
                }
            }
        }
    }
}
