using UnityEngine;
using UnityEngine.UI;

public class ServerRequestExceptionWindow : MonoBehaviour
{
    [SerializeField]Text Title;
    [SerializeField]Text Detail;
    [SerializeField]Button CloseBtn;

    public void Init()
    {
        CloseBtn.onClick.RemoveAllListeners();
        CloseBtn.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }
    public void ShowError(string title, string detail)
    {
        Title.text = title;
        Detail.text = detail;
        gameObject.SetActive(true);
    }
}