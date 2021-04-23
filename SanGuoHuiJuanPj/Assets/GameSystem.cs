using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static GameSystem instance;
    public static LoginUiController LoginUi { get; private set; }
    public LoginUiController loginUiController;
    public static TimeSystemControl TimeSystemControl { get; private set; }
    public TimeSystemControl timeSystemControl;
    public Canvas systemCanvas;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoginUi = loginUiController;
        TimeSystemControl = timeSystemControl;
    }

    public static void InitGameDependencyComponents()
    {
        TimeSystemControl.Init();
    }
}
