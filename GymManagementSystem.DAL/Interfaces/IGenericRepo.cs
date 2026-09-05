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


        //include=>Query
        //includes=>List of include <object>

        //GetAllAsync with its overloads 


        // 1 - تجيب كل ال entities من ال database 
        // من غير ال navigation properties
        Task<IReadOnlyList<TEntity>> GetAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default);

        //2- بترجع كل ال entities من ال database مع ال navigation properties
        // بتاخد array of expressions of navigation properties -- مش بتفرق معاه النوع 
        Task<IReadOnlyList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, object>>[] includes,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);

        // 3- بتاخد function of queryable of entities و بترجع queryable of entities
        
        Task<IReadOnlyList<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);

        //-------------------------------------------------------------------

        //GetByIdAsync with its overloads

        // 1- بتجيب entity by id من غير ال navigation properties
        Task<TEntity?> GetByIdAsync(int id, bool trackChanges = false, CancellationToken cancellationToken = default);


        //2- بتجيب بال navigation properties
        Task<TEntity?> GetByIdAsync(
            int id,
            Expression<Func<TEntity, object>>[] includes,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);

        // 3- بتاخد function of queryable of entities و بترجع queryable of entities
        Task<TEntity?> GetByIdAsync(
            int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);


        //---------------------------------------------------------------------------------

        
        Task<TEntity?> GetByIdIncludeDeletedAsync(int id, bool trackChanges = false, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdIncludeDeletedAsync(
            int id,
            Expression<Func<TEntity, object>>[] includes,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdIncludeDeletedAsync(
            int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);

        //---------------------------------------------------------------------------------


        // زي ال GetByIdAsync 
        // بس بتجيب ال entity بشرط 
        Task<TEntity> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>>[] includes,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> include,
            bool trackChanges = false,
            CancellationToken cancellationToken = default);

        //---------------------------------------------------------------------------------

        void Update(TEntity entity);
        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<TEntity>AddAsync(TEntity entity,CancellationToken cancellationToken=default);
        Task <TEntity>SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);

    }
    












}
