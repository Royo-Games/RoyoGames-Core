using System.Collections.Generic;
using UnityEngine;

public class EventInvoker : MonoBehaviour
{
    [Space]
    [SerializeField] protected List<EventListener> _eventListeners;

    public virtual void InvokeEvent(string value)
    {
        foreach (EventListener eventListener in _eventListeners)
        {
            if (eventListener != null)
                eventListener.InvokeEvent(value);
        }
    }

    public virtual void AddListener(EventListener eventListener)
    {
        _eventListeners.Add(eventListener);
    }

    public virtual void RemoveListener(EventListener eventListener)
    {
        _eventListeners.Remove(eventListener);
    }

    public virtual void RemoveAllListeners()
    {
        _eventListeners.Clear();
    }
}
