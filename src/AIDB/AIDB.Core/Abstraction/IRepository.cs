using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIDB.Core.Abstraction;

public interface IRepository<Tkey, TEntity>
    where Tkey : IConvertible
    where TEntity : DbModel<Tkey>
{
    IQueryable<TEntity> AsQueryable();
    Task<TEntity?> GetAsync(Tkey id);
    Task<bool> AddAsync(TEntity? entity);
    Task<bool> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<bool> RemoveAsync(Tkey id);
    Task<bool> RemoveAsync(TEntity? entity);
    Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entity);
    Task UpdateAsync(TEntity? entity);
    Task UpdateRange(IEnumerable<TEntity> entities);
    Task SaveChangesAsync();
}