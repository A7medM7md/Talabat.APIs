using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;
using Product = Talabat.Core.Entities.Product; /*Alias Name*/

namespace Talabat.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _config;
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(IConfiguration config,
			IBasketRepository basketRepository,
			IUnitOfWork unitOfWork
		)
        {
			_config = config;
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
		}
        public async Task<CustomerBasket?> CreateorUpdatePaymentIntent(string basketId)
		{
			/// Configure Secret Key
			StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

			var basket = await _basketRepository.GetBasketAsync(basketId);

			if (basket is null) return null;

			if (basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
				basket.ShippingCost = deliveryMethod.Cost;
			}

			if(basket?.Items?.Count() > 0)
			{
				foreach (var item in basket.Items)
				{
					var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
					
					if(item.Price != product.Price)
						item.Price = product.Price;
				}
			}

			PaymentIntent paymentIntent;

			var service = new PaymentIntentService();

			/* Create Or Update Payment Intent ?! */
			if(string.IsNullOrEmpty(basket.PaymentIntentId))
			{
				var options = new PaymentIntentCreateOptions()
				{
					Amount = (long)basket.Items.Sum(Item => Item.Price * Item.Quantity * 100) + (long)basket.ShippingCost * 100,
					Currency = "usd",
					PaymentMethodTypes = new List<string>() { "card" }
				};

				paymentIntent = await service.CreateAsync(options);
				basket.PaymentIntentId = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}
			else // Update Payment Intent
			{
				var options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)basket.Items.Sum(Item => Item.Price * Item.Quantity * 100) + (long)basket.ShippingCost * 100
				};

				await service.UpdateAsync(basket.PaymentIntentId, options);
			}

			await _basketRepository.UpdateBasketAsync(basket);

			return basket;
		}

        public async Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool isSucceeded)
        {
			var spec = new OrderWithPaymentIntentIdSpecifications(paymentIntentId);
			var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

			if (isSucceeded)
				order.Status = OrderStatus.PaymentReceived;
			else
                order.Status = OrderStatus.PaymentFailed;

			_unitOfWork.Repository<Order>().Update(order);

			await _unitOfWork.Complete();

			return order;
        }
    }
}
