using RealStateApp.Application.Interfaces.Repositories;
using RealStateApp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RealStateApp.Infrastructure.Persistence.Repositories;

public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryAsync(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllWithIncludeAsync(List<string> properties)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        foreach (var property in properties)
        {
            query = query.Include(property);
        }
        return await query.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        var entry = _dbContext.Entry(entity);
        var primaryKey = entry.Metadata.FindPrimaryKey();
        if (primaryKey != null)
        {
            var keyValues = primaryKey.Properties.Select(p => entry.Property(p.Name).CurrentValue).ToArray();
            var localEntity = _dbContext.Set<T>().Local.FirstOrDefault(e => 
                primaryKey.Properties.Select(p => _dbContext.Entry(e).Property(p.Name).CurrentValue)
                    .SequenceEqual(keyValues));
            if (localEntity != null)
            {
                _dbContext.Entry(localEntity).State = EntityState.Detached;
            }
        }

        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_dbContext.Database.CurrentTransaction == null)
        {
            await _dbContext.Database.BeginTransactionAsync();
        }
    }

    public async Task CommitTransactionAsync()
    {
        if (_dbContext.Database.CurrentTransaction != null)
        {
            await _dbContext.Database.CurrentTransaction.CommitAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_dbContext.Database.CurrentTransaction != null)
        {
            await _dbContext.Database.CurrentTransaction.RollbackAsync();
        }
    }
}
