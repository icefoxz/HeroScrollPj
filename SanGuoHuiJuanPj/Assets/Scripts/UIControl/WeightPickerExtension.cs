using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

/// <summary>
/// 权重元素接口，一旦声明这个接口，便可以在列表中根据权重随机选择一个元素
/// </summary>
public interface IWeightElement
{
    int Weight { get; }
}

public static class WeightPickerExtension
{
    /// <summary>
    /// 根据权重随机获取一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Pick<T>(this IEnumerable<T> list) where T : IWeightElement
    {
        var weightElements = list as T[] ?? list.ToArray();
        if (!weightElements.Any())
            throw new InvalidOperationException($"{nameof(WeightPickerExtension)}.{nameof(Pick)}:权重元素为空！");
        //获取总权重数
        var total = weightElements.Sum(e => e.Weight);
        //声明票根
        var ticket = 0;
        //声明抽奖桶
        var ticketMap = new Dictionary<int, T>();
        //循环每个权重元素
        foreach (var element in weightElements)
        {
            //根据每个权重元素的权重数，给予票号
            for (var i = 0; i < element.Weight; i++)
            {
                ticketMap.Add(ticket, element);
                ticket++;
            }
        }

        if (ticketMap.Any(t => t.Key >= total || t.Key < 0)) //测试，如果有票根超过范围就会报错！
            throw new InvalidOperationException(
                $"{nameof(WeightPickerExtension)}.{nameof(Pick)}:权重计算错误！总: {total},最后票号:{ticket},权重元素数:{weightElements.Length}！");
        //随机开彩
        var draw = Random.Range(0, total);
        return ticketMap[draw]; //返回选中索引
    }
}
