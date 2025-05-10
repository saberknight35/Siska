using System.Linq.Expressions;

namespace Siska.Admin.Database.Repositories
{
    public interface IQuerySpesification<TEntity>
    {
        IQuerySpesification<TEntity> Skip(int skip);
        IQuerySpesification<TEntity> Take(int count);
        IQuerySpesification<TEntity> Include(string path);
        IQuerySpesification<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> property);
        IQuerySpesification<TEntity> AsNoTracking();
        IQueryOrderSpesification<TEntity> Order(string path, bool asc);
        IQueryOrderSpesification<TEntity> OrderBy(string path);
        IQueryOrderSpesification<TEntity> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> property);
        IQueryOrderSpesification<TEntity> OrderByDescending(string path);
        IQueryOrderSpesification<TEntity> OrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> property);
        IQueryable<TEntity> ApplyTo(IQueryable<TEntity> source);
    }
}
