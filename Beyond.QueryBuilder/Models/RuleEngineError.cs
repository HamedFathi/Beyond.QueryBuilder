namespace Beyond.QueryBuilder.Models;

public class RuleEngineError
{
    public RuleEngineError()
    {
        Errors = new List<string>();
    }

    public IList<string> Errors { get; set; }
    public string Name { get; set; } = null!;
}