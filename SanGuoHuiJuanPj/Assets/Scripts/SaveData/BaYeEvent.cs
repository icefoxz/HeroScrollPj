using System.Collections.Generic;

public class BaYeCityEvent
{
    public List<int> ExpList { get; set; } = new List<int>();
    public bool[] PassedStages { get; set; } = new bool[0];
    public int CityId { get; set; } = -1;
    public int WarId { get; set; } = -1;
    public int EventId { get; set; } = -1;
    public int ForceId { get; set; } = -1;
}

public class BaYeStoryEvent
{
    public int StoryId { get; set; }
    public int Type { get; set; }
    public int GoldReward { get; set; }
    public int ExpReward { get; set; }
}