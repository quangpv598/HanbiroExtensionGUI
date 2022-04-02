using HanbiroExtensionGUI.Models;
using HanbiroExtensionGUI.Resources;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanbiroExtensionGUI.Services
{
    public interface IEmailService
    {
        Task Send(string from, string to, string subject, string message, string imagePath);
    }

    public class EmailService : ServicesBase, IEmailService
    {
        public EmailService(CurrentUserSettings userSettings) : base(userSettings)
        {

        }

        public async Task Send(string from, string to, string subject, string message, string imagePath)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder();
            var image = builder.LinkedResources.Add(imagePath);

            image.ContentId = MimeUtils.GenerateMessageId();

            builder.HtmlBody = string.Format(@"<p>{0}!</p><img src=""cid:{1}"">", message, image.ContentId);


            email.Body = builder.ToMessageBody();

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(AppSettings.SmtpHost, 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(AppSettings.SmtpUser, AppSettings.SmtpPass);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
