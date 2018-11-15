using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace Jinher.AMP.BTP.Common.Extensions
{
    public static class ExpressionExtension
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null) return second;
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null) return second;
            return first.Compose(second, Expression.Or);
        }

        public static IQueryable<TSource> WhereDate<TSource>(this IQueryable<TSource> source, DateTime? start, DateTime? end, string timeField = "SubTime")
        {
            Expression express = null;
            ParameterExpression param = Expression.Parameter(typeof(TSource));
            var timeProperty = Expression.Property(param, timeField);
            Type timeType;
            if (IsNullableType(timeProperty.Type)) timeType = typeof(DateTime?);
            else timeType = typeof(DateTime);
            if (start.HasValue)
            {
                if (end.HasValue)
                {
                    express = Expression.AndAlso(
                             Expression.GreaterThan(timeProperty, Expression.Constant(start.Value.Date, timeType)),
                             Expression.LessThan(timeProperty, Expression.Constant(end.Value.Date.AddDays(1), timeType))
                         );
                }
                else
                {
                    express = Expression.GreaterThan(timeProperty, Expression.Constant(start.Value.Date, timeType));
                }
            }
            else
            {
                if (end.HasValue)
                {
                    express = Expression.LessThan(timeProperty, Expression.Constant(end.Value.Date, timeType));
                }
            }
            return express == null ? source : source.Where(Expression.Lambda<Func<TSource, bool>>(express, param));
        }

        static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }

}
