namespace Beyond.QueryBuilder.Models;

public class RuleEngineResult
{
    public RuleEngineResult()
    {
        Errors = new List<RuleEngineError>();
    }

    public IList<RuleEngineError> Errors { get; set; }
    public bool IsSuccessful { get; set; }
}