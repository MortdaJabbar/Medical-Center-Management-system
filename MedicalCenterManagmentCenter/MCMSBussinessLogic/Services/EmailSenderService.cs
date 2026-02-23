using MCMSBussinessLogic.Interfaces;

namespace MCMSBussinessLogic.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        public Task SendTwoFactorCodeAsync(string toEmail, Guid userId)
        {
            return EmailSender.SendTwoFactorCodeAsync(toEmail, userId);
        }
    }
}
