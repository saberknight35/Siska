using System.Linq.Expressions;
using Siska.Admin.Database;

namespace Siska.Admin.Database
{
    public static class LinqExtensions
    {
        public static bool Any<T>(this IEnumerable<T> source, int count)
        {
            int idx = 0;
            return source.Any(e => ++idx == count);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string path) => source.Order(path);

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string path) => source.Order(path, false);

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string path) => source.Then(path);

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string path) => source.Then(path, false);

        public static IOrderedQueryable<T> Then<T>(this IQueryable<T> source, string path, bool asc = true) => BuildOrder(source, path, asc ? "ThenBy" : "ThenByDescending");

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string path) => source.Order(path);

        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, string path) => source.Order(path, false);

        public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> source, string path, bool asc = true) => BuildOrder(source, path, asc ? "OrderBy" : "OrderByDescending");

        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, string path) => source.Then(path);

        public static IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> source, string path) => source.Then(path, false);

        public static IOrderedEnumerable<T> Then<T>(this IOrderedEnumerable<T> source, string path, bool asc = true) => BuildOrder(source, path, asc ? "ThenBy" : "ThenByDescending");

        public static IOrderedQueryable<T> Order<T>(this IQueryable<T> source, string path, bool asc = true) => BuildOrder(source, path, asc ? "OrderBy" : "OrderByDescending");

        private static IOrderedQueryable<T> BuildOrder<T>(IQueryable<T> source, string path, string methodName)
        {
            if (source == null)
                throw new ArgumentNullException("source IQueryable");

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path is required");

            var props = path.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "e");
            Expression expr = arg;

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof(Queryable).GetMethods().Single(
                            method => method.Name == methodName
                                            && method.IsGenericMethodDefinition
                                            && method.GetGenericArguments().Length == 2
                                            && method.GetParameters().Length == 2)
                            .MakeGenericMethod(typeof(T), type)
                            .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        private static IOrderedEnumerable<T> BuildOrder<T>(IEnumerable<T> source, string path, string methodName)
        {
            if (source == null)
                throw new ArgumentNullException("source IEnumerable");

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path is required");

            var props = path.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "e");
            Expression expr = arg;

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof(Enumerable).GetMethods().Single(
                            method => method.Name == methodName
                                            && method.IsGenericMethodDefinition
                                            && method.GetGenericArguments().Length == 2
                                            && method.GetParameters().Length == 2)
                            .MakeGenericMethod(typeof(T), type)
                            .Invoke(null, new object[] { source, lambda.Compile() });

            return (IOrderedEnumerable<T>)result;
        }

        public static Expression<Func<TInput, bool>> AndAlso<TInput>(this Expression<Func<TInput, bool>> source
                , Expression<Func<TInput, bool>> other) => Expression.Lambda<Func<TInput, bool>>(
                        Expression.AndAlso(source.Body, new ExpressionParameterReplacer(other.Parameters, source.Parameters).Visit(other.Body))
                    , source.Parameters);

        public static Expression<Func<TInput, bool>> AndAlso<TInput>(this Expression<Func<TInput, bool>> source
                , bool condition
                , Func<Expression<Func<TInput, bool>>> other) => condition ? source.AndAlso(other()) : source;

        public static Expression<Func<TInput, bool>> AndAlsoIfPosible<TInput>(this Expression<Func<TInput, bool>> source
                , Expression<Func<TInput, bool>> other)
        {
            if (source == null && other != null) return other;
            else if (source != null && other == null) return source;
            return source.AndAlso(other);
        }

        public static Expression<Func<TInput, bool>> OrElse<TInput>(this Expression<Func<TInput, bool>> source
                , Expression<Func<TInput, bool>> other) => Expression.Lambda<Func<TInput, bool>>(
                        Expression.OrElse(source.Body, new ExpressionParameterReplacer(other.Parameters, source.Parameters).Visit(other.Body))
                    , source.Parameters);

        public static Expression<Func<TInput, bool>> OrElse<TInput>(this Expression<Func<TInput, bool>> source
                , bool condition
                , Func<Expression<Func<TInput, bool>>> other) => condition ? source.OrElse(other()) : source;

        public static Expression<Func<TInput, bool>> OrElseIfPosible<TInput>(this Expression<Func<TInput, bool>>? source
             , Expression<Func<TInput, bool>>? other)
        {
            if (source == null && other != null) return other;
            else if (source != null && other == null) return source;
            return source.OrElse(other);
        }

        class ExpressionParameterReplacer : ExpressionVisitor
        {
            private IDictionary<ParameterExpression, ParameterExpression> parameterReplacements;

            public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters
                , IList<ParameterExpression> toParameters)
            {
                parameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
                for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                {
                    parameterReplacements.Add(fromParameters[i], toParameters[i]);
                }
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (parameterReplacements.TryGetValue(node, out ParameterExpression replacement))
                {
                    node = replacement;
                }
                return base.VisitParameter(node);
            }
        }
    }
}
