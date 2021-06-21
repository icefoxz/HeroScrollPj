using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SanXuanWindowUi : MonoBehaviour
{
    public TradableGameCardUi[] GameCards;
    public Image Title;
    public Button RefreshBtn;
    public Text RefreshText;
    public Button AdRefreshBtn;
    public GameObject InfoWindow;
    public Text InfoText;

    public void ShowInfo(string text)
    {
        InfoText.text = text;
        InfoWindow.SetActive(true);
        InfoText.DOFade(0, 0f).OnComplete(() => InfoText.DOFade(1, 0.5f));
    }

    public void CloseInfo() => InfoWindow.SetActive(false);

    public void SetTrade(int refreshCost)
    {
        Title.sprite = GameResources.Instance.GuanQiaEventImg[6];
        DisplayTradeSet(true);
        RefreshText.text = refreshCost.ToString();
    }

    private void DisplayTradeSet(bool isTrade)
    {
        RefreshBtn.gameObject.SetActive(isTrade);
        AdRefreshBtn.gameObject.SetActive(!isTrade);
    }


    public void SetRecruit()
    {
        Title.sprite = GameResources.Instance.GuanQiaEventImg[5];
        DisplayTradeSet(false);
    }

    public void ResetUi() => gameObject.SetActive(false);
}