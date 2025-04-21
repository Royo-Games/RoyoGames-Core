public class NumericCondition : ICondition
{
    private readonly string _key;
    private readonly float _threshold;
    private readonly ComparisonType _comparison;

    public NumericCondition(string key, float threshold, ComparisonType comparison)
    {
        _key = key;
        _threshold = threshold;
        _comparison = comparison;
    }

    public bool Evaluate(Parameters parameters)
    {
        if (!parameters.Has(_key)) return false;
        float value = parameters.Get<float>(_key);
        return _comparison switch
        {
            ComparisonType.Less => value < _threshold,
            ComparisonType.LessOrEqual => value <= _threshold,
            ComparisonType.Greater => value > _threshold,
            ComparisonType.GreaterOrEqual => value >= _threshold,
            _ => false
        };
    }
}
