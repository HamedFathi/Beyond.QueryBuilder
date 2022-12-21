// ReSharper disable UnusedMember.Global

using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Beyond.QueryBuilder.Helpers;

internal static class Extensions
{
    internal static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var param = expr1.Parameters[0];
        if (ReferenceEquals(param, expr2.Parameters[0]))
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(expr1.Body, expr2.Body), param);
        }
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                expr1.Body,
                Expression.Invoke(expr2, param)), param);
    }

    internal static bool Contains(this string? source, string? target, StringComparison comparison)
    {
        return target != null && source?.IndexOf(target, comparison) >= 0;
    }

    internal static int Count(this IEnumerable enumerable, bool excludeNullValues = false)
    {
        var list = enumerable.Cast<object?>();
        if (excludeNullValues) list = list.Where(x => x != null);
        return Enumerable.Count(list);
    }

    internal static Type?[] GenericTypeArguments(this Type type)
    {
        switch (type)
        {
            case { IsArray: true }:
                return new[] { type.GetElementType() };

            case { IsGenericType: true } when type.GetGenericTypeDefinition() == typeof(IEnumerable<>):
                return type.GetGenericArguments();

            default:
                {
                    var enumType = type.GetInterfaces()
                        .Where(t => t.IsGenericType &&
                                    t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        .Select(t => t.GenericTypeArguments).FirstOrDefault();
                    return enumType ?? new[] { type };
                }
        }
    }

    internal static string GetPropertyName<TSource, TProperty>(this Expression<Func<TSource, TProperty>> property)
    {
        return property.GetMemberExpression().Member.Name;
    }

    internal static bool IsNumericType(this object? o)
    {
        if (o == null) return false;

        switch (Type.GetTypeCode(o.GetType()))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;

            case TypeCode.Empty:
            case TypeCode.Object:
            case TypeCode.DBNull:
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.DateTime:
            case TypeCode.String:
            default:
                return false;
        }
    }

    internal static bool IsValidRegex(this string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern)) return false;
        try
        {
            var _ = Regex.Match("", pattern);
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }

    internal static long LongCount(this IEnumerable enumerable, bool excludeNullValues = false)
    {
        var list = enumerable.Cast<object?>();
        if (excludeNullValues) list = list.Where(x => x != null);
        return Enumerable.LongCount(list);
    }

    internal static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var param = expr1.Parameters[0];
        if (ReferenceEquals(param, expr2.Parameters[0]))
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(expr1.Body, expr2.Body), param);
        }
        return Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                expr1.Body,
                Expression.Invoke(expr2, param)), param);
    }

    internal static string RemoveFirstAndLast(this string str)
    {
        if (string.IsNullOrEmpty(str)) throw new ArgumentException("Value cannot be null or empty.", nameof(str));
        if (str.Length < 2)
        {
            throw new Exception($"Length of {nameof(str)} cannot be less than 2.");
        }
        return str.Substring(1, str.Length - 2);
    }

    private static MemberExpression GetMemberExpression<TSource, TProperty>(
        this Expression<Func<TSource, TProperty>> property)
    {
        if (Equals(property, null))
        {
            throw new NullReferenceException($"{nameof(property)} is required");
        }

        MemberExpression expr;

        if (property.Body is MemberExpression body)
        {
            expr = body;
        }
        else if (property.Body is UnaryExpression expression)
        {
            expr = (MemberExpression)expression.Operand;
        }
        else
        {
            const string format = "Expression '{0}' not supported.";
            var message = string.Format(format, property);

            throw new ArgumentException(message, nameof(property));
        }

        return expr;
    }
}