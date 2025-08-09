using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T : BaseEntity
	{
		// 2 Automatic Properties Implements 2 Property Signatures
		public Expression<Func<T, bool>> Criteria { get; set; }
		public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>(); // Before Executing Ctor : This Initialization Executes !
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
		public int Skip { get; set; }
		public int Take { get; set; }
		public bool IsPagenationEnabled { get; set; }

		public BaseSpecification() // No Need Of Filters , No Where Linq Operator On This Query ==> With GetAllProducts()
		{
			//Includes = new List<Expression<Func<TEntity, object>>>();
        }

		public BaseSpecification(Expression<Func<T, bool>> criteriExpression) // Need Of Filters/Criteria , There Is Where Linq Operator On This Query ==> With GetAProductById()
		{
			Criteria = criteriExpression; // P => (P.ProductBrandId == 1 && true)
			//Includes = new List<Expression<Func<TEntity, object>>>(); // We Need To Includes Also, We Will Include Nav Properties
		}


		public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
			=> OrderBy = orderByExpression;

		public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
			=> OrderByDescending = orderByDescExpression;
    
		public void ApplyPagenation(int skip, int take)
		{
			IsPagenationEnabled = true;
			Skip = skip;
			Take = take;
		}
	}
}
