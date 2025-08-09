using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrdersController : ApiBaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)] // This Endpoint In Swagger, Produce 2 Forms From Response
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)] // This Endpoint In Swagger, Produce 2 Forms From Response
        [HttpPost] // POST : /api/orders
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto) // Instead Of Passing 3 Params Needed As Query String or Segment.. [BasketId, DeliveryMethodId, ShippingAddress], I Can Get Them FromBody By Wrapping Them In Class and Take Obj From This Class
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email); // User That Logging-In [Has The Token]
            var address = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
            var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, address);

            if (order is null) return BadRequest(new ApiResponse(400));

            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }

        [HttpGet] // GET : /api/orders
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var orders = await _orderService.GetOrdersForUserAsync(User.FindFirstValue(ClaimTypes.Email));
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")] // GET : /api/orders/id
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var order = await _orderService.GetOrderByIdForUserAsync(id, User.FindFirstValue(ClaimTypes.Email));

            if (order is null) return NotFound(new ApiResponse(404));
            
            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }

        [HttpGet("deliveryMethods")] // GET : /api/orders/deliveryMethods
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await _orderService.GetDeliveryMethodsAsync());
        }

    }
}
