using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Contracts
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : Entities.BaseEntity<TKey>;
        Task<int> SaveChangesAsync();
    }
}
