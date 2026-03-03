namespace MCMSBussinessLogic.Configuration
{
    public sealed class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
