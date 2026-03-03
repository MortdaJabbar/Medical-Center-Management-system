using MCMSBussinessLogic.Configuration;
using MCMSBussinessLogic.Interfaces;
using MCMSDAL.Interfaces;
using System.Net;
using System.Net.Mail;

namespace MCMSBussinessLogic.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly ITwoFactorCodeData _twoFactorCodeData;
        private readonly SmtpSettings _smtpSettings;

        public EmailSenderService(ITwoFactorCodeData twoFactorCodeData, SmtpSettings smtpSettings)
        {
            _twoFactorCodeData = twoFactorCodeData;
            _smtpSettings = smtpSettings;
        }

        public async Task SendTwoFactorCodeAsync(string toEmail, Guid userId)
        {
            string code = new Random().Next(100000, 999999).ToString();
            DateTime expiry = DateTime.UtcNow.AddMinutes(5);

            await _twoFactorCodeData.CreateCodeAsync(userId, code, expiry);

            if (string.IsNullOrWhiteSpace(_smtpSettings.Host)
                || string.IsNullOrWhiteSpace(_smtpSettings.FromEmail)
                || string.IsNullOrWhiteSpace(_smtpSettings.AppPassword))
            {
                throw new InvalidOperationException("SMTP settings are not configured. Set Email:Smtp:Host, Email:Smtp:FromEmail and Email:Smtp:AppPassword in appsettings.json.");
            }

            string fromEmail = _smtpSettings.FromEmail;
            string appPassword = _smtpSettings.AppPassword;

            string body = $"Your 2FA code is: <b>{code}</b>. It will expire in 5 minutes.";

            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Your Two-Factor Code",
                Body = body,
                IsBodyHtml = true
            };

            using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = _smtpSettings.EnableSsl
            };

            await smtp.SendMailAsync(message);
        }
    }
}
