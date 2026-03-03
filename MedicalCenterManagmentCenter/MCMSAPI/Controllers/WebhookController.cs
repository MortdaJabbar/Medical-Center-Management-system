using MCMSBussinessLogic;
using MCMSBussinessLogic.Configuration;
using MCMSBussinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace MCMSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {

        private readonly IServicePayment _servicePayment;

        private readonly StripeSettings _stripeSettings;

        public WebhookController(IServicePayment servicePayment, StripeSettings stripeSettings)
        {
            _servicePayment = servicePayment;
            _stripeSettings = stripeSettings;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(_stripeSettings.WebhookSecret))
            {
                return StatusCode(500, "Stripe webhook secret is not configured. Set Stripe:WebhookSecret in appsettings.json.");
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _stripeSettings.WebhookSecret
                );

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;

                    // تحديث حالة الدفع في قاعدة البيانات
                    await _servicePayment.MarkPaymentCompletedFromStripeAsync(session.Id);
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }
        }
    }
}

