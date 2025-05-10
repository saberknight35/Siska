using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using Siska.Admin.Database;

namespace Siska.Admin.Database.Repositories
{
    public partial class Repository<TEntity> : RepositoryBase<TEntity>, IRepository<TEntity>
            where TEntity : class
    {

        public Repository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
        {
            InnerDbContext = dbContext;
            InnerDbSet = dbContext.Set<TEntity>();
        }

        protected DbContext InnerDbContext { get; }

        protected DbSet<TEntity> InnerDbSet { get; }


        #region sync

        public TEntity? GetById(params object[] id) => InnerDbSet.Find(id);

        public bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return InnerDbSet.Any(filter);
        }

        public int Count()
        {
            return InnerDbSet.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> filter)
        {
            return InnerDbSet.Count(filter);
        }

        public TEntity? SingleOrDefault(Action<IQueryIncludeSpesification<TEntity>> spec)
        {
            return ApplyQueryDataAttribute(InnerDbSet, spec).SingleOrDefault();
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> filter, Action<IQueryIncludeSpesification<TEntity>>? spec = null)
        {
            return ApplyQueryDataAttribute(InnerDbSet.Where(filter), spec).SingleOrDefault();
        }

        public TEntity? FirstOrDefault(Action<IQueryIncludeSpesification<TEntity>>? spec = null)
        {
            return ApplyQueryDataAttribute(InnerDbSet, spec).FirstOrDefault();
        }

        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> filter, Action<IQueryIncludeSpesification<TEntity>>? spec = null)
        {
            return ApplyQueryDataAttribute(InnerDbSet.Where(filter), spec).FirstOrDefault();
        }

        public IEnumerable<TEntity> List()
        {
            return InnerDbSet.ToList();
        }

        public IEnumerable<TEntity> List(Action<IQuerySpesification<TEntity>> spec)
        {
            return ApplyQueryDataAttribute(InnerDbSet, spec).ToList();
        }

        public IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> filter, Action<IQuerySpesification<TEntity>>? spec = null)
        {
            return ApplyQueryDataAttribute(InnerDbSet.Where(filter), spec).ToList();
        }
        #endregion


        #region async

        public async Task<TEntity?> GetByIdAsync(params object[] id)
        {
            return await InnerDbSet.FindAsync(id);
        }

        public async Task<TEntity?> GetByIdAsync(CancellationToken cancellationToken, params object[] id)
        {
            return await InnerDbSet.FindAsync(cancellationToken, id);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
        {
            return await InnerDbSet.AnyAsync(filter, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
        {
            return await InnerDbSet.CountAsync(filter, cancellationToken);
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await InnerDbSet.Where(filter).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Action<IQueryIncludeSpesification<TEntity>> spec, CancellationToken cancellationToken = default)
        {
            return await ApplyQueryDataAttribute(InnerDbSet, spec).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter, Action<IQueryIncludeSpesification<TEntity>> spec, CancellationToken cancellationToken = default)
        {
            return await ApplyQueryDataAttribute(InnerDbSet.Where(filter), spec).SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return await InnerDbSet.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await InnerDbSet.FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Action<IQueryIncludeSpesification<TEntity>> spec, CancellationToken cancellationToken = default)
        {
            return await ApplyQueryDataAttribute(InnerDbSet, spec).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, Action<IQueryIncludeSpesification<TEntity>> spec, CancellationToken cancellationToken = default)
        {
            return await ApplyQueryDataAttribute(InnerDbSet.Where(filter), spec).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await InnerDbSet.Where(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> ListAsync(Action<IQuerySpesification<TEntity>> spec, CancellationToken cancellationToken = default)
        {
            return await ApplyQueryDataAttribute(InnerDbSet, spec).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter, Action<IQuerySpesification<TEntity>> spec, CancellationToken cancellationToken = default)
        {
            return await ApplyQueryDataAttribute(InnerDbSet.Where(filter), spec).ToListAsync(cancellationToken);
        }

        #endregion


        #region 9nternal
        protected IQueryable<TEntity> ApplyQueryDataAttribute(IQueryable<TEntity> query, Action<IQueryIncludeSpesification<TEntity>>? spec)
        {
            if (spec == null) return query;
            var querySpec = new QueryIncludeSpesification();
            spec(querySpec);
            return querySpec.ApplyTo(query);
        }

        protected IQueryable<TEntity> ApplyQueryDataAttribute(IQueryable<TEntity> query, Action<IQuerySpesification<TEntity>>? spec)
        {
            if (spec == null) return query;
            var querySpec = new QuerySpesification();
            spec(querySpec);
            return querySpec.ApplyTo(query);
        }

        public int SaveChanges()
        {
            return InnerDbContext.SaveChanges();
        }

        public void Add(TEntity entity)
        {
            InnerDbSet.Add(entity);
        }

        public void AddRange(params TEntity[] entities)
        {
            InnerDbSet.AddRange(entities);
        }

        abstract class QuerySpesificationBase
        {
            protected List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> Spesifications = new();

            public IQueryable<TEntity> ApplyTo(IQueryable<TEntity> source)
            {
                foreach (var attr in Spesifications) source = attr(source);
                return source;
            }
        }

        class QueryIncludeSpesification : QuerySpesificationBase
            , IQueryIncludeSpesification<TEntity>
        {
            public IQueryIncludeSpesification<TEntity> AsNoTracking()
            {
                Spesifications.Add(q => q.AsNoTracking());
                return this;
            }

            public IQueryIncludeSpesification<TEntity> Include(string path)
            {
                Spesifications.Add(q => q.Include(path));
                return this;
            }

            public IQueryIncludeSpesification<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                Spesifications.Add(q => q.Include(property));
                return this;
            }
        }

        class QuerySpesification : QuerySpesificationBase
            , IQuerySpesification<TEntity>
            , IQueryOrderSpesification<TEntity>
        {
            public IQuerySpesification<TEntity> AsNoTracking()
            {
                Spesifications.Add(q => q.AsNoTracking());
                return this;
            }

            public IQuerySpesification<TEntity> Skip(int skip)
            {
                Spesifications.Add(q => q.Skip(skip));
                return this;
            }

            public IQuerySpesification<TEntity> Take(int count)
            {
                Spesifications.Add(q => q.Take(count));
                return this;
            }

            public IQuerySpesification<TEntity> Include(string path)
            {
                Spesifications.Add(q => q.Include(path));
                return this;
            }

            public IQuerySpesification<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                Spesifications.Add(q => q.Include(property));
                return this;
            }

            public IQueryOrderSpesification<TEntity> Order(string path, bool asc) => asc ? OrderBy(path) : OrderByDescending(path);

            public IQueryOrderSpesification<TEntity> OrderBy(string path)
            {
                Spesifications.Add(q => q.OrderBy(path));
                return this;
            }

            public IQueryOrderSpesification<TEntity> OrderByDescending(string path)
            {
                Spesifications.Add(q => q.OrderByDescending(path));
                return this;
            }

            public IQueryOrderSpesification<TEntity> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                Spesifications.Add(q => q.OrderBy(property));
                return this;
            }

            public IQueryOrderSpesification<TEntity> OrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                Spesifications.Add(q => q.OrderByDescending(property));
                return this;
            }

            IQueryOrderSpesification<TEntity> IQueryOrderSpesification<TEntity>.Then(string path, bool asc)
            {
                Spesifications.Add(q => q.Then(path, asc));
                return this;
            }

            IQueryOrderSpesification<TEntity> IQueryOrderSpesification<TEntity>.ThenBy(string path)
            {
                Spesifications.Add(q => ((IOrderedQueryable<TEntity>)q).ThenBy(path));
                return this;
            }

            IQueryOrderSpesification<TEntity> IQueryOrderSpesification<TEntity>.ThenBy<TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                Spesifications.Add(q => ((IOrderedQueryable<TEntity>)q).ThenBy(property));
                return this;
            }

            IQueryOrderSpesification<TEntity> IQueryOrderSpesification<TEntity>.ThenByDescending(string path)
            {
                Spesifications.Add(q => ((IOrderedQueryable<TEntity>)q).ThenByDescending(path));
                return this;
            }

            IQueryOrderSpesification<TEntity> IQueryOrderSpesification<TEntity>.ThenByDescending<TProperty>(Expression<Func<TEntity, TProperty>> property)
            {
                Spesifications.Add(q => ((IOrderedQueryable<TEntity>)q).ThenByDescending(property));
                return this;
            }
        }
        #endregion


    }
}
