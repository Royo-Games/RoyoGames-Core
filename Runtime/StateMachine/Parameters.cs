using System;
using System.Collections.Generic;


[Serializable]
public class Parameters 
{ 
    private Dictionary<string, object> _data = new();
    private readonly Dictionary<string, List<Delegate>> _listeners = new();
    private readonly Queue<PendingOp> _pendingOps = new();
    private bool _isNotifying = false;

    private readonly struct PendingOp
    {
        public readonly string Key;
        public readonly Delegate Callback;
        public readonly bool IsAdd;

        public PendingOp(string key, Delegate callback, bool isAdd)
        {
            Key = key;
            Callback = callback;
            IsAdd = isAdd;
        }
    }

    public void Add<T>(string key, T defaultValue)
    {
        if (_data.ContainsKey(key))
            throw new ArgumentException($"Parameter with key '{key}' already exists.", nameof(key));

        _data.Add(key, defaultValue!);
        InvokeListeners(key, defaultValue);
    }

    public bool Remove(string key)
    {
        _listeners.Remove(key);
        return _data.Remove(key);
    }

    public void Set<T>(string key, T value)
    {
        if (!_data.ContainsKey(key))
            throw new KeyNotFoundException($"Parameter key not found: {key}");

        _data[key] = value!;
        InvokeListeners(key, value);
    }

    public T Get<T>(string key)
    {
        if (_data.TryGetValue(key, out var obj) && obj is T t)
            return t;
        throw new KeyNotFoundException($"Parameter key not found or wrong type: {key}");
    }

    public bool Has(string key) => _data.ContainsKey(key);

    public void AddListener<T>(string key, Action<T> callback)
    {
        if (!_data.ContainsKey(key))
            throw new KeyNotFoundException($"Parameter key not found: {key}");

        if (_isNotifying)
        {
            _pendingOps.Enqueue(new PendingOp(key, callback, true));
        }
        else
        {
            if (!_listeners.TryGetValue(key, out var list))
                _listeners[key] = list = new List<Delegate>();
            list.Add(callback);
        }
    }

    public bool RemoveListener<T>(string key, Action<T> callback)
    {
        if (!_data.ContainsKey(key))
            return false;

        if (_isNotifying)
        {
            _pendingOps.Enqueue(new PendingOp(key, callback, false));
            return true;
        }
        else if (_listeners.TryGetValue(key, out var list))
        {
            bool removed = list.Remove(callback);
            if (list.Count == 0)
                _listeners.Remove(key);
            return removed;
        }
        return false;
    }

    private void InvokeListeners<T>(string key, T newValue)
    {
        if (!_listeners.TryGetValue(key, out var list))
            return;

        _isNotifying = true;

        try
        {
            foreach (var dlg in list)
            {
                if (dlg is Action<T> action)
                {
                    action(newValue);
                }
            }
        }
        finally
        {
            _isNotifying = false;

            while (_pendingOps.Count > 0)
            {
                var op = _pendingOps.Dequeue();
                if (op.IsAdd)
                {
                    if (!_listeners.TryGetValue(op.Key, out var cbList))
                        _listeners[op.Key] = cbList = new List<Delegate>();
                    cbList.Add(op.Callback);
                }
                else
                {
                    if (_listeners.TryGetValue(op.Key, out var cbList))
                    {
                        cbList.Remove(op.Callback);
                        if (cbList.Count == 0)
                            _listeners.Remove(op.Key);
                    }
                }
            }
        }
    }
}
