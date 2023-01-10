using Beyond.QueryBuilder.Models;

namespace Beyond.QueryBuilder;

public interface IRule<in T>
{
    public string Name { get; set; }

    void Execute(T data);

    RuleResult Validate(T data);
}