using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SanXuanWindowUi : MonoBehaviour
{
    public TradableGameCardUi[] GameCards;
    public Image Title;
    public Button RefreshBtn;
    public Text RefreshText;
    public AdConsumeController AdConsume;
    //public Button AdRefreshBtn;
    public GameObject InfoWindow;
    public Text InfoText;

    public void Init()
    {
        AdConsume.Init();
        gameObject.SetActive(false);
    }

    public void Show(bool isTrade)
    {
        SetDisplayAdConsume(isTrade);
        gameObject.SetActive(true);
    }

    public void Off() => gameObject.SetActive(false);

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
        SetDisplayAdConsume(isTrade);
        //AdRefreshBtn.gameObject.SetActive(!isTrade);
    }

    private void SetDisplayAdConsume(bool isTrade)
    {
        if(!isTrade) AdConsume.ShowWithUpdate();
        else AdConsume.Off();
    }

    public void SetRecruit()
    {
        Title.sprite = GameResources.Instance.GuanQiaEventImg[5];
        DisplayTradeSet(false);
    }

    public void ResetUi() => gameObject.SetActive(false);
}
