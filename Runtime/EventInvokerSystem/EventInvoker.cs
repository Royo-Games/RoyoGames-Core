using System.Collections.Generic;
using UnityEngine;

public class EventInvoker : MonoBehaviour
{
#if UNITY_EDITOR
    [Space]
    [SerializeField] protected bool _showGizmos;
    [SerializeField] protected Color _gizmoColor = Color.yellow;
#endif

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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_eventListeners == null || !_showGizmos)
            return;

        Gizmos.color = _gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.5f);

        GizmosUtility.Drawlines(transform.position, _eventListeners);
    }
#endif
}
