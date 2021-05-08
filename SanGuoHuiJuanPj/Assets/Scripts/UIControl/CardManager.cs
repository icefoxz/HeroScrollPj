using System.Collections.Generic;
using System.Linq;
using Beebyte.Obfuscator;

[Skip]
public class CardManager
{
    /// <summary>
    /// 重置羁绊映像
    /// </summary>
    /// <param name="jiBanMap"></param>
    public static void ResetJiBan(Dictionary<int, JiBanActivedClass> jiBanMap)
    {
        jiBanMap.Clear();
        foreach (var jiBan in DataTable.JiBan.Values)
        {
            if (jiBan.IsOpen == 0) continue;
            JiBanActivedClass jiBanActivedClass = new JiBanActivedClass();
            jiBanActivedClass.jiBanId = jiBan.Id;
            jiBanActivedClass.isActived = false;
            jiBanActivedClass.cardTypeLists = new List<JiBanCardTypeClass>();
            jiBanActivedClass.isHadBossId = jiBan.BossCards.Length > 0;

            for (int i = 0; i < jiBan.Cards.Length; i++)
            {
                var card = jiBan.Cards[i];
                jiBanActivedClass.cardTypeLists.Add(new JiBanCardTypeClass
                {
                    cardId = card.CardId,
                    cardType = card.CardType,
                    cardLists = new List<FightCardData>(),
                    bossId = jiBan.BossCards.Length == 0 ? 0 : jiBan.BossCards[i].CardId
                });
            }
            jiBanMap.Add(jiBan.Id, jiBanActivedClass);
        }
    }
}