using System.Linq.Expressions;

namespace Siska.Admin.Database.Repositories
{
    public interface IQueryOrderSpesification<TEntity>
    {
        IQueryOrderSpesification<TEntity> Then(string path, bool asc);

        IQueryOrderSpesification<TEntity> ThenBy(string path);

        IQueryOrderSpesification<TEntity> ThenBy<TProperty>(Expression<Func<TEntity, TProperty>> property);

        IQueryOrderSpesification<TEntity> ThenByDescending(string path);

        IQueryOrderSpesification<TEntity> ThenByDescending<TProperty>(Expression<Func<TEntity, TProperty>> property);

        IQueryable<TEntity> ApplyTo(IQueryable<TEntity> source);
    }
}
