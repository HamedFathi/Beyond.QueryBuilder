// ReSharper disable UnusedMember.Global

using Beyond.QueryBuilder.Models;

namespace Beyond.QueryBuilder;

public static class QueryLinqExtensions
{
    public static IQueryable<TModel> WhereQuery<TModel>(this IQueryable<TModel> queryable, Query query)
    {
        return queryable.Where(query.ToLambdaExpression<TModel>());
    }

    public static IQueryable<TModel> WhereQuery<TModel>(this IEnumerable<TModel> enumerable, Query query)
    {
        return enumerable.AsQueryable().Where(query.ToLambdaExpression<TModel>());
    }

    public static IQueryable<TModel> WhereQuery<TModel>(this TModel[] array, Query query)
    {
        return array.AsQueryable().Where(query.ToLambdaExpression<TModel>());
    }
}