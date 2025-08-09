using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Stripe;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class PaymentsController : ApiBaseController
	{
		private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private const string _webhookSecret = "whsec_9m5rjYaO7QutGuGqv2a8DHoXBYXEOGKm";
        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger) 
		{
			_paymentService = paymentService;
            _logger = logger;
        }


        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost("{basketId}")] // POST : /api/payments/basketId
				   // Consumer Need To Create PaymentIntent, We Response With CustomerBasket Within PaymentIntentId and ClientSecret
		public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var basket = await _paymentService.CreateorUpdatePaymentIntent(basketId);

			if (basket is null) return BadRequest(new ApiResponse(400, "a problem with your basket."));

			return Ok(basket);
		}


        // POST : https://localhost:7092/api/payments/webhook
		[HttpPost("webhook")] // POST : /api/payments/webhook
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            var signatureHeader = Request.Headers["Stripe-Signature"];

            var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _webhookSecret);

            var paymentIntent = (PaymentIntent) stripeEvent.Data.Object;

            Order order;

            // Handle the event
            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, true);
                    _logger.LogInformation("Payment is Succeeded ya Hamoksha :)", paymentIntent.Id);
                    break;

                case EventTypes.PaymentIntentPaymentFailed:
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                    _logger.LogInformation("Payment is Failed ya Hamoksha :(", paymentIntent.Id);
                    break;
            }

            return Ok(); // No Need To Return Anything
        }
            

    }
}
