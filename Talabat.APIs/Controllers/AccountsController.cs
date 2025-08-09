using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	public class AccountsController : ApiBaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService,
			IMapper mapper) 
		{
			_userManager = userManager;
			_signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

		[HttpPost("login")] // POST :  /api/accounts/login
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		{
			/*if (ModelState.IsValid) ==> Handled In Program.cs, Validation Error Done Automatically, So We Don't Need To Check ModelState Validatity لو مش فاليد اصلا مش هيدخل في الاندبوينت*/

			var user = await _userManager.FindByEmailAsync(model.Email); // Use FindByNameAsync If You Use UserName Instead Of Email

			if (user is null) return Unauthorized(new ApiResponse(StatusCodes.Status401Unauthorized));
			
			/*user not null => I Have a User With This Email*/
			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

			if (!result.Succeeded) return Unauthorized(new ApiResponse(StatusCodes.Status401Unauthorized));

			/*result succeeded => Password Of This User Is Correct*/
			return Ok(new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			});
		}

		[HttpPost("register")] // POST : /api/accounts/register
		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			// .Result Returns ActionResult, .Value Returns The Value Inside ActionResult
			if (CheckEmailExists(model.Email).Result.Value) // Here I Need Check First Before REgister So: I'll Not Using await Before It {The Code After It Works At the Same Time Because Threading [I Don't Want This]}, I Need This Condition Blocks All Code Comes After It..
				return BadRequest(new ApiValidationErrorResponse() { Errors = new string[] {"this email is already in use!!"} });

			var user = new AppUser() /*Manual Mapping*/
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				PhoneNumber = model.PhoneNumber,
				/*UserName Is Required, You Must Set It and Must Be Unique*/
				UserName = model.Email.Split('@')[0]
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded) return BadRequest(new ApiResponse(StatusCodes.Status400BadRequest));

			return Ok(new UserDto() { 
				Email = user.Email,
				DisplayName = user.DisplayName,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });
		}

		[Authorize]
		[HttpGet] // GET : /api/accounts
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var email = User.FindFirstValue(ClaimTypes.Email);

			var user = await _userManager.FindByEmailAsync(email);
			/*
				=> This User Always Come With Value, Not Null, Because He Is Always Authorized Inside This Action/Endpoint
			*/

			return Ok(new UserDto()
			{
				DisplayName = user.DisplayName,
				Email = user.Email,
				Token = await _tokenService.CreateTokenAsync(user, _userManager)
			});
		}

        [Authorize]
        [HttpGet("address")] // GET : /api/accounts/address
		public async Task<ActionResult<AddressDto>> GetUserAddress()
		{
            //var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManager.FindUserWithAddressAsync(User);

			// Auto Mapper
			var address = _mapper.Map<Address, AddressDto>(user.Address);

            return Ok(address);
		}

        [Authorize]
        [HttpPut("address")] // PUT : /api/accounts/address
																	/*I Must Put Validations In This Dto To Validate On Incoming Data*/
		public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto updatedAddress)
		{
			// We Handled ModelState/ValidationError Already على مستوى البروجكت كله

			var address = _mapper.Map<AddressDto, Address>(updatedAddress);

            var user = await _userManager.FindUserWithAddressAsync(User);

			address.Id = user.Address.Id;

			/*Here Obj State Will Changed To Be Modified, Then He Will Update The Address*/
			user.Address = address; /*If Nav Prop [Address] Not Loaded, Then {Address=null} => {NotTracked} and It Will Consider The address Is a New Address And Create/Insert It Because ObjState Of Address Will Be Added*/

			// Or Update Manual (Better) =>
			///user.Address.FirstName = address.FirstName;
			///user.Address.LastName = address.LastName;
			///user.Address.Street = address.Street;
			///user.Address.City = address.City;
			///user.Address.Country = address.Country;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return Ok(updatedAddress);
		}

		/*The User Is Now Register, Not Have To Be Authorized !!*/
		[HttpGet("emailexists")] // GET : /api/accounts/emailexists?email=ahmed.mo.elgebaly@gmail.com
		public async Task<ActionResult<bool>> CheckEmailExists(string email)
		{
			return await _userManager.FindByEmailAsync(email) is not null;
		}

    }
}
