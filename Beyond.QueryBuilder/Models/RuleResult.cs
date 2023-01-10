// ReSharper disable UnusedMember.Global
namespace Beyond.QueryBuilder.Models;

public class RuleResult
{
    public RuleResult()
    {
        Errors = new List<string>();
    }

    public IList<string> Errors { get; set; }
    public bool IsValid { get; set; }
    public string Name { get; set; } = null!;
}