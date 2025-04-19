using System.Linq;

public class OrCondition : ICondition
{
    private readonly ICondition[] _conditions;
    public OrCondition(params ICondition[] conditions) => _conditions = conditions;
    public bool Evaluate(Parameters parameters) => _conditions.Any(c => c.Evaluate(parameters));
}
