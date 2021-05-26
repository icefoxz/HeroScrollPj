using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public enum GameScene
    {
        StartScene,
        MainScene,
        WarScene
    }

    public static GameScene CurrentScene { get; private set; }

    public static GameSystem instance;
    public static LoginUiController LoginUi { get; private set; }
    public LoginUiController loginUiController;
    public static TimeSystemControl TimeSystemControl { get; private set; }
    public TimeSystemControl timeSystemControl;
    public PlayerDataForGame playerDataForGame;
    private List<UnityAction> SceneLoadActions { get; } = new List<UnityAction>();
    public Configuration configuration;
    public static Configuration Configuration { get; private set; }

    public static GameResources GameResources { get; private set; }
    public Canvas systemCanvas;
    public static bool IsInit { get; private set; }
    public static UnityAction OnWarSceneInit;
    public static UnityAction OnMainSceneInit;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        Configuration = configuration;
        LoginUi = loginUiController;
        TimeSystemControl = timeSystemControl;
    }

    void Start()
    {
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
        Configuration.Init();
        GameResources = new GameResources();
        GameResources.Init();
        AudioController0.instance.MusicSwitch(GamePref.PrefMusicPlay);
        AudioController1.instance.MusicSwitch(GamePref.PrefMusicPlay);
        playerDataForGame.Init();
        InitScene((GameScene)SceneManager.GetActiveScene().buildIndex);
        IsInit = true;
    }

    public static void InitGameDependencyComponents()
    {
        TimeSystemControl.Init();
    }

    private void InitScene(GameScene scene)
    {
        CurrentScene = scene;
        switch (CurrentScene)
        {
            case GameScene.StartScene:
                OnStartSceneLoaded();
                break;
            case GameScene.MainScene:
                OnMainSceneInit?.Invoke();
                OnMainSceneLoaded();
                break;
            case GameScene.WarScene:
                OnWarSceneInit?.Invoke();
                OnWarSceneLoaded();
                break;
        }
    }

    private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitScene((GameScene) scene.buildIndex);
        if(SceneLoadActions.Count == 0)return;
        SceneLoadActions.ForEach(a=>a?.Invoke());
        SceneLoadActions.Clear();
    }

    private void OnWarSceneLoaded()
    {
        WarsUIManager.instance.Init();
        EffectsPoolingControl.instance.Init();
    }

    private void OnMainSceneLoaded() => UIManager.instance.Init();

    private void OnStartSceneLoaded() 
    {
        EffectsPoolingControl.instance.Init();
    }

    public void RegNextSceneLoadAction(UnityAction action) => SceneLoadActions.Add(action);

}
