using GymManagementSystem.DAL.Models;
using GymManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Interfaces
{
    public  interface IGenericRepo<TEntity> where TEntity : BaseEntity
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, object>>[] includes,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            CancellationToken cancellationToken = default);

        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(
            int id,
            Expression<Func<TEntity, object>>[] includes,
            CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(
            int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            CancellationToken cancellationToken = default);

        Task<TEntity?> GetByIdIncludeDeletedAsync(int id, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdIncludeDeletedAsync(
            int id,
            Expression<Func<TEntity, object>>[] includes,
            CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdIncludeDeletedAsync(
            int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>>[] includes,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            CancellationToken cancellationToken = default);

        void Update(TEntity entity);
        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity>AddAsync(TEntity entity,CancellationToken cancellationToken=default);
        Task <TEntity>SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);

    }
    












}
