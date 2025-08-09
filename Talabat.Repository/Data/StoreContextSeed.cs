using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext dbContext)
        {
            // Seed Brand and Type First, Then Seed Product

            // Seeding For ProductBrand
            if (!dbContext.ProductBrands.Any())
            {
                var brandsData = File.ReadAllText(@"..\Talabat.Repository\Data\DataSeed\brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                if (brands?.Count > 0) // Holding Obj [Not Null]? Not Holding Empty List?
                {
                    //var brandsName = brands.Select(B => new ProductBrand() // Select => Projection Operator => Takes Specific Things From Object
                    //{
                    //    Name = B.Name
                    //}); // Ignore Id (due to Identity Constraint On It) ==> Do It If JSON Contains Values For Id Property

                    foreach (var brand in brands)
                    {
                        await dbContext.Set<ProductBrand>().AddAsync(brand);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }

            // Seeding For ProductType
            if (!dbContext.ProductTypes.Any())
            {
                var typesData = File.ReadAllText(@"..\Talabat.Repository\Data\DataSeed\types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                if (types?.Count > 0)
                {
                    foreach (var type in types)
                    {
                        await dbContext.Set<ProductType>().AddAsync(type);
                    }
                    await dbContext.SaveChangesAsync();
                }
            }

            // Seeding For Product
            if (!dbContext.Products.Any())
            {
                try // Better With Some Improvements.
                {
					var productsData = File.ReadAllText(@"..\Talabat.Repository\Data\DataSeed\products.json");
					var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                    if (products?.Count > 0)
                    {
                        await dbContext.Set<Product>().AddRangeAsync(products);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding products: {ex.Message}");
                }
            }


            // Seeding For Delivery Methods
            if (!dbContext.DeliveryMethods.Any())
            {
                try
                {
                    var deliveryMethodsData = File.ReadAllText(@"..\Talabat.Repository\Data\DataSeed\delivery.json");
                    var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);

                    if (deliveryMethods?.Count > 0)
                    {
                        await dbContext.Set<DeliveryMethod>().AddRangeAsync(deliveryMethods);
                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding products: {ex.Message}");
                }
            }
        }
    }
}
