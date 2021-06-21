using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UiDisplayMapper<TKey,TValue> where TKey : Enum where TValue : Component
{
    public Dictionary<TKey,TValue[]> Data { get; private set; }
    private List<TValue> List { get; }

    public UiDisplayMapper(params (TKey, TValue[])[] data)
    {
        Data = data.ToDictionary(d => d.Item1, d => d.Item2);
        List = data.SelectMany(i => i.Item2).Distinct().ToList();
    }

    public void Set(TKey key)
    {
        if (!Data.ContainsKey(key))
        {
            List.ForEach(t=>t.gameObject.SetActive(false));
            return;
        }
        var block = Data[key];
        List.ForEach(t => t.gameObject.SetActive(block.Any(b => b == t)));
    }
}