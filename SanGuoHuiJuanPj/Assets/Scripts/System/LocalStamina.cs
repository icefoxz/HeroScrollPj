using System;
using CorrelateLib;

public class LocalStamina
{
    public long Seed { get; private set; }
    public int Value => DefaultValue + UpdateValue;
    public int UpdateValue { get; private set; }
    public TimeSpan Countdown { get; private set; }
    public int SecsPerStamina { get; }
    public int DefaultValue { get; private set; }
    public int IncreaseLimit { get; }
    public int MaxValue { get; }
    public bool IsStopIncrease => Value >= IncreaseLimit;

    public LocalStamina(long seed,int defaultValue, int secsPerStamina, int increaseLimit, int maxValue)
    {
        if (seed == default) seed = SysTime.UnixNow;
        Seed = seed;
        DefaultValue = defaultValue;
        SecsPerStamina = secsPerStamina;
        IncreaseLimit = increaseLimit;
        MaxValue = maxValue;
        UpdateStamina();
    }

    public void AddStamina(int value) => SetValue(Value + value);

    //预防体力大于
    private void SetValue(int value)
    {
        DefaultValue = value;
        UpdateValue = 0;
        if (Value < IncreaseLimit) return;//如果体力小于自增上限，无需处理(体力还继续更新)
        if (Value > MaxValue)//如果体力大于极限，设极限值。
            DefaultValue = MaxValue;
        Seed = SysTime.UnixNow;//记录当前数据修改时间
    }

    public void UpdateStamina()
    {
        if (IsStopIncrease) return;
        var elapsed = TimeSpan.FromMilliseconds(SysTime.UnixNow - Seed);
        UpdateValue = (int)(elapsed.TotalSeconds / SecsPerStamina);
        var secondsRemain= 0;
        if (elapsed.TotalSeconds >= 1)
            secondsRemain = (int)elapsed.TotalSeconds % SecsPerStamina;
        Countdown = TimeSpan.FromSeconds(SecsPerStamina - secondsRemain);
    }
}
