using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unity 控件状态映像表
/// </summary>
/// <typeparam name="TValue">控件</typeparam>
public class UiStateMapper<TValue> : UiDisplayMapper<Enum,TValue> where TValue : Component
{
    public UiStateMapper((Enum, TValue[])[] data, UnityAction<bool, TValue> action = null):base(data, action) {}
    public UiStateMapper(params (Enum, TValue[])[] data) : this(data, null) { }
    public UiStateMapper(UnityAction<bool, TValue> action) : this(new (Enum, TValue[])[0], action) { }
    public UiStateMapper() : this(new (Enum, TValue[])[0], null) { }
}

public class UiDisplayMapper<TKey,TValue> where TValue : Component
{
    public IReadOnlyDictionary<TKey, IReadOnlyList<TValue>> Data => _data.ToDictionary(v => v.Key, v => new List<TValue>(v.Value) as IReadOnlyList<TValue>);

    private List<TValue> List { get; set; }
    private UnityAction<bool, TValue> processAction;
    private readonly Dictionary<TKey, List<TValue>> _data;

    public UiDisplayMapper((TKey, TValue[])[] data, UnityAction<bool, TValue> action = null)
    {
        _data = data.ToDictionary(d => d.Item1, d => d.Item2.ToList());
        processAction = action ?? GameObjectShow;
        ResolveList();
    }

    public UiDisplayMapper(params (TKey, TValue[])[] data) : this(data, null) { }

    public UiDisplayMapper(UnityAction<bool, TValue> action) : this(new (TKey, TValue[])[0], action) { }

    public UiDisplayMapper() : this(new (TKey, TValue[])[0], null) { }

    private static void GameObjectShow(bool result, TValue com) => com.gameObject.SetActive(result);

    public void Add(TKey key, params TValue[] values)
    {
        _data.Add(key, values.ToList());
        ResolveList();
    }

    private void ResolveList() => List = Data.SelectMany(i => i.Value).Distinct().ToList();

    /// <summary>
    /// Invoke the preset method (positive/negative) separated by key
    /// </summary>
    /// <param name="key"></param>
    public void Set(TKey key)
    {
        if (!Data.ContainsKey(key))
        {
            List.ForEach(t => processAction.Invoke(false, t));//t=>t.gameObject.SetActive(false));
            return;
        }
        var block = Data[key];
        List.ForEach(t => processAction.Invoke(block.Any(b => b == t), t)); //t.gameObject.SetActive(block.Any(b => b == t)));
    }
}