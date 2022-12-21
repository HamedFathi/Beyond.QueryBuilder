namespace Beyond.QueryBuilder.Helpers;

internal struct ParenthesesPair
{
    internal ParenthesesPair(int id, int startIndex, int endIndex, int depth, string value)
    {
        if (startIndex > endIndex)
            throw new ArgumentException("startIndex must be less than endIndex");

        StartIndex = startIndex;
        EndIndex = endIndex;
        Depth = depth;
        Value = value;
        Id = id;
    }

    internal int Depth { get; }
    internal int EndIndex { get; }
    internal int Id { get; }
    internal int StartIndex { get; }
    internal string Value { get; }
}