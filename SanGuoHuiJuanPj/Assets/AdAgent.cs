using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AdAgent : MonoBehaviour
{
    public static AdAgent instance;
    public abstract void Init(AdControllerBase controller);
    public abstract void BusyRetry(Action requestAction, Action cancelAction);
}