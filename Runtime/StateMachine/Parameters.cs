using System;
using System.Collections.Generic;

public class Parameters : Parameters<string> { }

public class Parameters<TKey>
{
    private readonly Dictionary<TKey, object> _data = new();

    public void Add<T>(TKey key, T defaultValue)
    {
        if (_data.ContainsKey(key))
            throw new ArgumentException($"Parameter with key '{key}' already exists.", nameof(key));

        _data.Add(key, defaultValue!);
    }

    public bool Remove(TKey key)
    {
        return _data.Remove(key);
    }

    public void Set<T>(TKey key, T value)
    {
        if (!_data.ContainsKey(key))
            throw new KeyNotFoundException($"Parameter key not found: {key}");

        _data[key] = value!;
    }

    public T Get<T>(TKey key)
    {
        if (_data.TryGetValue(key, out object obj) && obj is T t)
            return t;

        throw new KeyNotFoundException($"Parameter key not found or wrong type: {key}");
    }

    public bool Has(TKey key) => _data.ContainsKey(key);
}
