using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationWindowUi : MonoBehaviour
{
    public enum Resources
    {
        None,
        YuQue,
        YuanBao
    }
    public Button ConfirmButton;
    public Button CancelButton;
    public Image YuQueImage;
    public Image YuanBaoImage;
    public Text Value;
    private UiDisplayMapper<Component> mapper;

    public void Init()
    {
        mapper = new UiDisplayMapper<Component>(
            (Resources.YuQue, new Component[] {YuQueImage, Value}),
            (Resources.YuanBao, new Component[] {YuanBaoImage, Value})
        );
        CancelButton.onClick.AddListener(Cancel);
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        ConfirmButton.onClick.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    public void Show(UnityAction onConfirmAction,Resources resource,int cost = 0)
    {
        ConfirmButton.onClick.RemoveAllListeners();
        var message = string.Empty;
        switch (resource)
        {
            case Resources.None:
                break;
            case Resources.YuQue:
                if (PlayerDataForGame.instance.pyData.YvQue < cost)
                    message = DataTable.GetStringText(2); //玉阙不足
                break;
            case Resources.YuanBao:
                if (PlayerDataForGame.instance.pyData.YuanBao < cost)
                    message = DataTable.GetStringText(0); //元宝不足
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resource), resource, null);
        }

        ConfirmButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrWhiteSpace(message)) onConfirmAction();
            else PlayerDataForGame.instance.ShowStringTips(message);
        });
        mapper.Set(resource);
        Value.text = cost == 0 ? string.Empty : cost.ToString();
        gameObject.SetActive(true);
    }
}