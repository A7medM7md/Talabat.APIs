using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        private Hashtable _repositories;

        public UnitOfWork(StoreContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }

        public IGenericRepository<TRepo> Repository<TRepo>() where TRepo : BaseEntity
        {
            var typeName = typeof(TRepo).Name;

            if (!_repositories.ContainsKey(typeName))
            {
                var repoInstance = new GenericRepository<TRepo>(_dbContext);
                _repositories.Add(typeName, repoInstance);
            }

            return (IGenericRepository<TRepo>) _repositories[typeName];
        }
   
        public async Task<int> Complete()
            => await _dbContext.SaveChangesAsync();
        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();

    }
}
