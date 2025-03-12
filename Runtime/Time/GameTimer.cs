using UnityEngine;

public class GameTimer
{
    private float startTime;
    private bool isStart;
    private float pausedTime;
    public bool IsUnscaledTime { get; set; }

    private float CurrentTime
    {
        get
        {
            if (IsUnscaledTime)
                return Time.unscaledTime;
            else
                return Time.time;
        }
    }
    public bool IsStart
    {
        get
        {
            return isStart;
        }
    }

    public GameTimer(bool isUnscaledTime = false)
    {
        IsUnscaledTime = isUnscaledTime;
    }

    public float RemainingTime(float delay)
    {
        return Mathf.Clamp(startTime + delay - CurrentTime, 0, float.MaxValue);
    }
    public float ElapsedTime(float delay)
    {
        return Mathf.Clamp(CurrentTime - startTime, 0, delay);
    }
    public float Progress(float delay)
    {
        return Mathf.Clamp01(ElapsedTime(delay) / delay);
    }
    public float RemainingProgress(float delay)
    {
        return 1.0f - Progress(delay);
    }
    public void Start()
    {
        startTime = CurrentTime;
        isStart = true;
    }
    public void AddStartTime(float time)
    {
        startTime += time;
    }
    public void Stop()
    {
        isStart = false;
    }
    public void Pause()
    {
        if (isStart)
        {
            pausedTime = CurrentTime;
            isStart = false;
        }
    }
    public void Continue()
    {
        if (!isStart)
        {
            float pauseDuration = CurrentTime - pausedTime;
            startTime += pauseDuration;
            isStart = true;
        }
    }
    public bool IsTimeOut(float delay)
    {
        if (!isStart)
            return false;

        return startTime + delay <= CurrentTime;
    }
}
