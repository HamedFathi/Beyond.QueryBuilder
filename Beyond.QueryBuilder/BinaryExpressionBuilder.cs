// ReSharper disable UnusedParameter.Local

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Beyond.QueryBuilder.Helpers;
using Beyond.QueryBuilder.Models;

namespace Beyond.QueryBuilder;

internal static class BinaryExpressionBuilder
{
    private static readonly MethodInfo? BetweenExclusiveMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyBetweenExclusive),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? BetweenInclusiveMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyBetweenInclusive),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? ContainsCurrentCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyContainsCurrentCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? ContainsCurrentCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyContainsCurrentCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? ContainsInvariantCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyContainsInvariantCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? ContainsInvariantCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyContainsInvariantCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? ContainsOrdinalIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyContainsOrdinalIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? ContainsOrdinalMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyContainsOrdinal),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? EndsWithCurrentCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyEndsWithCurrentCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? EndsWithCurrentCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyEndsWithCurrentCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? EndsWithInvariantCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyEndsWithInvariantCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? EndsWithInvariantCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyEndsWithInvariantCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? EndsWithOrdinalIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyEndsWithOrdinalIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? EndsWithOrdinalMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyEndsWithOrdinal),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? HasLengthMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyHasLength),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? IsNotNullOrEmptyMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyIsNotNullOrEmpty),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? IsNotNullOrWhiteSpaceMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyIsNotNullOrWhiteSpace),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? IsNullOrEmptyMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyIsNullOrEmpty),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? IsNullOrWhiteSpaceMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyIsNullOrWhiteSpace),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LengthBetweenExclusiveMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLengthBetweenExclusive),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LengthBetweenInclusiveMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLengthBetweenInclusive),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LengthEqualMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLengthEqual),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LengthGreaterThanEqualMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLengthGreaterThanEqual),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LengthGreaterThanMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLengthGreaterThan),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LengthLessThanEqualMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLengthLessThanEqual),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LengthLessThanMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLengthLessThan),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LengthNotEqualMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLengthNotEqual),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LikesCurrentCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLikesCurrentCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LikesCurrentCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLikesCurrentCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LikesInvariantCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLikesInvariantCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LikesInvariantCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLikesInvariantCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LikesOrdinalIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLikesOrdinalIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? LikesOrdinalMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyLikesOrdinal),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? MatchesMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyMatches),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotBetweenExclusiveMethod =
    typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotBetweenExclusive),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotBetweenInclusiveMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotBetweenInclusive),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotContainCurrentCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotContainCurrentCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotContainCurrentCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotContainCurrentCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotContainInvariantCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotContainInvariantCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotContainInvariantCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotContainInvariantCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotContainOrdinalIgnoreCaseMethod =
                        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotContainOrdinalIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotContainOrdinalMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotContainOrdinal),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotEndWithCurrentCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotEndWithCurrentCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotEndWithCurrentCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotEndWithCurrentCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotEndWithInvariantCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotEndWithInvariantCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotEndWithInvariantCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotEndWithInvariantCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotEndWithOrdinalIgnoreCaseMethod =
                        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotEndWithOrdinalIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotEndWithOrdinalMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotEndWithOrdinal),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotLikeCurrentCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotLikeCurrentCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotLikeCurrentCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotLikeCurrentCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotLikeInvariantCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotLikeInvariantCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotLikeInvariantCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotLikeInvariantCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotLikeOrdinalIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotLikeOrdinalIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotLikeOrdinalMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotLikeOrdinal),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotMatchMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotMatch),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotStartWithCurrentCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotStartWithCurrentCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotStartWithCurrentCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotStartWithCurrentCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotStartWithInvariantCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotStartWithInvariantCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotStartWithInvariantCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotStartWithInvariantCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotStartWithOrdinalIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotStartWithOrdinalIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? NotStartWithOrdinalMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyNotStartWithOrdinal),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? StartsWithCurrentCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyStartsWithCurrentCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? StartsWithCurrentCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyStartsWithCurrentCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? StartsWithInvariantCultureIgnoreCaseMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyStartsWithInvariantCultureIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? StartsWithInvariantCultureMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyStartsWithInvariantCulture),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? StartsWithOrdinalIgnoreCaseMethod =
    typeof(BinaryExpressionBuilder).GetMethod(nameof(MyStartsWithOrdinalIgnoreCase),
            BindingFlags.NonPublic | BindingFlags.Static);

    private static readonly MethodInfo? StartsWithOrdinalMethod =
        typeof(BinaryExpressionBuilder).GetMethod(nameof(MyStartsWithOrdinal),
            BindingFlags.NonPublic | BindingFlags.Static);

    public static BinaryExpression MyInOrdinal(Expression value, QueryRule? array)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (array.Value is IEnumerable enumerable)
        {
            var v = enumerable.Cast<object>().ToArray();

            var arrayExpr = Expression.Constant(v, typeof(object[]));
            var containsMethod = typeof(Enumerable).GetMethods()
                .Where(x => string.Equals(x.Name, "Contains", StringComparison.Ordinal))
                .Single(x => x.GetParameters().Length == 2);

            var containsMethodGeneric = containsMethod.MakeGenericMethod(typeof(object));
            var convertedExpr = Expression.Convert(value, typeof(object));
            var containsCall = Expression.Call(containsMethodGeneric, arrayExpr, convertedExpr);

            return Expression.MakeBinary(ExpressionType.Equal, containsCall, Expression.Constant(true));
        }

        throw new Exception("Operator does not support this type.");
    }

    public static BinaryExpression MyInOrdinalIgnoreCase(Expression value, QueryRule? array)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (array.Value is IEnumerable enumerable)
        {
            var v = enumerable.Cast<object>().ToArray();

            var arrayExpr = Expression.Constant(v, typeof(object[]));
            var containsMethod = typeof(Enumerable).GetMethods()
                .Where(x => string.Equals(x.Name, "Contains", StringComparison.OrdinalIgnoreCase))
                .Single(x => x.GetParameters().Length == 2);

            var containsMethodGeneric = containsMethod.MakeGenericMethod(typeof(object));
            var convertedExpr = Expression.Convert(value, typeof(object));
            var containsCall = Expression.Call(containsMethodGeneric, arrayExpr, convertedExpr);

            return Expression.MakeBinary(ExpressionType.Equal, containsCall, Expression.Constant(true));
        }

        throw new Exception("Operator does not support this type.");
    }

    public static BinaryExpression MyNotInOrdinal(Expression value, QueryRule? array)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (array.Value is IEnumerable enumerable)
        {
            var v = enumerable.Cast<object>().ToArray();

            var arrayExpr = Expression.Constant(v, typeof(object[]));
            var containsMethod = typeof(Enumerable).GetMethods()
                .Where(x => string.Equals(x.Name, "Contains", StringComparison.Ordinal))
                .Single(x => x.GetParameters().Length == 2);

            var containsMethodGeneric = containsMethod.MakeGenericMethod(typeof(object));
            var convertedExpr = Expression.Convert(value, typeof(object));
            var containsCall = Expression.Call(containsMethodGeneric, arrayExpr, convertedExpr);

            return Expression.MakeBinary(ExpressionType.Equal, containsCall, Expression.Constant(false));
        }

        throw new Exception("Operator does not support this type.");
    }

    public static BinaryExpression MyNotInOrdinalIgnoreCase(Expression value, QueryRule? array)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (array.Value is IEnumerable enumerable)
        {
            var v = enumerable.Cast<object>().ToArray();

            var arrayExpr = Expression.Constant(v, typeof(object[]));
            var containsMethod = typeof(Enumerable).GetMethods()
                .Where(x => string.Equals(x.Name, "Contains", StringComparison.OrdinalIgnoreCase))
                .Single(x => x.GetParameters().Length == 2);

            var containsMethodGeneric = containsMethod.MakeGenericMethod(typeof(object));
            var convertedExpr = Expression.Convert(value, typeof(object));
            var containsCall = Expression.Call(containsMethodGeneric, arrayExpr, convertedExpr);

            return Expression.MakeBinary(ExpressionType.Equal, containsCall, Expression.Constant(false));
        }

        throw new Exception("Operator does not support this type.");
    }

    internal static BinaryExpression GetBinaryExpression(this string @operator, MemberExpression memberExpression,
                        ConstantExpression constantExpression, QueryRule? queryRule)
    {
        if (string.IsNullOrWhiteSpace(@operator))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(@operator));

        return @operator switch
        {
            "ContainsOrdinalIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, ContainsOrdinalIgnoreCaseMethod),
            "ContainsOrdinal" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, ContainsOrdinalMethod),
            "ContainsCurrentCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, ContainsCurrentCultureMethod),
            "ContainsCurrentCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, ContainsCurrentCultureIgnoreCaseMethod),
            "ContainsInvariantCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, ContainsInvariantCultureMethod),
            "ContainsInvariantCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, ContainsInvariantCultureIgnoreCaseMethod),

            "EndsWithOrdinalIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, EndsWithOrdinalIgnoreCaseMethod),
            "EndsWithOrdinal" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, EndsWithOrdinalMethod),
            "EndsWithCurrentCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, EndsWithCurrentCultureMethod),
            "EndsWithCurrentCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, EndsWithCurrentCultureIgnoreCaseMethod),
            "EndsWithInvariantCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, EndsWithInvariantCultureMethod),
            "EndsWithInvariantCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, EndsWithInvariantCultureIgnoreCaseMethod),

            "NotEndWithCurrentCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotEndWithCurrentCultureMethod),
            "NotEndWithCurrentCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotEndWithCurrentCultureIgnoreCaseMethod),
            "NotEndWithInvariantCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotEndWithInvariantCultureMethod),
            "NotEndWithInvariantCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotEndWithInvariantCultureIgnoreCaseMethod),
            "NotEndWithOrdinal" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotEndWithOrdinalMethod),
            "NotEndWithOrdinalIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotEndWithOrdinalIgnoreCaseMethod),

            "StartsWithOrdinalIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, StartsWithOrdinalIgnoreCaseMethod),
            "StartsWithOrdinal" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, StartsWithOrdinalMethod),
            "StartsWithCurrentCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, StartsWithCurrentCultureMethod),
            "StartsWithCurrentCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, StartsWithCurrentCultureIgnoreCaseMethod),
            "StartsWithInvariantCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, StartsWithInvariantCultureMethod),
            "StartsWithInvariantCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, StartsWithInvariantCultureIgnoreCaseMethod),

            "NotStartWithCurrentCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, NotStartWithCurrentCultureMethod),
            "NotStartWithCurrentCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, NotStartWithCurrentCultureIgnoreCaseMethod),
            "NotStartWithInvariantCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, NotStartWithInvariantCultureMethod),
            "NotStartWithInvariantCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, NotStartWithInvariantCultureIgnoreCaseMethod),
            "NotStartWithOrdinal" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, NotStartWithOrdinalMethod),
            "NotStartWithOrdinalIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, NotStartWithOrdinalIgnoreCaseMethod),

            "Equals" => Expression.Equal(memberExpression, constantExpression),
            "NotEqual" => Expression.NotEqual(memberExpression, constantExpression),

            "LessThan" => Expression.LessThan(memberExpression, constantExpression),
            "LessThanEqual" => Expression.LessThanOrEqual(memberExpression, constantExpression),

            "GreaterThan" => Expression.GreaterThan(memberExpression, constantExpression),
            "GreaterThanEqual" => Expression.GreaterThanOrEqual(memberExpression, constantExpression),

            "BetweenInclusive" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, BetweenInclusiveMethod),
            "BetweenExclusive" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, BetweenExclusiveMethod),
            "NotBetweenInclusive" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, NotBetweenInclusiveMethod),
            "NotBetweenExclusive" => Expression.MakeBinary(ExpressionType.Equal, memberExpression, constantExpression,
                false, NotBetweenExclusiveMethod),

            "NotContainCurrentCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotContainCurrentCultureMethod),
            "NotContainCurrentCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotContainCurrentCultureIgnoreCaseMethod),
            "NotContainInvariantCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotContainInvariantCultureMethod),
            "NotContainInvariantCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotContainInvariantCultureIgnoreCaseMethod),
            "NotContainOrdinal" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotContainOrdinalMethod),
            "NotContainOrdinalIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotContainOrdinalIgnoreCaseMethod),

            "InOrdinal" => MyInOrdinal(memberExpression, queryRule),
            "InOrdinalIgnoreCase" => MyInOrdinalIgnoreCase(memberExpression, queryRule),
            "NotInOrdinal" => MyNotInOrdinal(memberExpression, queryRule),
            "NotInOrdinalIgnoreCase" => MyNotInOrdinalIgnoreCase(memberExpression, queryRule),

            "IsNotNullOrEmpty" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, IsNotNullOrEmptyMethod),
            "IsNullOrEmpty" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, IsNullOrEmptyMethod),

            "IsNotNullOrWhiteSpace" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, IsNotNullOrWhiteSpaceMethod),
            "IsNullOrWhiteSpace" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, IsNullOrWhiteSpaceMethod),

            "HasLength" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, HasLengthMethod),

            "LengthBetweenInclusive" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LengthBetweenInclusiveMethod),
            "LengthBetweenExclusive" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LengthBetweenExclusiveMethod),
            "LengthEqual" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LengthEqualMethod),
            "LengthGreaterThan" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LengthGreaterThanMethod),
            "LengthGreaterThanEqual" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LengthGreaterThanEqualMethod),
            "LengthLessThan" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LengthLessThanMethod),
            "LengthLessThanEqual" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LengthLessThanEqualMethod),
            "LengthNotEqual" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LengthNotEqualMethod),

            "LikesCurrentCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LikesCurrentCultureMethod),
            "LikesCurrentCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LikesCurrentCultureIgnoreCaseMethod),
            "LikesInvariantCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LikesInvariantCultureMethod),
            "LikesInvariantCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LikesInvariantCultureIgnoreCaseMethod),
            "LikesOrdinal" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LikesOrdinalMethod),
            "LikesOrdinalIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, LikesOrdinalIgnoreCaseMethod),

            "NotLikeCurrentCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotLikeCurrentCultureMethod),
            "NotLikeCurrentCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotLikeCurrentCultureIgnoreCaseMethod),
            "NotLikeInvariantCulture" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotLikeInvariantCultureMethod),
            "NotLikeInvariantCultureIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotLikeInvariantCultureIgnoreCaseMethod),
            "NotLikeOrdinal" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotLikeOrdinalMethod),
            "NotLikeOrdinalIgnoreCase" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotLikeOrdinalIgnoreCaseMethod),

            "Matches" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, MatchesMethod),
            "NotMatch" => Expression.MakeBinary(ExpressionType.Equal, memberExpression,
                constantExpression, false, NotMatchMethod),

            _ => throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null)
        };
    }

    private static bool MyBetweenExclusive(object? source, object? value)
    {
        if (source == null || value == null) return false;
        if (value is not IEnumerable enumerable)
        {
            throw new Exception("operator just supports array type for the second operand.");
        }
        var values = enumerable.Cast<object?>().ToList();
        if (source.IsNumericType())
        {
            var v = Convert.ToDecimal(source);
            var a1 = Convert.ToDecimal(values[0]);
            var a2 = Convert.ToDecimal(values[1]);
            return v > a1 && v < a2;
        }
        if (source is DateTime)
        {
            var v = Convert.ToDateTime(source);
            var a1 = Convert.ToDateTime(values[0]);
            var a2 = Convert.ToDateTime(values[1]);
            return v > a1 && v < a2;
        }
        throw new Exception("operator just supports array type for the second operand.");
    }

    private static bool MyBetweenInclusive(object? source, object? value)
    {
        if (source == null || value == null) return false;
        if (value is not IEnumerable enumerable)
        {
            throw new Exception("operator just supports array type for the second operand.");
        }
        var values = enumerable.Cast<object?>().ToList();
        if (source.IsNumericType())
        {
            var v = Convert.ToDecimal(source);
            var a1 = Convert.ToDecimal(values[0]);
            var a2 = Convert.ToDecimal(values[1]);
            return v >= a1 && v <= a2;
        }
        if (source is DateTime)
        {
            var v = Convert.ToDateTime(source);
            var a1 = Convert.ToDateTime(values[0]);
            var a2 = Convert.ToDateTime(values[1]);
            return v >= a1 && v <= a2;
        }
        throw new Exception("operator just supports the type.");
    }

    private static bool MyContainsCurrentCulture(string source, string target)
    {
        return source.Contains(target, StringComparison.CurrentCulture);
    }

    private static bool MyContainsCurrentCultureIgnoreCase(string source, string target)
    {
        return source.Contains(target, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool MyContainsInvariantCulture(string source, string target)
    {
        return source.Contains(target, StringComparison.InvariantCulture);
    }

    private static bool MyContainsInvariantCultureIgnoreCase(string source, string target)
    {
        return source.Contains(target, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool MyContainsOrdinal(string source, string target)
    {
        return source.Contains(target, StringComparison.Ordinal);
    }

    private static bool MyContainsOrdinalIgnoreCase(string source, string target)
    {
        return source.Contains(target, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MyEndsWithCurrentCulture(string source, string target)
    {
        return source.EndsWith(target, StringComparison.CurrentCulture);
    }

    private static bool MyEndsWithCurrentCultureIgnoreCase(string source, string target)
    {
        return source.EndsWith(target, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool MyEndsWithInvariantCulture(string source, string target)
    {
        return source.EndsWith(target, StringComparison.InvariantCulture);
    }

    private static bool MyEndsWithInvariantCultureIgnoreCase(string source, string target)
    {
        return source.EndsWith(target, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool MyEndsWithOrdinal(string source, string target)
    {
        return source.EndsWith(target, StringComparison.Ordinal);
    }

    private static bool MyEndsWithOrdinalIgnoreCase(string source, string target)
    {
        return source.EndsWith(target, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MyHasLength(object source, object value)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() > 0;
        }
        return false;
    }

    private static bool MyIsNotNullOrEmpty(object? source, object? target)
    {
        if (source == null)
            return false;
        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() != 0;
        }
        return !string.IsNullOrEmpty(source.ToString());
    }

    private static bool MyIsNotNullOrWhiteSpace(string source, object? target)
    {
        return !string.IsNullOrWhiteSpace(source);
    }

    private static bool MyIsNullOrEmpty(object? source, object? target)
    {
        if (source != null)
            return false;

        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() == 0;
        }
        return string.IsNullOrEmpty(source?.ToString());
    }

    private static bool MyIsNullOrWhiteSpace(string source, object? target)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    private static bool MyLengthBetweenExclusive(object? source, object value)
    {
        if (source is IEnumerable enumerable && value is IEnumerable v)
        {
            var enumerable1 = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
            var values = v as object[] ?? v.Cast<object>().ToArray();
            var result = enumerable1.LongLength > Convert.ToInt64(values[0]) && enumerable1.Length < Convert.ToInt64(values[1]);
            return result;
        }
        throw new Exception("operator just supports enumerable types.");
    }

    private static bool MyLengthBetweenInclusive(object? source, object value)
    {
        if (source is IEnumerable enumerable && value is IEnumerable v)
        {
            var enumerable1 = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
            var values = v as object[] ?? v.Cast<object>().ToArray();
            var result = enumerable1.LongLength >= Convert.ToInt64(values[0]) && enumerable1.Length <= Convert.ToInt64(values[1]);
            return result;
        }
        throw new Exception("operator just supports enumerable types.");
    }

    private static bool MyLengthEqual(object source, object value)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() == Convert.ToInt64(value);
        }
        return false;
    }

    private static bool MyLengthGreaterThan(object source, object value)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() > Convert.ToInt64(value);
        }
        return false;
    }

    private static bool MyLengthGreaterThanEqual(object source, object value)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() >= Convert.ToInt64(value);
        }
        return false;
    }

    private static bool MyLengthLessThan(object source, object value)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() < Convert.ToInt64(value);
        }
        return false;
    }

    private static bool MyLengthLessThanEqual(object source, object value)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() <= Convert.ToInt64(value);
        }
        return false;
    }

    private static bool MyLengthNotEqual(object source, object value)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        if (source is IEnumerable enumerable)
        {
            return enumerable.LongCount() != Convert.ToInt64(value);
        }
        return false;
    }

    private static bool MyLikesCurrentCulture(string source, string target)
    {
        return
            source.StartsWith(target, StringComparison.CurrentCulture)
            || source.EndsWith(target, StringComparison.CurrentCulture)
            || source.Contains(target, StringComparison.CurrentCulture)
            ;
    }

    private static bool MyLikesCurrentCultureIgnoreCase(string source, string target)
    {
        return
            source.StartsWith(target, StringComparison.CurrentCultureIgnoreCase)
            || source.EndsWith(target, StringComparison.CurrentCultureIgnoreCase)
            || source.Contains(target, StringComparison.CurrentCultureIgnoreCase)
            ;
    }

    private static bool MyLikesInvariantCulture(string source, string target)
    {
        return
            source.StartsWith(target, StringComparison.InvariantCulture)
            || source.EndsWith(target, StringComparison.InvariantCulture)
            || source.Contains(target, StringComparison.InvariantCulture)
            ;
    }

    private static bool MyLikesInvariantCultureIgnoreCase(string source, string target)
    {
        return
            source.StartsWith(target, StringComparison.InvariantCulture)
            || source.EndsWith(target, StringComparison.InvariantCulture)
            || source.Contains(target, StringComparison.InvariantCulture)
            ;
    }

    private static bool MyLikesOrdinal(string source, string target)
    {
        return
            source.StartsWith(target, StringComparison.Ordinal)
            || source.EndsWith(target, StringComparison.Ordinal)
            || source.Contains(target, StringComparison.Ordinal)
            ;
    }

    private static bool MyLikesOrdinalIgnoreCase(string source, string target)
    {
        return
            source.StartsWith(target, StringComparison.OrdinalIgnoreCase)
            || source.EndsWith(target, StringComparison.OrdinalIgnoreCase)
            || source.Contains(target, StringComparison.OrdinalIgnoreCase)
            ;
    }

    private static bool MyMatches(string source, string pattern)
    {
        var regex = new Regex(pattern);
        return regex.IsMatch(source);
    }

    private static bool MyNotBetweenExclusive(object? source, object? value)
    {
        if (source == null || value == null) return false;
        if (value is not IEnumerable enumerable)
        {
            throw new Exception("operator just supports array type for the second operand.");
        }
        var values = enumerable.Cast<object?>().ToList();
        if (source.IsNumericType())
        {
            var v = Convert.ToDecimal(source);
            var a1 = Convert.ToDecimal(values[0]);
            var a2 = Convert.ToDecimal(values[1]);
            return !(v > a1 && v < a2);
        }
        if (source is DateTime)
        {
            var v = Convert.ToDateTime(source);
            var a1 = Convert.ToDateTime(values[0]);
            var a2 = Convert.ToDateTime(values[1]);
            return !(v > a1 && v < a2);
        }
        throw new Exception("operator just supports the type.");
    }

    private static bool MyNotBetweenInclusive(object? source, object? value)
    {
        if (source == null || value == null) return false;
        if (value is not IEnumerable enumerable)
        {
            throw new Exception("operator just supports array type for the second operand.");
        }
        var values = enumerable.Cast<object?>().ToList();
        if (source.IsNumericType())
        {
            var v = Convert.ToDecimal(source);
            var a1 = Convert.ToDecimal(values[0]);
            var a2 = Convert.ToDecimal(values[1]);
            return !(v >= a1 && v <= a2);
        }
        if (source is DateTime)
        {
            var v = Convert.ToDateTime(source);
            var a1 = Convert.ToDateTime(values[0]);
            var a2 = Convert.ToDateTime(values[1]);
            return !(v >= a1 && v <= a2);
        }
        throw new Exception("operator just supports the type.");
    }

    private static bool MyNotContainCurrentCulture(string source, string target)
    {
        return source.Contains(target, StringComparison.CurrentCulture);
    }

    private static bool MyNotContainCurrentCultureIgnoreCase(string source, string target)
    {
        return source.Contains(target, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool MyNotContainInvariantCulture(string source, string target)
    {
        return source.Contains(target, StringComparison.InvariantCulture);
    }

    private static bool MyNotContainInvariantCultureIgnoreCase(string source, string target)
    {
        return source.Contains(target, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool MyNotContainOrdinal(string source, string target)
    {
        return source.Contains(target, StringComparison.Ordinal);
    }

    private static bool MyNotContainOrdinalIgnoreCase(string source, string target)
    {
        return source.Contains(target, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MyNotEndWithCurrentCulture(string source, string target)
    {
        return source.EndsWith(target, StringComparison.CurrentCulture);
    }

    private static bool MyNotEndWithCurrentCultureIgnoreCase(string source, string target)
    {
        return source.EndsWith(target, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool MyNotEndWithInvariantCulture(string source, string target)
    {
        return source.EndsWith(target, StringComparison.InvariantCulture);
    }

    private static bool MyNotEndWithInvariantCultureIgnoreCase(string source, string target)
    {
        return source.EndsWith(target, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool MyNotEndWithOrdinal(string source, string target)
    {
        return source.EndsWith(target, StringComparison.Ordinal);
    }

    private static bool MyNotEndWithOrdinalIgnoreCase(string source, string target)
    {
        return source.EndsWith(target, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MyNotLikeCurrentCulture(string source, string target)
    {
        return !MyLikesCurrentCulture(source, target);
    }

    private static bool MyNotLikeCurrentCultureIgnoreCase(string source, string target)
    {
        return !MyLikesCurrentCultureIgnoreCase(source, target);
    }

    private static bool MyNotLikeInvariantCulture(string source, string target)
    {
        return !MyLikesInvariantCulture(source, target);
    }

    private static bool MyNotLikeInvariantCultureIgnoreCase(string source, string target)
    {
        return !MyLikesInvariantCultureIgnoreCase(source, target);
    }

    private static bool MyNotLikeOrdinal(string source, string target)
    {
        return !MyLikesOrdinal(source, target);
    }

    private static bool MyNotLikeOrdinalIgnoreCase(string source, string target)
    {
        return !MyLikesOrdinalIgnoreCase(source, target);
    }

    private static bool MyNotMatch(string source, string pattern)
    {
        var regex = new Regex(pattern);
        return regex.IsMatch(source);
    }

    private static bool MyNotStartWithCurrentCulture(string source, string target)
    {
        return source.StartsWith(target, StringComparison.CurrentCulture);
    }

    private static bool MyNotStartWithCurrentCultureIgnoreCase(string source, string target)
    {
        return source.StartsWith(target, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool MyNotStartWithInvariantCulture(string source, string target)
    {
        return source.StartsWith(target, StringComparison.InvariantCulture);
    }

    private static bool MyNotStartWithInvariantCultureIgnoreCase(string source, string target)
    {
        return source.StartsWith(target, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool MyNotStartWithOrdinal(string source, string target)
    {
        return source.StartsWith(target, StringComparison.Ordinal);
    }

    private static bool MyNotStartWithOrdinalIgnoreCase(string source, string target)
    {
        return source.StartsWith(target, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MyStartsWithCurrentCulture(string source, string target)
    {
        return source.StartsWith(target, StringComparison.CurrentCulture);
    }

    private static bool MyStartsWithCurrentCultureIgnoreCase(string source, string target)
    {
        return source.StartsWith(target, StringComparison.CurrentCultureIgnoreCase);
    }

    private static bool MyStartsWithInvariantCulture(string source, string target)
    {
        return source.StartsWith(target, StringComparison.InvariantCulture);
    }

    private static bool MyStartsWithInvariantCultureIgnoreCase(string source, string target)
    {
        return source.StartsWith(target, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool MyStartsWithOrdinal(string source, string target)
    {
        return source.StartsWith(target, StringComparison.Ordinal);
    }

    private static bool MyStartsWithOrdinalIgnoreCase(string source, string target)
    {
        return source.StartsWith(target, StringComparison.OrdinalIgnoreCase);
    }
}