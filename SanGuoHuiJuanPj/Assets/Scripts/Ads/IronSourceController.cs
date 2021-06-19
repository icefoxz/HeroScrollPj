using UnityEngine.Events;

public class IronSourceController : AdControllerBase
{
    private AdAgentBase.States status;

    public override AdAgentBase.States Status => status;
    private const string AppKey = "fd6ae971";
    private const string PlacementId = "DefaultRewardedVideo";
    private UnityAction<bool, string> showAction;
    
    public void Init()
    {
        IronSource.Agent.init(AppKey, IronSourceAdUnits.REWARDED_VIDEO);
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += OnRewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnRewardedVideoAdShowFailedEvent;
    }

    private void OnRewardedVideoAdShowFailedEvent(IronSourceError error) => showAction?.Invoke(false, error.getDescription());

    private void OnRewardedVideoAdRewardedEvent(IronSourcePlacement placement) => showAction?.Invoke(true, string.Empty);

    private void OnRewardedVideoAvailabilityChangedEvent(bool isAvailable) => status = isAvailable ? AdAgentBase.States.Loaded : AdAgentBase.States.None;

    public override void RequestShow(UnityAction<bool, string> requestAction)
    {
        showAction = requestAction;
        IronSource.Agent.showRewardedVideo(PlacementId);
    }

    public override void RequestLoad(UnityAction<bool, string> loadingAction)
    {
    }

    void OnApplicationPause(bool isPaused) => IronSource.Agent.onApplicationPause(isPaused);
}