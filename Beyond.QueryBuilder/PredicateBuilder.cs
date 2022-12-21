// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

using System.Linq.Expressions;

namespace Beyond.QueryBuilder;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> False<T>()
    { return _ => false; }

    public static Expression<Func<T, bool>> Nand<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var andExpr = expr1.And(expr2);
        return andExpr.Not();
    }

    public static Expression<Func<T, bool>> Nor<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var orExpr = expr1.Or(expr2);
        return orExpr.Not();
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
    {
        var notExpr = Expression.Not(expr.Body);
        return Expression.Lambda<Func<T, bool>>(notExpr, expr.Parameters);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> True<T>()
    { return _ => true; }

    public static Expression<Func<T, bool>> Xnor<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var xorExpr = expr1.Xor(expr2);
        return xorExpr.Not();
    }

    public static Expression<Func<T, bool>> Xor<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.ExclusiveOr(expr1.Body, invokedExpr), expr1.Parameters);
    }
}