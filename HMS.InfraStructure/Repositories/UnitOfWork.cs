using HMS.Core.Contracts;
using HMS.Core.Entities;
using HMS.InfraStructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.InfraStructure.Repositories
{
    public class UnitOfWork(HMSDbContext dbContext) : IUnitOfWork
    {
        private Dictionary<Type, object> _repositories = [];
        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var type = typeof(TEntity);

            if (_repositories.TryGetValue(type, out var repository))
                return (IGenericRepository<TEntity, TKey>)repository;
            
            var newRepository = new GenericRepository<TEntity, TKey>(dbContext);

            _repositories[type] = newRepository;

            return newRepository;
        }

        public async Task<int> SaveChangesAsync()
            => await dbContext.SaveChangesAsync();
    }
}
