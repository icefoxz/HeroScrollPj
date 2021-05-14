using UnityEngine;
using UnityEngine.UI;

public class ZhanYiChestUi : TaoYuanChestUI
{
    public override void UpdateUi(string valueText, string max)
    {
        var isHighlight = int.Parse(valueText) >= int.Parse(max);
        ResolveText(value,isHighlight);
        ResolveText(lasting,isHighlight);
        base.UpdateUi(valueText, max);

        void ResolveText(Text com, bool highlight)
        {
            if (highlight)
            {
                com.color = Color.red;
                com.fontStyle = FontStyle.Bold;
            }
            else
            {
                com.color = Color.black;
                com.fontStyle = FontStyle.Normal;
            }
        }
    }
}