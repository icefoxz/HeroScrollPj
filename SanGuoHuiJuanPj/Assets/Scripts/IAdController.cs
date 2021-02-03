using System;
using System.Threading;

public enum AdModes
{
    NoAd,
    DirectLoad,
    Preload
}

public interface IAdController
{
    AdModes Mode { get; }
    void RequestRewardAd(Action onSuccessAction, CancellationTokenSource tokenSource);
    void LoadRewardAd(Action onLoad, CancellationTokenSource cancellationToken);
}