using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventTriggerListener : EventTrigger
{
    public UnityAction<GameObject> onClick;
    public UnityAction<GameObject> onDown;
    public UnityAction<GameObject> onEnter;
    public UnityAction<GameObject> onExit;
    public UnityAction<GameObject> onUp;
    public UnityAction<GameObject> onSelect;
    public UnityAction<GameObject> onUpdateSelect;

    public static EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public static EventTriggerListener Get(Transform transform)
    {
        EventTriggerListener listener = transform.GetComponent<EventTriggerListener>();
        if (listener == null) listener = transform.gameObject.AddComponent<EventTriggerListener>();
        return listener;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(gameObject);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke(gameObject);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke(gameObject);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke(gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke(gameObject);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        onSelect?.Invoke(gameObject);
    }
    public override void OnUpdateSelected(BaseEventData eventData)
    {
        onUpdateSelect?.Invoke(gameObject);
    }
}