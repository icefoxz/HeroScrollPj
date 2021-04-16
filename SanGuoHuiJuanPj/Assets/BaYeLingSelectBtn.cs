using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaYeLingSelectBtn : MonoBehaviour
{
    public ForceFlagUI forceFlagUI;
    public Text text;
    public Button btn;

    public void Set(ForceFlags flag, int value)
    {
        forceFlagUI.Set(flag);
        text.text = $"+{value}";
    }
}
