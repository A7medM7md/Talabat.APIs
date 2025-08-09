using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : ApiBaseController
    {

        ///private readonly IGenericRepository<Product> _productRepo;
        ///private readonly IGenericRepository<ProductBrand> _brandsRepo;
        ///private readonly IGenericRepository<ProductBrand> _typesRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(
            ///IGenericRepository<Product> productRepo,
            ///IGenericRepository<ProductBrand> brandsRepo,
            ///IGenericRepository<ProductBrand> typesRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            ///_productRepo = productRepo;
            ///_brandsRepo = brandsRepo;
            ///_typesRepo = typesRepo;
            _unitOfWork = unitOfWork;

            _mapper = mapper;
        }

        //[AllowAnonymous] // Default : Any One Can Call It
        //[Authorize]
        [CachedAttribute(600)] // [Action Filter] Cached 600 Seconds [600/60 -> 10 min]
		[HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(specParams);

            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var countSpec = new ProductWithFilterationForCountSpecification(specParams);

            var count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(countSpec);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, count, data));
        }

        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[CachedAttribute(600)]
		[HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
			var spec = new ProductWithBrandAndTypeSpecifications(id);

			var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

            if (product is null)
                return NotFound(new ApiResponse(404));
            
            return Ok(_mapper.Map<Product, ProductToReturnDto>(product));
        }

		[CachedAttribute(600)]
		[HttpGet("brands")] // GET : api/products/brands
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();

            return Ok(brands);
        }

		[CachedAttribute(600)]
		[HttpGet("types")] // GET : api/products/types
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await _unitOfWork.Repository<ProductType>().GetAllAsync();

            return Ok(types);
        }

    }
}
