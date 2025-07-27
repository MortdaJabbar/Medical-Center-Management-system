using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Security;
using MCMSDAL;

namespace MCMSBussinessLogic
{
    public static class EmailSender
    {
        public static async Task SendVerificationEmailAsync(string toEmail,Guid UserId)
        {
            string token = Guid.NewGuid().ToString("N");
            DateTime expiry = DateTime.UtcNow.AddHours(1);

             
            await EmailVerificationData.CreateVerificationAsync(UserId, token, expiry);

            string link = $"https://localhost:7119/api/Auth/verify-email?token={token}";

            string app_Password = "wqve lsuq msxr mhwt";
            string from_email   = "mcms993st@gmail.com";
            string htmlBody = $@"
<!doctype html>
<html lang='en' dir='ltr'>
<head>
    <link href='https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap' rel='stylesheet'>
</head>
<body style='font-family: Inter, sans-serif; font-size: 15px; font-weight: 400;'>
    <div style='margin-top: 50px;'>
        <table cellpadding='0' cellspacing='0' style='max-width: 600px; margin: 0 auto; border-radius: 6px; overflow: hidden; background-color: #fff; box-shadow: 0 0 3px rgba(60, 72, 88, 0.15);'>
            <thead>
                <tr style='background-color: #396cf0; text-align: center; color: #fff; font-size: 24px; line-height: 68px;'>
                    <th scope='col'>Medical Center System</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style='padding: 48px 24px 0; color: #161c2d; font-size: 18px; font-weight: 600;'>
                        Hello,
                    </td>
                </tr>
                <tr>
                    <td style='padding: 15px 24px; color: #8492a6;'>
                        Thanks for creating an MCMS account. Please confirm your email by clicking the button below:
                    </td>
                </tr>
                <tr>
                    <td style='padding: 15px 24px;'>
                        <a href='{link}' style='padding: 10px 24px; font-size: 16px; font-weight: 600; border-radius: 6px; background-color: #396cf0; color: #fff; text-decoration: none; display: inline-block;'>Confirm Email Address</a>
                    </td>
                </tr>
                <tr>
                    <td style='padding: 15px 24px 0; color: #8492a6;'>
                        This link will be active for 1 Hour From time it was sent.
                    </td>
                </tr>
                <tr>
                    <td style='padding: 15px 24px 15px; color: #8492a6;'>
                        Medical Center Management System<br>Support Team
                    </td>
                </tr>
                <tr>
                    <td style='padding: 16px 8px; background-color: #f8f9fc; color: #8492a6; text-align: center;'>
                        &copy; {DateTime.Now.Year} Medical Center.
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</body>
</html>";

            var message = new MailMessage("mcms993st@gmail.com", toEmail)
            {
              
       
              Subject = "Email Verification",
              Body = htmlBody,
              IsBodyHtml = true
       
            };

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(from_email, app_Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }

        public static async Task SendTwoFactorCodeAsync(string toEmail, Guid userId)
        {
            string code = new Random().Next(100000, 999999).ToString(); // 6-digit
            DateTime expiry = DateTime.UtcNow.AddMinutes(5);
            string from_email   = "mcms993st@gmail.com";
            string App_Password = "wqve lsuq msxr mhwt";
            await TwoFactorCodeData.CreateCodeAsync(userId, code, expiry);

            string body = $"Your 2FA code is: <b>{code}</b>. It will expire in 5 minutes.";

            var message = new MailMessage(from_email, toEmail)
            {
                Subject = "Your Two-Factor Code",
                Body = body,
                IsBodyHtml = true
            };

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(from_email, App_Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }


        public static async Task SendPasswordResetEmailAsync(string toEmail, Guid userId)
        {
           
            string Generatedtoken = Guid.NewGuid().ToString("N");
            DateTime expirDate = DateTime.UtcNow.AddHours(1);
            var restpasswordtoken = new RestPasswordTokenDto
            { userId = userId, expiry = expirDate, token = Generatedtoken };

            // خزن التوكن بقاعدة البيانات
            await PasswordResetData.CreateResetTokenAsync(restpasswordtoken);

            string link = $"https://localhost:7119/pages/ResetPassword.html?token={restpasswordtoken.token}";

            string from_email = "mcms993st@gmail.com";
            string app_Password = "wqve lsuq msxr mhwt";

            string htmlBody = $@"
<!doctype html>
<html lang='en' dir='ltr'>
<head>
    <link href='https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap' rel='stylesheet'>
</head>
<body style='font-family: Inter, sans-serif; font-size: 15px; font-weight: 400;'>
    <div style='margin-top: 50px;'>
        <table cellpadding='0' cellspacing='0' style='max-width: 600px; margin: 0 auto; border-radius: 6px; overflow: hidden; background-color: #fff; box-shadow: 0 0 3px rgba(60, 72, 88, 0.15);'>
            <thead>
                <tr style='background-color: #396cf0; text-align: center; color: #fff; font-size: 24px; line-height: 68px;'>
                    <th scope='col'>Medical Center System</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style='padding: 48px 24px 0; color: #161c2d; font-size: 18px; font-weight: 600;'>Hello,</td>
                </tr>
                <tr>
                    <td style='padding: 15px 24px; color: #8492a6;'>
                        We received a request to reset your password. Click the button below to proceed:
                    </td>
                </tr>
                <tr>
                    <td style='padding: 15px 24px;'>
                        <a href='{link}' style='padding: 10px 24px; font-size: 16px; font-weight: 600; border-radius: 6px; background-color: #396cf0; color: #fff; text-decoration: none; display: inline-block;'>Reset Password</a>
                    </td>
                </tr>
                <tr>
                    <td style='padding: 15px 24px 0; color: #8492a6;'>
                        This link will expire in 1 hour for security reasons. If you didn't request a reset, you can safely ignore this message.
                    </td>
                </tr>
                <tr>
                    <td style='padding: 15px 24px 15px; color: #8492a6;'>Medical Center Management System<br>Support Team</td>
                </tr>
                <tr>
                    <td style='padding: 16px 8px; background-color: #f8f9fc; color: #8492a6; text-align: center;'>
                        &copy; {DateTime.Now.Year} Medical Center.
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</body>
</html>";

            var message = new MailMessage(from_email, toEmail)
            {
                Subject = "Password Reset Request",
                Body = htmlBody,
                IsBodyHtml = true
            };

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(from_email, app_Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }


    }

}
