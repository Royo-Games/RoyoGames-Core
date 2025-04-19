using System;
using System.Collections.Generic;

public class Parameters
{
    private readonly Dictionary<string, object> _data = new();

    public void Add<T>(string key, T defaultValue)
    {
        if (_data.ContainsKey(key))
            throw new ArgumentException($"Parameter with key '{key}' already exists.", nameof(key));

        _data.Add(key, defaultValue!);
    }

    public bool Remove(string key)
    {
        return _data.Remove(key);
    }

    public void Set<T>(string key, T value)
    {
        if (!_data.ContainsKey(key))
            throw new KeyNotFoundException($"Parameter key not found: {key}");

        _data[key] = value!;
    }

    public T Get<T>(string key)
    {
        if (_data.TryGetValue(key, out object obj) && obj is T t)
            return t;

        throw new KeyNotFoundException($"Parameter key not found or wrong type: {key}");
    }

    public bool Has(string key) => _data.ContainsKey(key);
}
