using HMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Contracts
{
    public interface IGenericRepository<TEntity,TKey>
        where TEntity : BaseEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(
               Expression<Func<TEntity, bool>>? filter = null,
               Expression<Func<TEntity, object>>? orderBy = null,
               Expression<Func<TEntity, object>>? orderByDesc = null,
               List<Expression<Func<TEntity, object>>>? includes = null
           );
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<TEntity?> GetByIdAsync(
            TKey id,
            Expression<Func<TEntity, bool>>? filter = null,
            List<Expression<Func<TEntity, object>>>? includes = null
            );
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
