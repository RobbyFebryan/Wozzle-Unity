using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class EventTriggerListener : EventTrigger
{
    public Action<GameObject> onEnter;
    public Action<GameObject> onExit;

    public static EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null)
            listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke(gameObject);
    }
}
