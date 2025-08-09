using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
	public class BasketsController : ApiBaseController
	{
		private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketsController(IBasketRepository basketRepository, IMapper mapper)
        {
			_basketRepository = basketRepository;
            _mapper = mapper;
        }

		//[HttpGet("{id}")] // GET : /api/baskets/id
		[HttpGet] // GET : /api/baskets?id=1
		public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string id) // Used For Get Basket Or ReCreate Expired Basket
		{
			var basket = await _basketRepository.GetBasketAsync(id);
			return basket is null? new CustomerBasket(id) : Ok(basket);
		}

		[HttpPost] // POST : /api/baskets
		/*I Makes This Dto For Validation Only, I Don't Need To Return The Dto*/
		public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket) // Used In Update Basket Or Creating Basket For First Time
		{
			var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);

			var createdOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);

			if (createdOrUpdatedBasket is null)
				return BadRequest(new ApiResponse(400));

			return Ok(createdOrUpdatedBasket);
		}

		[HttpDelete()] // DELETE : api/baskets?id=1
		public async Task<ActionResult<bool>> Delete(string id)
		{
			return await _basketRepository.DeleteBasketAsync(id);
		}


	}
}
