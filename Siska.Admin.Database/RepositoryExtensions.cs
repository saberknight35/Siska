using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Siska.Admin.Database.Repositories;
using Siska.Admin.Model.DTO;
using Siska.Admin.Utility;

namespace Siska.Admin.Database
{
    public static class RepositoryExtensions
    {
        public static async Task<PagedList<TEntity>> GetPagedList<TEntity>(this IReadRepositoryBase<TEntity> repository
            , int pageNumber, int pageSize, CancellationToken cancellationToken) where TEntity : class
        {
            if (pageNumber <= 0) pageNumber = 1;

            if (pageSize <= 0) pageSize = 10;
            var skipSize = (pageNumber - 1) * pageSize;

            var totalItems = await repository.CountAsync(cancellationToken: cancellationToken);
            var data = await repository.ListAsync(new PagedSpesification<TEntity>(skipSize, pageSize), cancellationToken);
            return new PagedList<TEntity>(data, totalItems, pageNumber, pageSize);
        }

        public static async Task<PagedList<TEntity>> GetPagedList<TEntity>(this IRepository<TEntity> repository
            , int pageNumber, int pageSize, string field, object value, CancellationToken cancellationToken) where TEntity : class
        {
            if (pageNumber <= 0) pageNumber = 1;

            if (pageSize <= 0) pageSize = 10;
            var skipSize = (pageNumber - 1) * pageSize;

            var totalItems = await repository.CountAsync(ExpressionUtils.BuildPredicate<TEntity>(field, "==", value), cancellationToken: cancellationToken);
            var data = await repository.ListAsync(new PagedSpesification<TEntity>(field, value, skipSize, pageSize), cancellationToken);
            return new PagedList<TEntity>(data, totalItems, pageNumber, pageSize);
        }

        public static async Task<PagedList<TEntity>> GetPagedList<TEntity>(this IRepository<TEntity> repository
            , int pageNumber, int pageSize, List<ListDataDTO.SearchTerm> searchList, CancellationToken cancellationToken) where TEntity : class
        {
            if (pageNumber <= 0) pageNumber = 1;

            if (pageSize <= 0) pageSize = 10;
            var skipSize = (pageNumber - 1) * pageSize;

            var expCondition = ExpressionUtils.BuildCondition<TEntity>(searchList);

            var totalItems = await repository.CountAsync(expCondition, cancellationToken: cancellationToken);
            var data = await repository.ListAsync(new PagedSpesification<TEntity>(expCondition, skipSize, pageSize), cancellationToken);
            return new PagedList<TEntity>(data, totalItems, pageNumber, pageSize);
        }

        public static async Task<PagedList<TEntity>> GetPagedList<TEntity>(this IRepository<TEntity> repository
            , ListDataDTO listDataDTO, CancellationToken cancellationToken) where TEntity : class
        {
            if (listDataDTO.PageNumber <= 0) listDataDTO.PageNumber = 1;

            if (listDataDTO.PageSize <= 0) listDataDTO.PageSize = 10;
            var skipSize = (listDataDTO.PageNumber - 1) * listDataDTO.PageSize;

            var totalItems = 0;
            Specification<TEntity> specification = null;

            if (listDataDTO.Search != null && listDataDTO.Search.Count > 0)
            {
                var expCondition = ExpressionUtils.BuildCondition<TEntity>(listDataDTO.Search);

                totalItems = await repository.CountAsync(expCondition, cancellationToken: cancellationToken);

                specification = new PagedSpesification<TEntity>(expCondition, skipSize, listDataDTO.PageSize);
            }
            else if (!listDataDTO.SearchField.IsNullOrEmpty() && !listDataDTO.SearchString.IsNullOrEmpty())
            {
                var expCondition = ExpressionUtils.BuildPredicate<TEntity>(listDataDTO.SearchField, "==", listDataDTO.SearchString);

                totalItems = await repository.CountAsync(expCondition, cancellationToken: cancellationToken);

                specification = new PagedSpesification<TEntity>(expCondition, skipSize, listDataDTO.PageSize);
            }
            else
            {
                totalItems = await repository.CountAsync(cancellationToken: cancellationToken);

                specification = new PagedSpesification<TEntity>(skipSize, listDataDTO.PageSize);
            }

            var data = await repository.ListAsync(specification, cancellationToken);


            return new PagedList<TEntity>(data, totalItems, listDataDTO.PageNumber, listDataDTO.PageSize);
        }

        public static async Task<PagedList<TEntity>> GetPagedList<TEntity>(this IRepository<TEntity> repository
            , ListDataDTO listDataDTO, List<string> include, CancellationToken cancellationToken) where TEntity : class
        {
            if (listDataDTO.PageNumber <= 0) listDataDTO.PageNumber = 1;

            if (listDataDTO.PageSize <= 0) listDataDTO.PageSize = 10;
            var skipSize = (listDataDTO.PageNumber - 1) * listDataDTO.PageSize;

            var totalItems = 0;
            Specification<TEntity> specification = null;

            if (listDataDTO.Search != null && listDataDTO.Search.Count > 0)
            {
                var expCondition = ExpressionUtils.BuildCondition<TEntity>(listDataDTO.Search);

                totalItems = await repository.CountAsync(expCondition, cancellationToken: cancellationToken);

                specification = new PagedSpesification<TEntity>(expCondition, skipSize, listDataDTO.PageSize);
            }
            else if (!listDataDTO.SearchField.IsNullOrEmpty() && !listDataDTO.SearchString.IsNullOrEmpty())
            {
                var expCondition = ExpressionUtils.BuildPredicate<TEntity>(listDataDTO.SearchField, "==", listDataDTO.SearchString);

                totalItems = await repository.CountAsync(expCondition, cancellationToken: cancellationToken);

                specification = new PagedSpesification<TEntity>(expCondition, skipSize, listDataDTO.PageSize);
            }
            else
            {
                totalItems = await repository.CountAsync(cancellationToken: cancellationToken);

                specification = new PagedSpesification<TEntity>(skipSize, listDataDTO.PageSize);
            }

            if (include != null && include.Count > 0)
            {
                foreach (var item in include)
                {
                    specification = specification.Query.Include(item).Specification;
                }
            }

            var data = await repository.ListAsync(specification, cancellationToken);


            return new PagedList<TEntity>(data, totalItems, listDataDTO.PageNumber, listDataDTO.PageSize);
        }

        class PagedSpesification<TEntity> : Specification<TEntity>
        {
            public PagedSpesification(int skip, int take)
            {
                Query.Skip(skip).Take(take);
            }

            public PagedSpesification(string field, object value, int skip, int take)
            {
                Query.Where(ExpressionUtils.BuildPredicate<TEntity>(field, "==", value)).Skip(skip).Take(take);
            }

            public PagedSpesification(Expression<Func<TEntity, bool>> exp, int skip, int take)
            {
                Query.Where(exp).Skip(skip).Take(take);
            }
        }
    }
}
