using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrayScale : MonoBehaviour
{
    [SerializeField] Material grayMaterial;
    [SerializeField] Color textGray;
    [SerializeField] Image[] images;
    [SerializeField] Text[] texts;
    private const string GrayScaleAmount = "_GrayscaleAmount";
    private Dictionary<Text, Color> oriColorMap;
    private Dictionary<Image, Material> oriMaterialMap;
    public void Init()
    {
        oriColorMap = new Dictionary<Text, Color>();
        oriMaterialMap = new Dictionary<Image, Material>();
        foreach (var image in images) oriMaterialMap.Add(image,image.material);
        foreach (var text in texts) oriColorMap.Add(text, text.color);
    }
    public void SetGray(float value)
    {
        //material.SetFloat(GrayScaleAmount, value);
        var isGray = Mathf.RoundToInt(value) == 1;
        foreach (var item in oriColorMap)
        {
            item.Key.color = isGray ? textGray : item.Value;
        }

        foreach (var item in oriMaterialMap)
        {
            item.Key.material = isGray ? grayMaterial : item.Value;
        }
    }
}