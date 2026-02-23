namespace MCMSBussinessLogic.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendTwoFactorCodeAsync(string toEmail, Guid userId);
    }
}
