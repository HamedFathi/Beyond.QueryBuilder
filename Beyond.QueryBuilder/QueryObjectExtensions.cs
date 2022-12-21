// ReSharper disable MergeIntoPattern
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using Beyond.QueryBuilder.Helpers;
using Beyond.QueryBuilder.Models;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beyond.QueryBuilder;

public static class QueryObjectExtensions
{
    public static bool HasRules(this Query query)
    {
        return query.Connector != null && query.Rules is { Count: > 0 };
    }

    public static bool IsEmpty(this Query query)
    {
        return query.Connector == null;
    }

    public static Func<T, bool> ToFunc<T>(this Query query)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        return query.ToLambdaExpression<T>().Compile();
    }

    public static string ToJsonQueryDefinition(this Query query, bool indented = false)
    {
        return JsonSerializer.Serialize(query, new JsonSerializerOptions
        {
            WriteIndented = indented,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }

    public static Expression<Func<T, bool>> ToLambdaExpression<T>(this Query query)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var qr = (QueryRule)query;
        var result = new[] { qr }.ToLambdaExpressionBuilder<T>();
        return result;
    }

    public static Query? ToQueryObject(this string queryObject)
    {
        if (string.IsNullOrWhiteSpace(queryObject))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(queryObject));

        return JsonSerializer.Deserialize<Query>(queryObject);
    }

    public static string ToReadableDefinition(this Query query)
    {
        var qr = (QueryRule)query;
        return new[] { qr }.ToReadableStringDefinition().RemoveFirstAndLast();
    }
}