using UnityEngine;
using UnityEngine.UI;

public class ZhanYiChestUi : TaoYuanChestUI
{
    public Color HighlightColor = new Color(170,0,0);
    public Color NormalColor = Color.black;
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
                com.color = HighlightColor;
                com.fontStyle = FontStyle.Bold;
            }
            else
            {
                com.color = NormalColor;
                com.fontStyle = FontStyle.Normal;
            }
        }
    }
}