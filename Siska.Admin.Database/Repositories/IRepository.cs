using Ardalis.Specification;
using System.Linq.Expressions;

namespace Siska.Admin.Database.Repositories
{
    public interface IRepository<TEntity> : IRepositoryBase<TEntity>
            where TEntity : class
    {
        #region sync
        int Count();

        int Count(Expression<Func<TEntity, bool>> filter);

        bool Any(Expression<Func<TEntity, bool>> filter);

        TEntity? GetById(params object[] id);

        TEntity? SingleOrDefault(Action<IQueryIncludeSpesification<TEntity>> spec);

        TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> filter, Action<IQueryIncludeSpesification<TEntity>>? spec = null);

        TEntity? FirstOrDefault(Action<IQueryIncludeSpesification<TEntity>>? spec = null);

        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> filter, Action<IQueryIncludeSpesification<TEntity>>? spec = null);

        IEnumerable<TEntity> List();

        IEnumerable<TEntity> List(Action<IQuerySpesification<TEntity>> spec);

        IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> filter, Action<IQuerySpesification<TEntity>>? spec = null);
        #endregion


        #region async
        Task<TEntity?> GetByIdAsync(params object[] id);

        Task<TEntity?> GetByIdAsync(CancellationToken cancellationToken, params object[] id);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        Task<TEntity?> SingleOrDefaultAsync(Action<IQueryIncludeSpesification<TEntity>> spec, CancellationToken cancellationToken = default);

        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter, Action<IQueryIncludeSpesification<TEntity>> spec, CancellationToken cancellationToken = default);

        Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        Task<TEntity?> FirstOrDefaultAsync(Action<IQueryIncludeSpesification<TEntity>> spec, CancellationToken cancellationToken = default);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, Action<IQueryIncludeSpesification<TEntity>> spec, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> ListAsync(Action<IQuerySpesification<TEntity>> spec, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter, Action<IQuerySpesification<TEntity>> spec, CancellationToken cancellationToken = default);
        #endregion

        int SaveChanges();

        void Add(TEntity entity);

        void AddRange(params TEntity[] entities);
    }
}
