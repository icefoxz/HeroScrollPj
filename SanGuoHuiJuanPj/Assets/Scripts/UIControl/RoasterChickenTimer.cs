using System;
using System.Collections.Generic;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;

public class RoasterChickenTrigger
{
    /***
     * 1. time-based trigger player interaction in a specified time
     * 2. interaction allows once for each range of time
     * 3. daily refresh the availability.
     */

    //鸡坛开启时间点 
    private int[][] openTimeRanges ={
            new[] {12, 14},
            new[] {19, 21}
            //new[] {12, 14},
            //new[] {19, 21}   
        };

    private DailyFlagsMapper mapper;
    public event UnityAction<int[]> OnRoasterChickenTrigger;

    public RoasterChickenTrigger()
    {
        mapper = Json.Deserialize<DailyFlagsMapper>(GamePref.RoasterChickenRecord()) ?? new DailyFlagsMapper();
    }

    public void UpdateTimeNow()
    {
        var index = IndexInRange();//get the first index of the list which matched in time range. -1 for none of matched
        if (index < 0) return;
        if (mapper.IsTodayTaken(index)) return;
        OnRoasterChickenTrigger?.Invoke(openTimeRanges[index]);
    }

    /// <summary>
    /// Return the first of the time in sequence if the hour of the day(now) is in range, -1 will given if all of the time doesn't match
    /// </summary>
    /// <returns></returns>
    private int IndexInRange()
    {
        var nowClock = SysTime.Now.Hour;
        return Array.FindIndex(openTimeRanges, range => range[0] <= nowClock && range[1] > nowClock);
    }

    public void FlagNow()
    {
        var index = IndexInRange();
        if (index <= 0) return;
        mapper.FlagTaken(index);
        GamePref.SetRoasterChicken(Json.Serialize(mapper));
    }

    /// <summary>
    /// daily actions flag mapper,
    /// flag whether is action taken or not,
    /// will reset flag records when the day passed
    /// </summary>
    private class DailyFlagsMapper
    {
        public IReadOnlyDictionary<int, bool> TimeMap => timeMap;
        private Dictionary<int, bool> timeMap { get; set; } = new Dictionary<int, bool>();
        public long LastTicks { get; set; }

        public bool IsTodayTaken(int index)
        {
            CheckAndResetWhenRecordedDayPass();
            return timeMap.ContainsKey(index) && timeMap[index];
        }

        private void CheckAndResetWhenRecordedDayPass()
        {
            if (!SysTime.IsToday(SysTime.UnixNow)) ResetMap();

            void ResetMap()
            {
                LastTicks = SysTime.ToUtcUnixTicks(SysTime.Now.Date);
                timeMap = new Dictionary<int, bool>();
            }
        }

        public void FlagTaken(int index)
        {
            LastTicks = SysTime.UnixNow;
            if (!timeMap.ContainsKey(index))
            {
                timeMap.Add(index, true);
            }
            timeMap[index] = true;
            CheckAndResetWhenRecordedDayPass();
        }
    }
}