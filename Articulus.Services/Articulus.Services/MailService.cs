using Articulus.DTOs.Env;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

namespace Articulus.Services
{
    public class MailService
    {
        private readonly MailSettings _mailSetting;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSetting = mailSettings.Value;
        }

        public async Task SendEmail(string toEmail, string recipientName, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Articulus", _mailSetting.Mail));
            message.To.Add(new MailboxAddress(recipientName, toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_mailSetting.SmtpServer, 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_mailSetting.Username, _mailSetting.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        //method to send otp email
        public async Task SendOtpEmail(string toEmail, string recipientName, string otp)
        {
            string subject = "Your OTP Code";
            string body = $"<p>Dear {recipientName},</p>" +
                          $"<p>Your OTP code is: <strong>{otp}</strong></p>" +
                          "<p>This code is valid for 10 minutes.</p>" +
                          "<p>If you did not request this code, please ignore this email.</p>" +
                          "<br><p>Best regards,<br>Articulus Team</p>";

            await SendEmail(toEmail, recipientName, subject, body);
        }
    }
}
