using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService
        ){
            _basketRepository = basketRepository; // Talk With Redis DB
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            // 1. Get Basket From Basket Repo
            var basket = await _basketRepository.GetBasketAsync(basketId);

            // 2. Get Selected Items at Basket From Products Repo
            var orderItems = new List<OrderItem>();
            if(basket?.Items?.Count > 0)
            {
                foreach(var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    var productItemOrdered = new ProductItemOrdered(item.Id, product.Name, product.PictureUrl);
                    orderItems.Add(new OrderItem(productItemOrdered, product.Price, item.Quantity));
                }
            }

            // 3. Calculate SubTotal
            var subtotal = orderItems.Sum(I => I.Price * I.Quantity);

            // 4. Get Delivery Method From DeliveryMethods Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // 5. Create Order
            var spec = new OrderWithPaymentIntentIdSpecifications(basket.PaymentIntentId); // Critria => ( ??
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (existingOrder is not null) // !! دي قبل كدا PaymentIntentId معني كدا ان كان فيه اوردر بال
            { 
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateorUpdatePaymentIntent(basket.Id);
            } 

            var order = new Order(buyerEmail, shippingAddress, subtotal, deliveryMethod, orderItems, basket.PaymentIntentId);

            await _unitOfWork.Repository<Order>().AddAsync(order); // Create OrderRepo On The Fly [At Runtime]

            // 6. Save To Database [TODO - UnitOfWork]
            var result = await _unitOfWork.Complete();
            if (result <= 0)
                return null;

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);
            return await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
        }

        public async Task<Order> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        }
    }
}
