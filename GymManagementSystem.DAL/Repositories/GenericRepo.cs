using GymManagementSystem.DAL.Interfaces;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.Dbcontexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace GymManagementSystem.DAL.Implementation
{
    public class GenericRepo<TEntity>(GymContext dbcontext) : IGenericRepo<TEntity> where TEntity : BaseEntity
    {
        private readonly GymContext _dbcontext=dbcontext;//primary constructor injection
        private readonly DbSet<TEntity> _dbSet=dbcontext.Set<TEntity>(); // بدل ما كل مره اعمله set

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)

        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }


        public async Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>>[] includes,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            query = ApplyIncludes(query, includes, null);
            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            query = ApplyIncludes(query, null, include);
            return await query.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, object>>[] includes,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();
            query = ApplyIncludes(query, includes, null);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();
            query = ApplyIncludes(query, null, include);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(t => t.ID == id, cancellationToken);

        public async Task<TEntity?> GetByIdAsync(
            int id,
            Expression<Func<TEntity, object>>[] includes,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            query = ApplyIncludes(query, includes, null);
            return await query.FirstOrDefaultAsync(t => t.ID == id, cancellationToken);
        }

        public async Task<TEntity?> GetByIdAsync(
            int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet;
            query = ApplyIncludes(query, null, include);
            return await query.FirstOrDefaultAsync(t => t.ID == id, cancellationToken);
        }

        public async Task<TEntity?> GetByIdIncludeDeletedAsync(int id, CancellationToken cancellationToken = default)
            => await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.ID == id, cancellationToken);

        public async Task<TEntity?> GetByIdIncludeDeletedAsync(
            int id,
            Expression<Func<TEntity, object>>[] includes,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.IgnoreQueryFilters();
            query = ApplyIncludes(query, includes, null);
            return await query.FirstOrDefaultAsync(t => t.ID == id, cancellationToken);
        }

        public async Task<TEntity?> GetByIdIncludeDeletedAsync(
            int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.IgnoreQueryFilters();
            query = ApplyIncludes(query, null, include);
            return await query.FirstOrDefaultAsync(t => t.ID == id, cancellationToken);
        }

        private static IQueryable<TEntity> ApplyIncludes(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, object>>[]? includes = null,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            if (includes != null && includes.Length > 0)
            {
                foreach (var inc in includes)
                {
                    query = query.Include(inc);
                }
            }

            if (include != null)
            {
                query = include(query);
            }

            return query;
        }


        public Task<TEntity> SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
            return Task.FromResult(entity);
        }

        public void Update(TEntity entity) => _dbSet.Update(entity);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)=>_dbcontext.SaveChangesAsync(cancellationToken);

        public Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(predicate, cancellationToken);
    }
}
