using HMS.Core.Contracts;
using HMS.Core.Entities;
using HMS.InfraStructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.Repositories
{
    internal class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        private readonly HMSDbContext _dbContext;

        public GenericRepository(HMSDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
           => await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();
        
        public async Task<TEntity?> GetByIdAsync(TKey id)
           => await _dbContext.Set<TEntity>().FindAsync(id);
        
        public async Task AddAsync(TEntity entity)
           => await _dbContext.Set<TEntity>().AddAsync(entity);
        
        public void Update(TEntity entity)
           => _dbContext.Set<TEntity>().Update(entity);
        
        public void Delete(TEntity entity)
           => _dbContext.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            Expression<Func<TEntity, object>>? orderByDesc = null,
            List<Expression<Func<TEntity, object>>>? includes = null
            )
        {
            var query = _dbContext.Set<TEntity>().AsQueryable(); // select * from TEntity

            if (filter is not null)
                query = query.Where(filter); // select * from TEntity where filter
            
            if (includes is not null)
            {
                foreach (var include in includes)
                    query = query.Include(include); // select * from TEntity where filter include include1 include include2 ...
            }

            if (orderBy is not null)
                query = query.OrderBy(orderBy); // select * from TEntity where filter order by orderBy

            if (orderByDesc is not null)
                query = query.OrderByDescending(orderByDesc); // select * from TEntity where filter order by orderByDesc desc);


            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(
            TKey id,
            Expression<Func<TEntity, bool>>? filter = null,
            List<Expression<Func<TEntity, object>>>? includes = null
            )
        {
           var query =  _dbContext.Set<TEntity>().AsQueryable();
           
            if (filter is not null)
            { 
                query = query.Where(filter);
            }

            if (includes is not null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }
    }
}
