// ReSharper disable UnusedMember.Global
using Beyond.QueryBuilder.Enums;
using Beyond.QueryBuilder.Models;

namespace Beyond.QueryBuilder;

public class RuleEngine<T>
{
    private readonly RuleEngineCascadeMode _cascadeMode;
    private readonly List<IRule<T>> _rules;

    public RuleEngine(RuleEngineCascadeMode cascadeMode)
    {
        _cascadeMode = cascadeMode;
        _rules = new List<IRule<T>>();
    }

    public void AddRule(IRule<T> rule)
    {
        if (rule == null) throw new ArgumentNullException(nameof(rule));
        _rules.Add(rule);
    }

    public void AddRules(params IRule<T>[] rules)
    {
        if (rules == null) throw new ArgumentNullException(nameof(rules));
        foreach (var rule in rules)
        {
            _rules.Add(rule);
        }
    }

    public void AddRules(IEnumerable<IRule<T>> rules)
    {
        if (rules == null) throw new ArgumentNullException(nameof(rules));
        foreach (var rule in rules)
        {
            _rules.Add(rule);
        }
    }

    public RuleEngineResult ApplyRules(T data)
    {
        var result = new RuleEngineResult
        {
            IsSuccessful = true
        };
        foreach (var rule in _rules)
        {
            var ruleResult = rule.Validate(data);
            var status = ruleResult.IsValid;

            if (!status && result.IsSuccessful)
            {
                result.IsSuccessful = false;
            }

            if (!status && _cascadeMode == RuleEngineCascadeMode.StopOnFirstFailure)
            {
                result.Errors.Add(new RuleEngineError
                {
                    Name = rule.Name,
                    Errors = ruleResult.Errors,
                });
                break;
            }
            if (status && _cascadeMode == RuleEngineCascadeMode.RunIfValid)
            {
                rule.Execute(data);
            }
            if (_cascadeMode == RuleEngineCascadeMode.RunAllPossible)
            {
                if (!status)
                {
                    result.Errors.Add(new RuleEngineError
                    {
                        Name = rule.Name,
                        Errors = ruleResult.Errors,
                    });
                }
                else
                {
                    rule.Execute(data);
                }
            }
        }
        return result;
    }
}