using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CorrelateLib;
using UnityEngine;
using UnityEngine.UI;

public class SanGuoTvUi : MonoBehaviour
{
    public Text[] contents;
    public Text[] clocks;
    public int Secs = 30;
    
    private void Start()
    {
        GenerateReport();
        StartCoroutine(CountdownTimer());
    }

    private IEnumerator CountdownTimer()
    {
        while (true)
        {
            GenerateReport();
            yield return new WaitForSeconds(Secs);
        }
    }

    public void GenerateReport()
    {
        var now = DateTime.Now;
        var npcs = DataTable.BaYeTv.Values
            .Select(obj => new Speech
            {
                Id = obj.Id,
                Weight = obj.Weight, //权重
                Text = obj.Text, //文本
                Time = obj.Time, //时间
                Format = obj.Format //格式
            }).Where(r => r.Time.IsTableTimeInRange(now))
            .Pick(contents.Length).ToList();

        for (int i = 0; i < contents.Length; i++)
        {
            contents[i].text = GameSystem.MapService.GetCharacterInRandom(50, out var cha)
                ? $"{cha.Name} : {cha.Sign}"
                : GetFormattedText(npcs[i]);
            clocks[i].text = SystemTimer.instance.CurrentClock;
        }
    }

    private string GetFormattedText(Speech speech)
    {
        object[] names = DataTable.BaYeName.Values.Select(obj => new Character
        {
            Name = obj.Name,
            Weight = obj.Weight
        }).Pick(speech.Format).Select(r => r.Name).ToArray();
        return string.Format(speech.Text, names);
    }

    private class Speech : IWeightElement
    {
        public int Id{ get; set; }
        public int Weight { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public int Format { get; set; }
    }

    private class Character : IWeightElement
    {
        public string Name { get; set; }
        public int Weight { get; set; }
    }
}
