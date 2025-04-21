using System;

public class PredicateCondition : ICondition
{
    private readonly Func<Parameters, bool> predicate;
    public PredicateCondition(Func<Parameters, bool> predicate)
    {
        this.predicate = predicate;
    }

    public bool Evaluate(Parameters parameters)
        => predicate(parameters);
}