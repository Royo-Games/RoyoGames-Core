using System.Linq;

public class ConditionOr : ICondition
{
    private readonly ICondition[] _conditions;
    public ConditionOr(params ICondition[] conditions) => _conditions = conditions;
    public bool Evaluate(Parameters parameters) => _conditions.Any(c => c.Evaluate(parameters));
}
