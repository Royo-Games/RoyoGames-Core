using System.Linq;

public class AndCondition : ICondition
{
    private readonly ICondition[] _conditions;
    public AndCondition(params ICondition[] conditions) => _conditions = conditions;
    public bool Evaluate(Parameters parameters) => _conditions.All(c => c.Evaluate(parameters));
}