using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
	public static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
	{
		public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
		{
			var query = inputQuery;

			if (spec.Criteria is not null)
				query = query.Where(spec.Criteria);

			if(spec.OrderBy is not null) // P => P.Price
				query = query.OrderBy(spec.OrderBy); // _dbContext.Set<Product>().OrderBy(P => P.Price)
            
			if (spec.OrderByDescending is not null) // P => P.Price
                query = query.OrderByDescending(spec.OrderByDescending); // _dbContext.Set<Product>().OrderByDescending(P => P.Price)

			if(spec.IsPagenationEnabled)
				query = query.Skip(spec.Skip).Take(spec.Take); // Apply Pagenation


            query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

			return query;
		}
	}
}
