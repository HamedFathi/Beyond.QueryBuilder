// ReSharper disable UnusedMember.Global

using System.Text.RegularExpressions;

namespace Beyond.QueryBuilder.Helpers;

internal static class NotationConverter
{
    internal static string CompleteParenthesisOfInfix(string infix, string operandPattern)
    {
        if (string.IsNullOrWhiteSpace(infix))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(infix));
        infix = infix.RemoveWhitespace();
        var postfix = InfixToPostfix(infix, operandPattern);
        var newInfix = PostfixToInfix(postfix, operandPattern);
        if (!newInfix.StartsWith("(") || !newInfix.EndsWith(")"))
        {
            newInfix = $"({newInfix})";
        }
        return newInfix;
    }

    internal static bool IsValidInfix(string infix, string operandPattern = "[a-zA-Z]", string operatorsAndParenthesesPattern = @"\+|-|\*|/|\^|\||&|\(|\)")
    {
        if (string.IsNullOrWhiteSpace(infix))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(infix));

        infix = infix.RemoveWhitespace();
        var condition = IsWellFormattedParentheses(infix);
        var prev = string.Empty;
        var matches = Regex.Matches(infix, $"{operandPattern}|{operatorsAndParenthesesPattern}");

        foreach (Match match in matches)
        {
            var val = match.Value;

            if (val is "(" or ")") continue;

            if (prev != string.Empty)
            {
                var prevOperand = Regex.IsMatch(prev, operandPattern);
                var valOperand = Regex.IsMatch(val, operandPattern);
                if (prevOperand == valOperand)
                {
                    return false;
                }
                var prevOperator = Regex.IsMatch(prev, operatorsAndParenthesesPattern);
                var valOperator = Regex.IsMatch(val, operatorsAndParenthesesPattern);
                if (prevOperator == valOperator)
                {
                    return false;
                }
            }
            prev = val;
        }
        infix = infix.Replace("(", "").Replace(")", "");
        var condition2 = !Regex.IsMatch(infix[0].ToString(), operatorsAndParenthesesPattern) && !Regex.IsMatch(infix[^1].ToString(), operatorsAndParenthesesPattern);

        return condition && condition2;
    }

    private static string InfixToPostfix(string infix, string operandPattern = "[a-zA-Z]", string operatorsAndParenthesesPattern = @"\+|-|\*|/|\^|\||&|\(|\)", Func<string, int>? precedence = null)
    {
        if (string.IsNullOrWhiteSpace(infix))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(infix));
        if (string.IsNullOrWhiteSpace(operandPattern))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(operandPattern));
        if (!operandPattern.IsValidRegex())
            throw new ArgumentException("Value is not valid for a regex pattern.", nameof(operandPattern));
        if (string.IsNullOrWhiteSpace(operatorsAndParenthesesPattern))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(operatorsAndParenthesesPattern));
        if (!operatorsAndParenthesesPattern.IsValidRegex())
            throw new ArgumentException("Value is not valid for a regex pattern.", nameof(operatorsAndParenthesesPattern));

        var parentheses = new[] { "(", ")" };
        precedence ??= Precedence;
        var result = string.Empty;
        string @out;
        var stack = new Stack<string>();
        infix = infix.RemoveWhitespace();
        var matches = Regex.Matches(infix, $"{operandPattern}|{operatorsAndParenthesesPattern}");
        foreach (Match match in matches)
        {
            var val = match.Value;
            if (!parentheses.Contains(val) && !Regex.IsMatch(val, operatorsAndParenthesesPattern))
            {
                result += val;
            }
            else
            {
                if (!parentheses.Contains(val) && Regex.IsMatch(val, operatorsAndParenthesesPattern))
                {
                    while (stack.Count > 0 && precedence(val) <= precedence(stack.Peek()))
                    {
                        @out = stack.Peek();
                        stack.Pop();
                        result = result + " " + @out;
                    }
                    stack.Push(val);
                    result += " ";
                }
                else if (val == "(")
                {
                    stack.Push(val);
                }
                else if (val == ")")
                {
                    while (stack.Count > 0 && (@out = stack.Peek()) != "(")
                    {
                        stack.Pop();
                        result = result + " " + @out + " ";
                    }

                    if (stack.Count > 0 && stack.Peek() == "(")
                        stack.Pop();
                }
            }
        }
        while (stack.Count > 0)
        {
            @out = stack.Peek();
            stack.Pop();
            result = result + @out + " ";
        }
        return result.RemoveWhitespace();
    }

    private static bool IsWellFormattedParentheses(string str)
    {
        var lastOpen = new Stack<char>();
        foreach (var c in str)
        {
            switch (c)
            {
                case ')':
                    if (lastOpen.Count == 0 || lastOpen.Pop() != '(') return false;
                    break;

                case ']':
                    if (lastOpen.Count == 0 || lastOpen.Pop() != '[') return false;
                    break;

                case '}':
                    if (lastOpen.Count == 0 || lastOpen.Pop() != '{') return false;
                    break;

                case '(':
                case '[':
                case '{': lastOpen.Push(c); break;
            }
        }
        return lastOpen.Count == 0;
    }

    private static string PostfixToInfix(string postfix, string operandPattern = "[a-zA-Z]", string operatorsAndParenthesesPattern = @"\+|-|\*|/|\^|\||&|\(|\)")
    {
        if (string.IsNullOrWhiteSpace(postfix))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(postfix));
        if (string.IsNullOrWhiteSpace(operandPattern))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(operandPattern));
        if (string.IsNullOrWhiteSpace(operatorsAndParenthesesPattern))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(operatorsAndParenthesesPattern));

        if (!operandPattern.IsValidRegex())
            throw new ArgumentException("Value is not valid for a regex pattern.", nameof(operandPattern));
        if (!operatorsAndParenthesesPattern.IsValidRegex())
            throw new ArgumentException("Value is not valid for a regex pattern.", nameof(operatorsAndParenthesesPattern));

        var stackOperands = new Stack<string>();
        postfix = postfix.RemoveWhitespace();
        var matches = Regex.Matches(postfix, $"{operandPattern}|{operatorsAndParenthesesPattern}");
        foreach (Match match in matches)
        {
            var val = match.Value;
            if (!Regex.IsMatch(val, operatorsAndParenthesesPattern))
            {
                stackOperands.Push(val);
            }
            else
            {
                var operandOld = stackOperands.Pop();
                var operandOlder = stackOperands.Pop();
                var newStr = $"({operandOlder}{val}{operandOld})";
                stackOperands.Push(newStr);
            }
        }
        return stackOperands.Count == 1 ? stackOperands.Pop() : string.Empty;
    }

    private static int Precedence(this string op)
    {
        if (op is "*" or "/" or "%" or "&")
            return 3;
        if (op is "+" or "-" or "|")
            return 2;
        if (op == "^")
            return 1;
        return -1;
    }

    private static string RemoveWhitespace(this string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray());
    }
}