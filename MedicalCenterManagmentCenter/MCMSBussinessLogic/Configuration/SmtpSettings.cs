namespace MCMSBussinessLogic.Configuration
{
    public sealed class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string FromEmail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
    }
}
