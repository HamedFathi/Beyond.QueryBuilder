// ReSharper disable UnusedMember.Global
namespace Beyond.QueryBuilder.Helpers;

internal static class ParenthesesHelper
{
    internal static IEnumerable<ParenthesesPair> ParseParenthesesPairs(string text, char[]? pairs = null)
    {
        if (pairs != null && pairs.Length != 2)
        {
            throw new Exception($"{nameof(pairs)} should contain only two items.");
        }

        if (pairs != null && pairs.Contains(' '))
        {
            throw new Exception($"{nameof(pairs)} should not contain whitespace.");
        }

        if (pairs != null && pairs[0] == pairs[1])
        {
            throw new Exception($"{nameof(pairs)} should not contain same value.");
        }

        pairs ??= new[] { '(', ')' };
        var result = new List<ParenthesesPair>();

        var startPositions = new Stack<int>();
        var id = 0;
        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] == pairs[0])
            {
                startPositions.Push(index);
            }
            else if (text[index] == pairs[1] && startPositions.Count == 0)
            {
                throw new ArgumentException($"Mismatched end at index {index}");
            }
            else if (text[index] == pairs[1])
            {
                id++;
                var depth = startPositions.Count - 1;
                var start = startPositions.Pop();
                var length = index - start + 1;
                var val = text.Substring(start, length);

                result.Add(new ParenthesesPair(id, start, index, depth, val));
            }
        }

        if (startPositions.Count > 0)
            throw new ArgumentException($"Mismatched start, {startPositions.Count} total");

        return result.OrderBy(x => x.Depth).ThenBy(x => x.StartIndex);
    }
}