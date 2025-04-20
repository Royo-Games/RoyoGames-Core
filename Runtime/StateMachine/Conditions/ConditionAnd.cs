using System.Linq;

public class ConditionAnd : ICondition
{
    private readonly ICondition[] _conditions;
    public ConditionAnd(params ICondition[] conditions) => _conditions = conditions;
    public bool Evaluate(Parameters parameters) => _conditions.All(c => c.Evaluate(parameters));
}