using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Unity 控件状态映像表
/// </summary>
/// <typeparam name="TValue">控件</typeparam>
public class UiDisplayMapper<TValue> where TValue : Component
{
    public IReadOnlyDictionary<Enum, IReadOnlyList<TValue>> Data => _data.ToDictionary(v => v.Key, v => new List<TValue>(v.Value) as IReadOnlyList<TValue>);

    private List<TValue> List { get; set; }
    private UnityAction<bool,TValue> processAction;
    private readonly Dictionary<Enum, List<TValue>> _data;

    public UiDisplayMapper((Enum, TValue[])[] data, UnityAction<bool,TValue> action = null)
    {
        _data = data.ToDictionary(d => d.Item1, d => d.Item2.ToList());
        processAction = action ?? GameObjectShow;
        ResolveList();
    }

    public UiDisplayMapper(params (Enum, TValue[])[] data):this(data,null) { }

    public UiDisplayMapper(UnityAction<bool,TValue> action):this(new (Enum, TValue[])[0],action) { }

    public UiDisplayMapper():this(new (Enum, TValue[])[0],null) { }

    private static void GameObjectShow(bool result, TValue com) => com.gameObject.SetActive(result);

    public void Add(Enum key, params TValue[] values)
    {
        _data.Add(key,values.ToList());
        ResolveList();
    }

    private void ResolveList() => List = Data.SelectMany(i => i.Value).Distinct().ToList();

    public void Set(Enum key)
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