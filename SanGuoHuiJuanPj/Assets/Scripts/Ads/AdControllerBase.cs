using System;
using System.Threading;
using Donews.mediation;
using UnityEngine;
using UnityEngine.Events;

public abstract class AdControllerBase : MonoBehaviour
{
    public abstract AdAgent.States Status { get; }
    public abstract void RequestShow(UnityAction<bool, string> requestAction);

    public abstract void RequestLoad(UnityAction<bool, string> loadingAction);
}