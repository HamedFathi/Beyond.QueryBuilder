// ReSharper disable MergeIntoPattern
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

using Beyond.QueryBuilder.Helpers;
using Beyond.QueryBuilder.Models;
using System.Collections;
using System.Linq.Expressions;

namespace Beyond.QueryBuilder;

public static class QueryRuleExtensions
{
    public static List<QueryRule> FlattenQueryRule(this QueryRule queryRule)
    {
        var members = new List<QueryRule> { queryRule };
        if (queryRule.Rules != null) members.AddRange(queryRule.Rules.FlattenQueryRules());
        return members;
    }

    public static IEnumerable<QueryRule> FlattenQueryRules(this IEnumerable<QueryRule> list)
    {
        /*
        public static IEnumerable<QueryRule> FlattenQueryRules(this IEnumerable<QueryRule> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));

            foreach (var item in list)
            {
                yield return item;
                if (item.Rules != null)
                    foreach (var children in FlattenQueryRules(item.Rules))
                        yield return children;
            }
        }
        */

        if (list == null) throw new ArgumentNullException(nameof(list));

        var stack = new Stack<QueryRule>(list.Reverse());
        while (stack.Count > 0)
        {
            var item = stack.Pop();
            yield return item;

            if (item.Rules == null) continue;

            foreach (var child in item.Rules)
            {
                stack.Push(child);
            }
        }
    }

    public static bool HasRules(this QueryRule queryRule)
    {
        return queryRule.Connector != null && queryRule.Rules is { Count: > 0 };
    }

    public static bool IsSimpleExpression(this QueryRule queryRule)
    {
        return queryRule.Connector == null && !string.IsNullOrEmpty(queryRule.Property);
    }

    internal static Expression<Func<T, bool>> ToLambdaExpressionBuilder<T>(this IEnumerable<QueryRule> queryRules, string? connector = null)
    {
        var list = new List<Expression<Func<T, bool>>>();

        foreach (var queryRule in queryRules)
        {
            if (queryRule.HasRules())
            {
                if (queryRule.Rules != null)
                    list.Add(ToLambdaExpressionBuilder<T>(queryRule.Rules, queryRule.Connector));
            }
            else
            {
                switch (connector?.ToLower())
                {
                    case "and":
                        list.Add(AndQueryRules<T>(new[] { queryRule }));
                        break;

                    case "or":
                        list.Add(OrQueryRules<T>(new[] { queryRule }));
                        break;
                }
            }
        }

        var result = list.Aggregate((a, b) =>
            connector?.ToLower() == "and"
                ? a.AndAlso(b)
                : a.OrElse(b));
        return result;
    }

    internal static string ToReadableStringDefinition(this IEnumerable<QueryRule> queryRules, string? connector = null)
    {
        var list = new List<string>();
        foreach (var queryRule in queryRules)
        {
            if (queryRule.Rules != null)
                list.Add(queryRule.HasRules()
                    ? ToReadableStringDefinition(queryRule.Rules, queryRule.Connector)
                    : queryRule.ToStringDefinition());
        }
        return list.Count > 0 ? "(" + list.Aggregate((a, b) => $"{a} {connector?.ToUpper()} {b}") + ")" : string.Empty;
    }

    internal static string ToStringDefinition(this QueryRule queryRule)
    {
        var val = string.Empty;
        var obj = queryRule.Value;
        if (obj != null)
        {
            var status = Utilities.GetOperatorInfo(queryRule.Operator).AcceptsArrayAsSecondOperand;
            if (status)
            {
                var objects = GetObjects(obj, out var type);
                switch (type)
                {
                    case "string":
                        {
                            var strings = objects.Select(x => $"\"{x}\"");
                            val = "[" + strings.Aggregate((a, b) => a + ", " + b) + "]";
                            break;
                        }
                    case "char":
                        {
                            var strings = objects.Select(x => $"'{x}'");
                            val = "[" + strings.Aggregate((a, b) => a + ", " + b) + "]";
                            break;
                        }
                    default:
                        val = "[" + objects.Aggregate((a, b) => a + ", " + b) + "]";
                        break;
                }
            }
            else
            {
                if (obj is IEnumerable enumerable and not string)
                {
                    var i = enumerable.Count();
                    if (i > 1)
                        throw new Exception($"The '{queryRule.Operator}' operator does not accept array for second operand!");
                }
                val = obj switch
                {
                    string => $"\"{obj}\"",
                    char => $"'{obj}'",
                    _ => obj.ToString()
                };
            }
        }
        return "(" + $"{queryRule.Property} {queryRule.Operator} {val}".Trim() + ")";
    }

    private static Expression<Func<TModel, bool>> AndQueryRules<TModel>(this IEnumerable<QueryRule> queryRules)
    {
        Expression<Func<TModel, bool>> result = _ => true;
        foreach (var item in queryRules)
        {
            var parameterExpression = Expression.Parameter(typeof(TModel));
            if (item.Property != null)
            {
                var memberExpression = Expression.Property(parameterExpression, item.Property);

                var constantExpression = Expression.Constant(item.Value);
                if (item.Operator != null)
                {
                    var comparison = item.Operator.GetBinaryExpression(memberExpression, constantExpression, item);
                    var expression = Expression.Lambda<Func<TModel, bool>>(comparison, parameterExpression);
                    var param = Expression.Parameter(typeof(TModel), "x");
                    var body = Expression.AndAlso(
                        Expression.Invoke(result, param),
                        Expression.Invoke(expression, param)
                    );
                    result = Expression.Lambda<Func<TModel, bool>>(body, param);
                }
            }
        }
        return result;
    }

    private static IEnumerable<object> GetObjects(object obj, out string type)
    {
        var result = ((IEnumerable)obj).Cast<object>().ToList();
        if (result[0] is char)
        {
            type = "char";
        }
        else if (result[0] is string)
        {
            type = "string";
        }
        else
        {
            type = "object";
        }
        return result;
    }

    private static Expression<Func<TModel, bool>> OrQueryRules<TModel>(this IEnumerable<QueryRule> queryRules)
    {
        Expression<Func<TModel, bool>> result = _ => false;
        foreach (var item in queryRules)
        {
            var parameterExpression = Expression.Parameter(typeof(TModel));
            if (item.Property != null)
            {
                var memberExpression = Expression.Property(parameterExpression, item.Property);
                var constantExpression = Expression.Constant(item.Value);
                if (item.Operator != null)
                {
                    var comparison = item.Operator.GetBinaryExpression(memberExpression, constantExpression, item);
                    var expression = Expression.Lambda<Func<TModel, bool>>(comparison, parameterExpression);
                    var param = Expression.Parameter(typeof(TModel), "x");
                    var body = Expression.OrElse(
                        Expression.Invoke(result, param),
                        Expression.Invoke(expression, param)
                    );
                    result = Expression.Lambda<Func<TModel, bool>>(body, param);
                }
            }
        }
        return result;
    }
}