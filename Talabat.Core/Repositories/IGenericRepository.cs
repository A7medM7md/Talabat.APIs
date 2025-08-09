using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity // T is always a Domain Model
    {
        // With Static Query
        public Task<IReadOnlyList<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);

        // With Dynamic Query
		public Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);
		public Task<T> GetEntityWithSpecAsync(ISpecification<T> spec);

        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);


        Task AddAsync(T entity); // entity Is The Record/Row That You'll Add In Repo/DbSet/Table
        void Update(T entity);
        void Delete(T entity);
	}
}
