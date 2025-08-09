using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Helpers;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Services;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection  services)
        {
            ///services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            ///services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
            ///services.AddScoped<IGenericRepository<ProductType>, GenericRepository<ProductType>>();
            /* Better => */
            //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork)); // No Need To Allow DI For GenericRepository

            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository)); // The Generic Way [We Can Do It By Normal Way <, >]
            
            services.AddScoped(typeof(IOrderService), typeof(OrderService));

			// Payment Service
			services.AddScoped(typeof(IPaymentService), typeof(PaymentService));

            // Caching Service
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();


			services.AddAutoMapper(typeof(MappingProfiles));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) => // Modifing Behaviour Of This Factory That Generates Response For Endpoint That Has Invalid Model State / Validation Error
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0) // Array Of Params/Models (Key : Val Pairs) With Invalid ModelState
                                                         .SelectMany(P => P.Value.Errors) // Array Of Errors Contains Object/s
                                                         .Select(E => E.ErrorMessage) // Array Of Messages 
                                                         .ToArray();

                    var validationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(validationErrorResponse);
                };
            });

            
            return services;
        }
    }
}
