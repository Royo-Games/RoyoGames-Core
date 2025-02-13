using System;

public class CountTracker
{
    private Action<object[]> callback;
    public int RemainingCount { get; private set; }
    public bool IsSet { get; private set; }
    private object[] data;

    public void Set(int count, object[] data, Action<object[]> callback = null)
    {
        IsSet = true;
        RemainingCount = count > 0 ? count : 0;
        this.data = data;
        this.callback = callback;
    }
    public void Report(int count = 1)
    {
        if (RemainingCount <= 0) return;

        RemainingCount -= count;

        if (RemainingCount <= 0 && IsSet)
        {
            callback?.Invoke(data);
            callback = null;
            IsSet = false;
        }
    }
}