using System.Collections.Generic;
using System.Linq;

public class CardManager
{
    /// <summary>
    /// 重置羁绊映像
    /// </summary>
    /// <param name="jiBanMap"></param>
    public static void ResetJiBan(Dictionary<int, JiBanActivedClass> jiBanMap)
    {
        jiBanMap.Clear();
        foreach (var map in DataTable.JiBan)
        {
            var enableValue = int.Parse(map.Value[2]);
            if (enableValue == 0) continue;
            JiBanActivedClass jiBanActivedClass = new JiBanActivedClass();
            jiBanActivedClass.jiBanIndex = map.Key;
            jiBanActivedClass.isActived = false;
            jiBanActivedClass.cardTypeLists = new List<JiBanCardTypeClass>();
            jiBanActivedClass.isHadBossId = !string.IsNullOrWhiteSpace(map.Value[5]);
            var bossIds = new List<int[]>();
            if(jiBanActivedClass.isHadBossId) bossIds = map.Value[5].Split(';').Select(s=>s.TableStringToInts().ToArray()).ToList();

            var cards = map.Value[3].Split(';').Where(v=>!string.IsNullOrWhiteSpace(v))
                .Select(s=>s.TableStringToInts().ToArray())
                .ToArray();

            for (int i = 0; i < cards.Length; i++)
            {
                var card = cards[i];
                jiBanActivedClass.cardTypeLists.Add(new JiBanCardTypeClass
                {
                    cardId = card[1],
                    cardType = card[0],
                    cardLists = new List<FightCardData>(),
                    bossId = bossIds.Count == 0 ? 0 : bossIds[i][1]
                });
            }
            jiBanMap.Add(map.Key, jiBanActivedClass);
        }
    }
}