using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public abstract class AdAgent : MonoBehaviour
{
    public enum States
    {
        None,
        Loading,
        Loaded,
        FailedToLoad,
        Showing,
        Closed
    }

    public static AdAgent instance;
    public abstract void Init(AdControllerBase controller);
    public abstract void BusyRetry(UnityAction requestAction, UnityAction cancelAction);
    public abstract void CallAd(UnityAction<bool> callBack);
}