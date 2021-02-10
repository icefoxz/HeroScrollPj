using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// 权重元素接口，一旦声明这个接口，便可以在列表中根据权重随机选择一个元素
/// </summary>
public interface IWeightElement
{
    int Weight { get; }
}

public static class GameSystemExtension
{
    private const char Comma = ',';
    private const char Dash = '-';
    /// <summary>
    /// 根据权重随机获取一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Pick<T>(this IEnumerable<T> list) where T : IWeightElement
    {
        //随机开彩
        var ticketMap = list.PickMap();
        var draw = Random.Range(0, ticketMap.Count);
        return ticketMap[draw]; //返回选中索引
    }

    public static IEnumerable<T> Pick<T>(this IEnumerable<T> list, int pickAmt,bool repeatItem = false)
        where T : IWeightElement
    {
        var ticketMap = list.PickMap();
        var result = new List<T>();
        for (int i = 0; i < pickAmt; i++)
        {
            var drawElement = ticketMap[Random.Range(0, ticketMap.Count)];
            result.Add(drawElement);
            if (!repeatItem) 
                ticketMap = ticketMap.Where(t => !t.Equals(drawElement)).ToList();
        }
        return result;
    }

    private static List<T> PickMap<T>(this IEnumerable<T> list) where T : IWeightElement
    {
        var weightElements = list as T[] ?? list.ToArray();
        if (!weightElements.Any())
            throw new InvalidOperationException($"{nameof(GameSystemExtension)}.{nameof(Pick)}:权重元素为空！");
        //声明票根
        var ticket = 0;
        //声明抽奖桶
        var elements = new List<T>();
        //循环每个权重元素
        foreach (var element in weightElements)
        {
            //根据每个权重元素的权重数，给予票号
            for (var i = 0; i < element.Weight; i++) elements.Add(element);
        }

        return elements;
    }

    public static IEnumerable<int> TableStringToInts(this string text, char separator = Comma) => text.Split(separator)
        .Where(t => !string.IsNullOrWhiteSpace(t)).Select(int.Parse);

    public static IEnumerable<DateTime> TableStringToDates(this string text, char separator = Dash) => text.Split(separator)
        .Where(t => !string.IsNullOrWhiteSpace(t))
        .Select(s => DateTime.ParseExact(s, "yyyymmdd", CultureInfo.InvariantCulture));

    public static bool IsTableTimeInRange(this string dateText, DateTime checkDate,
        char separator = Dash)
    {
        var isNoTimeLimit = string.IsNullOrWhiteSpace(dateText);
        if (isNoTimeLimit) return true;
        var dates = dateText.TableStringToDates(separator).ToArray();
        var isTimeInRange = checkDate >= dates[0] && checkDate <= dates[1];
        return isTimeInRange;
    }

    public static IEnumerable<NowLevelAndHadChip> Enlist(this IEnumerable<NowLevelAndHadChip> cards,int forceId)=> cards.Where(card =>
    {
        var force = int.Parse(LoadJsonFile.heroTableDatas[card.id][6]);
        return force == forceId && (card.level > 0 || card.chips > 0) && card.isFight > 0;
    });
}
