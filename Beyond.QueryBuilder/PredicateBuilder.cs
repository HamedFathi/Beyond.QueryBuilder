// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

using System.Linq.Expressions;

namespace Beyond.QueryBuilder;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> AndWith<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> FalseExpression<T>()
    { return _ => false; }

    public static Expression<Func<T, bool>> NandWith<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var andExpr = expr1.AndWith(expr2);
        return andExpr.NotExpression();
    }

    public static Expression<Func<T, bool>> NorWith<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var orExpr = expr1.OrWith(expr2);
        return orExpr.NotExpression();
    }

    public static Expression<Func<T, bool>> NotExpression<T>(this Expression<Func<T, bool>> expr)
    {
        var notExpr = Expression.Not(expr.Body);
        return Expression.Lambda<Func<T, bool>>(notExpr, expr.Parameters);
    }

    public static Expression<Func<T, bool>> OrWith<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> TrueExpression<T>()
    { return _ => true; }

    public static Expression<Func<T, bool>> XnorWith<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var xorExpr = expr1.XorWith(expr2);
        return xorExpr.NotExpression();
    }

    public static Expression<Func<T, bool>> XorWith<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
        return Expression.Lambda<Func<T, bool>>
            (Expression.ExclusiveOr(expr1.Body, invokedExpr), expr1.Parameters);
    }
}