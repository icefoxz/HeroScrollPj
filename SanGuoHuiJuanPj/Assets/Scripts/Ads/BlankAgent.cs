using Assets.Scripts.Utl;
using UnityEngine.Events;

public class BlankAgent : AdAgent
{
    protected AdControllerBase controller;
    protected UnityAction<bool,string> callBackAction;

    public override void Init(AdControllerBase adController)
    {
        controller = adController;
        instance = this;
        gameObject.SetActive(false);
    }

    public override void BusyRetry(UnityAction<string> requestAction, UnityAction cancelAction)
    {
        CallAd((success,msg) =>
        {
            if (success) requestAction(msg);
            else cancelAction();
        });

    }

    public override void BusyRetry(UnityAction requestAction, UnityAction cancelAction)
    {
        CallAd((success,_) =>
        {
            if (success) requestAction();
            else cancelAction();
        });
    }

    public override void CallAd(UnityAction<bool,string> callBack)
    {
        gameObject.SetActive(true);
        callBackAction = callBack;
        if(controller.Status != States.Loaded)
            OnLoad();
        else OnShow();
    }

    protected void OnLoad()
    {
        UnityMainThread.thread.RunNextFrame(() => PlayerDataForGame.instance.ShowStringTips("请求广告中，请等待..."));
        controller.RequestLoad(OnLoadResponse);
    }

    private void OnLoadResponse(bool success, string msg)
    {
        if (success)
        {
            OnShow();
            return;
        }
        UnityMainThread.thread.RunNextFrame(() =>
        {
            gameObject.SetActive(false);
            PlayerDataForGame.instance.ShowStringTips("请求失败。");
        });
    }

    protected void OnShow()
    {
        UnityMainThread.thread.RunNextFrame(() => PlayerDataForGame.instance.ShowStringTips("请求成功，正加载..."));
        controller.RequestShow(OnShowResponse);
    }

    private void OnShowResponse(bool success, string msg)
    {
        UnityMainThread.thread.RunNextFrame(() =>
        {
            callBackAction(success, msg);
            var message = success ? "请求成功！" : $"请求失败:{msg}";
            PlayerDataForGame.instance.ShowStringTips(message);
            gameObject.SetActive(false);
        });
    }
}