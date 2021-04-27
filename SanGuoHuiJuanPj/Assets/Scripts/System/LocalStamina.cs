using System;

public class LocalStamina
{
    public long Seed { get; }
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
        Seed = seed;
        DefaultValue = defaultValue;
        SecsPerStamina = secsPerStamina;
        IncreaseLimit = increaseLimit;
        MaxValue = maxValue;
    }

    public void AddStamina(int value)
    {
        DynamicValue += value;
        ResolveLimit();
    }

    private void ResolveLimit()
    {
        if (Value <= MaxValue) return;
        DynamicValue = MaxValue - DefaultValue;
    }

    public void UpdateStamina(long timeTicks)
    {
        if (IsStopIncrease) return;
        var elapsed = TimeSpan.FromMilliseconds(timeTicks - Seed);
        DynamicValue = (int)(elapsed.TotalSeconds / SecsPerStamina);
        Countdown = TimeSpan.FromSeconds(elapsed.TotalSeconds % SecsPerStamina);
    }
}