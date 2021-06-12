using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ExpSlider : MonoBehaviour
{
    public Text DisplayText;
    public Slider Slider;

    public void Set(int exp, int maxExp)
    {
        Slider.maxValue = maxExp;
        Slider.value = exp;
        DisplayText.text = $"{exp}/{maxExp}";
    }
}