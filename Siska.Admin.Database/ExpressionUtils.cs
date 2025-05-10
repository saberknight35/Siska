using System.Linq.Expressions;
using Siska.Admin.Model.DTO;
using Siska.Admin.Utility;

namespace Siska.Admin.Database
{
    public static class ExpressionUtils
    {
        public static Expression<Func<T, bool>> BuildPredicate<T>(string propertyName, string comparison, object value)
        {
            var parameter = Expression.Parameter(typeof(T));
            var left = propertyName.Split('.').Aggregate((Expression)parameter, Expression.PropertyOrField);
            var body = MakeComparison(left, comparison, value);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        static Expression MakeComparison(Expression left, string comparison, object value)
        {
            var constant = left;

            if (left.Type == typeof(string))
            {
                constant = Expression.Constant(value.ToString(), left.Type);
            }
            else if (left.Type == typeof(Guid))
            {
                constant = Expression.Constant(Guid.Parse(value.ToString()), left.Type);
            }
            else if (left.Type == typeof(bool))
            {
                constant = Expression.Constant(bool.Parse(value.ToString()), left.Type);
            }
            else if (left.Type == typeof(DateTime))
            {
                constant = Expression.Constant(DateTime.Parse(value.ToString()), left.Type);
            }
            else if (left.Type == typeof(decimal))
            {
                constant = Expression.Constant(decimal.Parse(value.ToString()), left.Type);
            }
            else if (left.Type == typeof(short))
            {
                constant = Expression.Constant(short.Parse(value.ToString()), left.Type);
            }
            else if (left.Type.BaseType == typeof(Enum))
            {
                constant = Expression.Constant(Enum.Parse(left.Type, value.ToString()));
            }
            else
            {
                constant = Expression.Constant(int.Parse(value.ToString()), left.Type);
            }

            switch (comparison)
            {
                case "==":
                    return Expression.MakeBinary(ExpressionType.Equal, left, constant);
                case "!=":
                    return Expression.MakeBinary(ExpressionType.NotEqual, left, constant);
                case ">":
                    return Expression.MakeBinary(ExpressionType.GreaterThan, left, constant);
                case ">=":
                    return Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, left, constant);
                case "<":
                    return Expression.MakeBinary(ExpressionType.LessThan, left, constant);
                case "<=":
                    return Expression.MakeBinary(ExpressionType.LessThanOrEqual, left, constant);
                case "Contains":
                    if (value is string)
                    {
                        return Expression.Call(left, comparison, Type.EmptyTypes, constant);
                    }
                    throw new NotSupportedException($"Comparison operator '{comparison}' only supported on string.");
                case "StartsWith":
                case "EndsWith":
                    if (value is string)
                    {
                        return Expression.Call(left, comparison, Type.EmptyTypes, constant);
                    }
                    throw new NotSupportedException($"Comparison operator '{comparison}' only supported on string.");
                default:
                    throw new NotSupportedException($"Invalid comparison operator '{comparison}'.");
            }
        }

        public static Expression<Func<T, bool>> BuildCondition<T>(List<ListDataDTO.SearchTerm> conditions)
        {
            if (conditions == null || !conditions.Any())
            {
                throw new ArgumentNullException("No Condition to check");
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression combined = null;

            foreach (var condition in conditions)
            {
                ConstantExpression constant;

                var left = condition.Field.Split('.').Aggregate((Expression)parameter, Expression.PropertyOrField);

                if (left.Type == typeof(string))
                {
                    constant = Expression.Constant(condition.Value.ToString());
                }
                else if (left.Type == typeof(Guid))
                {
                    constant = Expression.Constant(Guid.Parse(condition.Value.ToString()));
                }
                else if (left.Type == typeof(bool))
                {
                    constant = Expression.Constant(bool.Parse(condition.Value.ToString()));
                }
                else if (left.Type == typeof(DateTime))
                {
                    constant = Expression.Constant(DateTime.Parse(condition.Value.ToString()));
                }
                else if (left.Type == typeof(decimal))
                {
                    constant = Expression.Constant(decimal.Parse(condition.Value.ToString()));
                }
                else if (left.Type == typeof(short))
                {
                    constant = Expression.Constant(short.Parse(condition.Value.ToString()));
                }
                else if (left.Type.BaseType == typeof(Enum))
                {
                    constant = Expression.Constant(Enum.Parse(left.Type, condition.Value.ToString()));
                }
                else
                {
                    constant = Expression.Constant(int.Parse(condition.Value.ToString()));
                }

                MemberExpression property = null;
                if (condition.Field.Split(".").Length > 1)
                {
                    var fields = condition.Field.Split(".");
                    property = Expression.Property(parameter, fields[0]);
                    for (var iStart = 1; iStart < fields.Length; iStart++)
                    {
                        property = Expression.Property(property, fields[iStart]);
                    }
                }
                else
                {
                    property = Expression.Property(parameter, condition.Field);
                }

                Expression comparison = condition.Opr switch
                {
                    "==" => Expression.Equal(property, constant),
                    "!=" => Expression.NotEqual(property, constant),
                    ">" => Expression.GreaterThan(property, constant),
                    "<" => Expression.LessThan(property, constant),
                    ">=" => Expression.GreaterThanOrEqual(property, constant),
                    "<=" => Expression.LessThanOrEqual(property, constant),
                    "Contains" => Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant),
                    "StartsWith" => Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant),
                    "EndsWith" => Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant),
                    _ => throw new NotSupportedException($"Operator {condition.Opr} is not supported.")
                };

                combined = combined == null ? comparison : condition.Added.IsNullOrEmpty() ? comparison : condition.Added.Equals("or") ? Expression.OrElse(combined, comparison) : Expression.AndAlso(combined, comparison);
            }

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }
}
