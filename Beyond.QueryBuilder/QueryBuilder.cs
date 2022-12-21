// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

using Beyond.QueryBuilder.Enums;
using Beyond.QueryBuilder.Helpers;
using Beyond.QueryBuilder.Models;
using System.Collections;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Beyond.QueryBuilder;

public class QueryBuilder<T>
{
    private const string OperandPattern = @"#\d+";
    private readonly List<Tuple<string, QueryRule>> _rules;
    private int _id;
    private string _infix = string.Empty;

    public QueryBuilder()
    {
        _id = -1;
        _rules = new List<Tuple<string, QueryRule>>();
    }

    public QueryBuilder<T> And()
    {
        // And is &
        _rules.Add(new Tuple<string, QueryRule>("&", new QueryRule()));
        return this;
    }

    public QueryBuilder<T> Between<TU>(Expression<Func<T, TU>> property, TU lower, TU upper, RangeType rangeType)
        where TU : struct, IConvertible
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "Between" + rangeType;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = new[] { lower, upper },
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> Build()
    {
        _infix = _rules.Select(x => x.Item1).Aggregate((a, b) => a + b);
        var status = NotationConverter.IsValidInfix(_infix, OperandPattern);
        if (!status)
        {
            throw new Exception($"The query builder structure is invalid. It looks like {_infix}");
        }

        _infix = NotationConverter.CompleteParenthesisOfInfix(_infix, OperandPattern);

        return this;
    }

    public QueryBuilder<T> Contains(Expression<Func<T, string>> property, string value,
        StringComparison stringComparison)
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "Contains" + stringComparison;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> EndsWith<TU>(Expression<Func<T, TU>> property, string value,
        StringComparison stringComparison)
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "EndsWith" + stringComparison;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> Equals<TU>(Expression<Func<T, TU>> property, TU value)
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "Equals";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> GreaterThan<TU>(Expression<Func<T, TU>> property, TU value)
        where TU : struct, IConvertible
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "GreaterThan";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> GreaterThanEqual<TU>(Expression<Func<T, TU>> property, TU value)
        where TU : struct, IConvertible
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "GreaterThanEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> Group(Expression<Func<QueryBuilder<T>, QueryBuilder<T>>> queryBuilder)
    {
        _rules.Add(new Tuple<string, QueryRule>("(", new QueryRule()));
        queryBuilder.Compile().Invoke(this);
        _rules.Add(new Tuple<string, QueryRule>(")", new QueryRule()));

        return this;
    }

    public QueryBuilder<T> HasLength<TU>(Expression<Func<T, TU>> property) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "HasLength";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = null,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> In<TU>(Expression<Func<T, TU>> property, TU[] values, bool ignoreCase)
    {
        _id++;
        var propName = property.GetPropertyName();
        string operatorName = ignoreCase ? "InOrdinal" : "InOrdinalIgnoreCase";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = values,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> IsNotNullOrEmpty<TU>(Expression<Func<T, TU>> property)
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "IsNotNullOrEmpty";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = null,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> IsNotNullOrWhiteSpace(Expression<Func<T, string>> property)
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "IsNotNullOrWhiteSpace";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = null,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> IsNullOrEmpty<TU>(Expression<Func<T, TU>> property)
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "IsNullOrEmpty";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = null,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> IsNullOrWhiteSpace(Expression<Func<T, string>> property)
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "IsNullOrWhiteSpace";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = null,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> LengthBetween<TU>(Expression<Func<T, TU>> property, int lower, int upper,
                                           RangeType rangeType) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "LengthBetween" + rangeType;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = new[] { lower.ToString(), upper.ToString(), },
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthBetween<TU>(Expression<Func<T, TU>> property, long lower, long upper,
        RangeType rangeType) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "LengthBetween" + rangeType;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = new[] { lower.ToString(), upper.ToString() },
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthEqual<TU>(Expression<Func<T, TU>> property, int length) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthEqual<TU>(Expression<Func<T, TU>> property, long length) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthGreaterThan<TU>(Expression<Func<T, TU>> property, int length) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthGreaterThan";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthGreaterThan<TU>(Expression<Func<T, TU>> property, long length)
        where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthGreaterThan";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthGreaterThanEqual<TU>(Expression<Func<T, TU>> property, int length)
        where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthGreaterThanEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthGreaterThanEqual<TU>(Expression<Func<T, TU>> property, long length)
        where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthGreaterThanEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthLessThan<TU>(Expression<Func<T, TU>> property, int length) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthLessThan";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthLessThan<TU>(Expression<Func<T, TU>> property, long length) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthLessThan";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthLessThanEqual<TU>(Expression<Func<T, TU>> property, int length)
        where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthLessThanEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthLessThanEqual<TU>(Expression<Func<T, TU>> property, long length)
        where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthLessThanEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthNotEqual<TU>(Expression<Func<T, TU>> property, int length) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthNotEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LengthNotEqual<TU>(Expression<Func<T, TU>> property, long length) where TU : IEnumerable
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LengthNotEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = length.ToString(),
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LessThan<TU>(Expression<Func<T, TU>> property, TU value) where TU : struct, IConvertible
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LessThan";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> LessThanEqual<TU>(Expression<Func<T, TU>> property, TU value)
        where TU : struct, IConvertible
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "LessThanEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> Likes(Expression<Func<T, string>> property, string value,
        StringComparison stringComparison)
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "Likes" + stringComparison;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> Matches(Expression<Func<T, string>> property, string regex)
    {
        _id++;
        var propName = property.GetPropertyName();

        if (!regex.IsValidRegex())
            throw new ArgumentException("Value is not valid for a regex pattern.", nameof(regex));

        var operatorName = "Matches";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = regex,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> NotBetween<TU>(Expression<Func<T, TU>> property, TU lower, TU upper, RangeType rangeType)
        where TU : struct, IConvertible
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "NotBetween" + rangeType;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = new[] { lower, upper },
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> NotContain(Expression<Func<T, string>> property, string value,
        StringComparison stringComparison)
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "NotContain" + stringComparison;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> NotEndWith<TU>(Expression<Func<T, TU>> property, string value,
        StringComparison stringComparison)
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "NotEndWith" + stringComparison;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> NotEqual<TU>(Expression<Func<T, TU>> property, TU value)
    {
        _id++;
        var propName = property.GetPropertyName();
        const string operatorName = "NotEqual";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> NotIn<TU>(Expression<Func<T, TU>> property, TU[] values, bool ignoreCase)
    {
        _id++;
        var propName = property.GetPropertyName();
        string operatorName = ignoreCase ? "NotInOrdinal" : "NotInOrdinalIgnoreCase";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = values,
            Type = GetTypeOf(typeof(TU))
        }));
        return this;
    }

    public QueryBuilder<T> NotLike(Expression<Func<T, string>> property, string value,
        StringComparison stringComparison)
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "NotLike" + stringComparison;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> NotMatch(Expression<Func<T, string>> property, string regex)
    {
        _id++;
        var propName = property.GetPropertyName();

        if (!regex.IsValidRegex())
            throw new ArgumentException("Value is not valid for a regex pattern.", nameof(regex));

        var operatorName = "NotMatch";

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = regex,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> NotStartWith<TU>(Expression<Func<T, TU>> property, string value,
        StringComparison stringComparison)
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "NotStartWith" + stringComparison;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = "string"
        }));
        return this;
    }

    public QueryBuilder<T> Or()
    {
        // And is |
        _rules.Add(new Tuple<string, QueryRule>("|", new QueryRule()));
        return this;
    }

    public QueryBuilder<T> StartsWith<TU>(Expression<Func<T, TU>> property, string value,
        StringComparison stringComparison)
    {
        _id++;
        var propName = property.GetPropertyName();
        var operatorName = "StartsWith" + stringComparison;

        _rules.Add(new Tuple<string, QueryRule>($"#{_id}", new QueryRule
        {
            Property = propName,
            Operator = operatorName,
            Value = value,
            Type = "string"
        }));
        return this;
    }

    public Func<T, bool> ToFunc()
    {
        return ToLambdaExpression().Compile();
    }

    public string ToJsonQueryDefinition(bool indented = false)
    {
        var q = ToQueryDefinition();
        return JsonSerializer.Serialize(q, new JsonSerializerOptions
        {
            WriteIndented = indented,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }

    public Expression<Func<T, bool>> ToLambdaExpression()
    {
        return ToQueryDefinition().ToLambdaExpression<T>();
    }

    public Query ToQueryDefinition()
    {
        if (string.IsNullOrEmpty(_infix))
        {
            throw new Exception("Seems you did not call Build().");
        }

        const string replacementPattern = @"___!@\d+!%___";
        var q = new Query();
        var insideGroups = new List<Tuple<string, string, QueryRule>>();
        var pairs = ParenthesesHelper.ParseParenthesesPairs(_infix).Reverse().ToList();
        foreach (var pair in pairs)
        {
            var value = pair.Value;

            var values = pairs.Where(x => x.Depth >= pair.Depth && x.Value != pair.Value).Reverse().ToList();
            foreach (var val in values)
            {
                value = value.Replace(val.Value, PatternMaker(val.Id));
            }

            var matches = Regex.Matches(value, OperandPattern + "|" + replacementPattern);
            var connector = value.Contains('&') ? "And" : "Or";
            var rules = new List<QueryRule>();
            foreach (Match match in matches)
            {
                var val = match.Value;

                var isOperand = Regex.IsMatch(val, OperandPattern);
                var isReplacement = Regex.IsMatch(val, replacementPattern);

                if (isOperand)
                {
                    var rule = _rules.Single(x => x.Item1 == val);
                    rules.Add(rule.Item2);
                }

                if (isReplacement)
                {
                    var rule = insideGroups.Single(x => x.Item1 == val);
                    rules.Add(rule.Item3);
                }
            }

            insideGroups.Add(new Tuple<string, string, QueryRule>(PatternMaker(pair.Id), pair.Value, new QueryRule
            {
                Connector = connector,
                Rules = rules
            }));
        }

        var last = insideGroups.LastOrDefault();
        if (last == null) return q;

        q.Connector = last.Item3.Connector;
        q.Rules ??= new List<QueryRule>();
        if (last.Item3.Rules != null)
            q.Rules.AddRange(last.Item3.Rules);
        return q;

        string PatternMaker(int value) => "___!@" + value + "!%___";
    }

    public string ToReadableDefinition()
    {
        if (string.IsNullOrWhiteSpace(_infix))
        {
            throw new Exception("Seems you did not call Build().");
        }

        var result = string.Empty;

        foreach (var rule in _rules)
        {
            result += rule.Item1 switch
            {
                "(" or ")" => rule.Item1,
                "&" => " AND ",
                "|" => " OR ",
                _ => rule.Item2.ToStringDefinition()
            };
        }

        return result;
    }

    private string GetTypeOf(Type type)
    {
        string t;
        var isIEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
        var elemTypes = type.GenericTypeArguments();
        if (isIEnumerable && type != typeof(string))
        {
            if (elemTypes.Length > 1)
            {
                t = "any[]";
            }
            else
            {
                if (elemTypes[0] == typeof(DateTime) || elemTypes[0] == typeof(DateTimeOffset)) t = "datetime[]";
                else if (elemTypes[0] == typeof(string)) t = "string[]";
                else if (elemTypes[0] == typeof(char)) t = "char[]";
                else if (elemTypes[0].IsNumericType()) t = "value[]";
                else if (elemTypes[0] == typeof(bool)) t = "boolean[]";
                else t = "any[]";
            }
        }
        else
        {
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) t = "datetime";
            else if (type == typeof(string)) t = "string";
            else if (type == typeof(char)) t = "char";
            else if (type.IsNumericType()) t = "value";
            else if (type == typeof(bool)) t = "boolean";
            else t = "any";
        }

        return t;
    }
}