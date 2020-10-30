using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerListener : EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public delegate void BoolDelegate(GameObject go, bool state);
    public delegate void FloatDelegate(GameObject go, float delta);
    public delegate void VectorDelegate(GameObject go, Vector2 delta);
    public delegate void ObjectDelegate(GameObject go, GameObject obj);
    public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    static public EventTriggerListener Get(Transform transform)
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

    private void TakeEventFun()
    {
        EventTriggerListener.Get(gameObject).onDown += (go) => {
            //Debug.Log("按下！");
        };
        EventTriggerListener.Get(gameObject).onUp += (go) => {
            //Debug.Log("抬起！");
        };
        EventTriggerListener.Get(gameObject).onSelect += (go) => {
            //Debug.Log("选中！");
        };
        EventTriggerListener.Get(gameObject).onEnter += (go) => {
            //Debug.Log("进入！");
        };
        EventTriggerListener.Get(gameObject).onExit += (go) => {
            //Debug.Log("退出！");
        };
    }
    //public override void OnPointerClick(PointerEventData eventData)
    //{
    //    if (onClick != null) onClick(gameObject);
    //}
    //public override void OnPointerDown(PointerEventData eventData)
    //{
    //    if (onDown != null) onDown(gameObject);
    //}
    //public override void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (onEnter != null) onEnter(gameObject);
    //}
    //public override void OnPointerExit(PointerEventData eventData)
    //{
    //    if (onExit != null) onExit(gameObject);
    //}
    //public override void OnPointerUp(PointerEventData eventData)
    //{
    //    if (onUp != null) onUp(gameObject);
    //}
    //public override void OnSelect(BaseEventData eventData)
    //{
    //    if (onSelect != null) onSelect(gameObject);
    //}
    //public override void OnUpdateSelected(BaseEventData eventData)
    //{
    //    if (onUpdateSelect != null) onUpdateSelect(gameObject);
    //}
}