using Beyond.QueryBuilder.Models;

namespace Beyond.QueryBuilder.Helpers;

internal static class Utilities
{
    internal static OperatorInfo GetOperatorInfo(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

        if (name.StartsWith("Has") || name.StartsWith("Is"))
        {
            return new OperatorInfo
            {
                Name = name,
                AcceptsArrayAsSecondOperand = false,
                AcceptsSecondOperand = false
            };
        }

        if (name.Contains("Between") || name.Contains("InOrdinal"))
        {
            return new OperatorInfo
            {
                Name = name,
                AcceptsArrayAsSecondOperand = true,
                AcceptsSecondOperand = true
            };
        }

        return new OperatorInfo
        {
            Name = name,
            AcceptsArrayAsSecondOperand = false,
            AcceptsSecondOperand = true
        };
    }
}