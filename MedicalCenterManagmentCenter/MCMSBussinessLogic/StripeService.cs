using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCMSBussinessLogic.Configuration;
using Stripe;
using Stripe.Checkout;

namespace MCMSBussinessLogic
{
    public class StripeService : IStripeService
    {
        private readonly StripeSettings _stripeSettings;

        public StripeService(StripeSettings stripeSettings)
        {
            _stripeSettings = stripeSettings;
        }

        public Session CreateStripeSession(decimal amount, string successUrl, string cancelUrl)
        {
            if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
            {
                throw new InvalidOperationException("Stripe secret key is not configured. Set Stripe:SecretKey in appsettings or via environment variable Stripe__SecretKey.");
            }

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Medical Service Payment"
                        }
                    },
                    Quantity = 1
                }
            },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            return service.Create(options);
        }
    }
}
