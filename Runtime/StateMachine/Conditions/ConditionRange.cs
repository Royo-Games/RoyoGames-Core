using System;

public class ConditionRange : ICondition
{
    private readonly string _key;
    private readonly float _min, _max;

    public ConditionRange(string key, float min, float max)
    {
        if (min > max)
            throw new ArgumentException($"min ({min}) cannot be greater than max ({max}).");
        _key = key;
        _min = min;
        _max = max;
    }

    public bool Evaluate(Parameters p)
    {
        if (!p.Has(_key))
            return false;

        float value = p.Get<float>(_key);
        return value >= _min && value <= _max;
    }
}
