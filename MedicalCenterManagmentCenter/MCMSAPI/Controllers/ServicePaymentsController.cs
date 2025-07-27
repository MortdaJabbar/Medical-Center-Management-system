using MCMSBussinessLogic;
using MCMSDAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MCMSAPI.Controllers
{
    [Route("api/ServicePayments")]
    [ApiController]
    public class ServicePaymentsController : ControllerBase
    {
 
            private readonly ServicePayment _logic = new();
            private readonly StripeService _stripe = new();

            // ✅ 1. إنشاء جلسة دفع Stripe + حفظ الدفع
            [HttpPost("create-stripe-session")]
            public async Task<IActionResult> CreateStripeSession([FromBody] ServicePaymentDto dto)
            {
                try
                {
                    // إنشاء جلسة Stripe
                    var session = _stripe.CreateStripeSession(dto.Amount,
                        successUrl: "https://yoursite.com/success?session_id={CHECKOUT_SESSION_ID}",
                        cancelUrl: "https://yoursite.com/cancel");

                    // حفظ الدفع في قاعدة البيانات مع Stripe IDs
                    var result = await _logic.AddPaymentAsync(dto, session.Id, session.PaymentIntentId);

                    if (!result)
                        return BadRequest("Payment creation failed");

                    return Ok(new
                    {
                        sessionId = session.Id,
                        url = session.Url
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Stripe error: {ex.Message}");
                }
            }

            // ✅ 2. جلب جميع مدفوعات مريض معيّن
            [HttpGet("patient/{patientId}")]
            public async Task<IActionResult> GetByPatient(Guid patientId)
            {
                var payments = await _logic.GetPaymentsForPatientAsync(patientId);
                return Ok(payments);
            }

            // ✅ 3. تحديث حالة الدفع يدوياً (اختياري، للإداري فقط)
            [HttpPut("{paymentId}/status")]
            public async Task<IActionResult> UpdateStatus(int paymentId, [FromBody] PaymentUpdateRequest request)
            {
                var result = await _logic.UpdatePaymentStatusAsync(paymentId, request.PaymentStatus, request.Notes);
                return result ? Ok() : BadRequest("Update failed");
            }

            // ✅ 4. حذف دفع (اختياري، فقط من قبل الإدارة أو للتجارب)
            [HttpDelete("{paymentId}")]
            public async Task<IActionResult> Delete(int paymentId)
            {
                var result = await _logic.DeletePaymentAsync(paymentId);
                return result ? Ok() : NotFound();
            }

            // DTO داخلي للتحديث
            public class PaymentUpdateRequest
            {
                public string PaymentStatus { get; set; } = string.Empty;
                public string? Notes { get; set; }
            }
        }
    }

