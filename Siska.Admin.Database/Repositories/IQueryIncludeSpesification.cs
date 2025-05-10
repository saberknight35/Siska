using System.Linq.Expressions;

namespace Siska.Admin.Database.Repositories
{
    public interface IQueryIncludeSpesification<TEntity>
    {
        IQueryIncludeSpesification<TEntity> AsNoTracking();

        IQueryIncludeSpesification<TEntity> Include(string path);

        IQueryIncludeSpesification<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> property);

        IQueryable<TEntity> ApplyTo(IQueryable<TEntity> source);
    }
}
