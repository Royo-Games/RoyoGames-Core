using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    [Space]
    [SerializeField] protected UnityEvent<string> _onEvent;

    public virtual void InvokeEvent(string eventName)
    {
        _onEvent?.Invoke(eventName);
    }

    public virtual void AddListener(UnityAction<string> action)
    {
        _onEvent.AddListener(action);
    }

    public virtual void RemoveListener(UnityAction<string> action)
    {
        _onEvent.RemoveListener(action);
    }

    public virtual void RemoveAllListeners()
    {
        _onEvent.RemoveAllListeners();
    }
}