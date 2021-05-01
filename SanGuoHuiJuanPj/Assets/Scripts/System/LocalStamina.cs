using System;
using CorrelateLib;

public class LocalStamina
{
    public long Seed { get; private set; }
    public int Value => DefaultValue + DynamicValue;
    public int DynamicValue { get; private set; }
    public TimeSpan Countdown { get; private set; }
    public int SecsPerStamina { get; }
    public int DefaultValue { get; }
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
    }

    public void AddStamina(int value)
    {
        DynamicValue += value;
        Resolve();
    }

    private void Resolve()
    {
        Seed = SysTime.UnixNow;
        if (Value <= MaxValue) return;
        DynamicValue = MaxValue - DefaultValue;
    }

    public void UpdateStamina(long timeTicks)
    {
        if (IsStopIncrease)
        {
            Seed = timeTicks;
            return;
        }
        var elapsed = TimeSpan.FromMilliseconds(timeTicks - Seed);
        DynamicValue = (int)(elapsed.TotalSeconds / SecsPerStamina);
        var remainder = 0;
        if (elapsed.TotalSeconds >= 1)
            remainder = (int)elapsed.TotalSeconds % SecsPerStamina;
        Countdown = TimeSpan.FromSeconds(SecsPerStamina - remainder);
    }
}
