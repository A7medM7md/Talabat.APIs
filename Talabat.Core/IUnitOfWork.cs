using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;

namespace Talabat.Core
{
    public interface IUnitOfWork : IAsyncDisposable // I May Implement This Interface More Than One Time, If I Have More Than One DbContext / DB
    {
        // Property Signature For Each Repo, For Each Table

        IGenericRepository<TRepo> Repository<TRepo>() where TRepo : BaseEntity;

        Task<int> Complete();

        // The One He Creates Obj From DbContext [Open Connection] (UnitOfWork) Is Responsible To Close/Dispose This Connection, So He Must Has DisposeAsync(); Method Inherited From IAsyncDisposable
    }
}
