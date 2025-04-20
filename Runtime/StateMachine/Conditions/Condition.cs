using System;

public class Condition : ICondition
{
    private readonly Func<Parameters, bool> predicate;
    public Condition(Func<Parameters, bool> predicate)
    {
        this.predicate = predicate;
    }

    public bool Evaluate(Parameters parameters)
        => predicate(parameters);
}