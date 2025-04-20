using System;

public class ConditionEquals<T> : ICondition where T : IEquatable<T>
{
    private readonly string _key;
    private readonly T _targetValue;

    public ConditionEquals(string key, T targetValue)
    {
        _key = key;
        _targetValue = targetValue;
    }

    public bool Evaluate(Parameters parameters)
    {
        if (!parameters.Has(_key)) return false;
        return parameters.Get<T>(_key).Equals(_targetValue);
    }
}