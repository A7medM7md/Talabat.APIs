using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class ProductWithBrandAndTypeSpecifications : BaseSpecification<Product>
	{
		// This Constructor Is Used To "GetAllProducts()"
		public ProductWithBrandAndTypeSpecifications(ProductSpecParams specParams) // Chain On EPLC Of BaseSpecification By Default If Yu Not Specifying Another
			: base(P =>
						   (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search))
						&& (!specParams.BrandId.HasValue || P.ProductBrandId == specParams.BrandId.Value)
						&& (!specParams.TypeId.HasValue || P.ProductTypeId == specParams.TypeId.Value)

            ) 
        {
			Includes.Add(P => P.ProductBrand);
			Includes.Add(P => P.ProductType);

			// If You Write This => The default Sort Become By Name
			AddOrderBy(P => P.Name);

			if (!string.IsNullOrEmpty(specParams.Sort))
			{
				switch(specParams.Sort)
				{
					case "priceAsc" : 
						AddOrderBy(P => P.Price);
						//OrderBy = P => P.Price; // => Func Is More Readable Than This !!
						break;
					case "priceDesc" : 
						AddOrderByDesc(P => P.Price);
						break;
					default : 
						AddOrderBy(P => P.Name);
						break;
				};
			}

			// Suppose :
			// TotalProducts = 18
			// PageSize = 5
			// PageIndex = 1
			int skipSteps = (specParams.PageIndex - 1) * specParams.PageSize;
			ApplyPagenation(skipSteps, specParams.PageSize);
		}

		// This Constructor Is Used To "GetProductById()"
		public ProductWithBrandAndTypeSpecifications(int id) : base(P => P.Id == id)
		{
			Includes.Add(P => P.ProductBrand);
			Includes.Add(P => P.ProductType);
		}

	}
}
