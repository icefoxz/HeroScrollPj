using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Switch every <see cref="TValue"/>(s) in the condition of matching from the <see cref="TKey"/> mapped,
/// typically <see cref="TValue"/>(s) will change form whether the <see cref="TKey"/> is matched
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public abstract class SwitchMapperBase<TKey, TValue>
{
    protected UnityAction<bool, TValue> switchOperation;
    protected Dictionary<TKey, List<TValue>> _data;
    public IReadOnlyDictionary<TKey, IReadOnlyList<TValue>> Data => _data.ToDictionary(v => v.Key, v => new List<TValue>(v.Value) as IReadOnlyList<TValue>);
    private List<TValue> List { get; set; }

    protected SwitchMapperBase(params (TKey, TValue[])[] data) : this(data, null) { }
    protected SwitchMapperBase((TKey, TValue[])[] data, UnityAction<bool, TValue> action = null)
    {
        _data = data != null
            ? data.ToDictionary(d => d.Item1, d => d.Item2.ToList())
            : new Dictionary<TKey, List<TValue>>();
        switchOperation = action ?? DefaultOperation;
        ResolveList();
    }

    protected abstract void DefaultOperation(bool isCorrectKey, TValue operation);


    public void Add(TKey key, params TValue[] values)
    {
        _data.Add(key, values.ToList());
        ResolveList();
    }

    private void ResolveList() => List = Data.SelectMany(i => i.Value).Distinct().ToList();

    /// <summary>
    /// Invoke the preset method to change <see cref="TValue"/>(s) into active/inactive form according to the present <see cref="TKey"/>.
    /// Note that if there are duplicated <see cref="TValue"/> in both active or inactive <see cref="TKey"/>, the inactive method will not called.
    /// </summary>
    /// <param name="key"></param>
    public void Set(TKey key)
    {
        if (!Data.ContainsKey(key))
        {
            List.ForEach(t => switchOperation?.Invoke(false, t));
            return;
        }
        var activeList = Data[key];
        var inActiveList = List.Except(activeList);
        foreach (var value in inActiveList) switchOperation?.Invoke(false, value);
        foreach (var value in activeList) switchOperation?.Invoke(true, value);
    }
}

public class GameObjectSwitch<TKey> : SwitchMapperBase<TKey, GameObject> 
{
    public GameObjectSwitch((TKey, GameObject[])[] data, UnityAction<bool, GameObject> defaultAction) : base(data, defaultAction)
    {

    }
    public GameObjectSwitch((TKey, GameObject[])[] data) : base(data)
    {

    }

    public GameObjectSwitch(UnityAction<bool, GameObject> defaultAction):base(null,defaultAction)
    {
    }

    protected override void DefaultOperation(bool isCorrectKey, GameObject obj) => obj.SetActive(isCorrectKey);
}

/// <summary>
/// action for every component is gameObject.SetActive(isActiveEnum), if (custom) method is not set in constructor
/// </summary>
/// <typeparam name="TKey">Specific Key</typeparam>
/// <typeparam name="TValue">Specific Component</typeparam>
public class ComponentStateSwitch<TKey,TValue> : SwitchMapperBase<TKey, TValue> where TValue : Component
{
    public ComponentStateSwitch((TKey, TValue[])[] data, UnityAction<bool, TValue> action = null) : base(data, action){}

    public ComponentStateSwitch() : base(new (TKey, TValue[])[0], null) { }
    
    protected override void DefaultOperation(bool isCorrectValue, TValue com) => com.gameObject.SetActive(isCorrectValue);
}

/// <summary>
/// action for every component is gameObject.SetActive(isActiveEnum), if (custom) method is not set in constructor
/// </summary>
public class ComponentActivateSwitch: ComponentStateSwitch<Enum, Component>
{
    public ComponentActivateSwitch((Enum, Component[])[] data, UnityAction<bool, Component> action) : base(data, action){}
    public ComponentActivateSwitch(params (Enum, Component[])[] data) : this(data, null){}

    public ComponentActivateSwitch() : base(new (Enum, Component[])[0], null) { }
    
    protected override void DefaultOperation(bool isActiveEnum, Component com) => com.gameObject.SetActive(isActiveEnum);
}

/// <summary>
/// action for every component is gameObject.SetActive(isActiveEnum), if (custom) method is not set in constructor
/// </summary>
/// <typeparam name="T">Specific component</typeparam>
public class ComponentActivateSwitch<T>: ComponentStateSwitch<Enum, T> where T:Component
{
    public ComponentActivateSwitch((Enum, T[])[] data, UnityAction<bool, T> action) : base(data, action){}
    public ComponentActivateSwitch(params (Enum, T[])[] data) : this(data, null){}

    public ComponentActivateSwitch(UnityAction<bool, T> action) : base(null, action){}
    public ComponentActivateSwitch() : base(new (Enum, T[])[0], null) { }
    
    protected override void DefaultOperation(bool isActiveEnum, T com) => com.gameObject.SetActive(isActiveEnum);
}