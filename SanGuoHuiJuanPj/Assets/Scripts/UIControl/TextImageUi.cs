using UnityEngine;
using UnityEngine.UI;

public class TextImageUi :MonoBehaviour
{
    public Text Txt;
    public Image Img;

    public void Set(string text, Sprite image)
    {
        Txt.text = text;
        Img.sprite = image;
        Show();
    }

    public void Show()=>gameObject.SetActive(true);

    public void Off() => gameObject.SetActive(false);
}