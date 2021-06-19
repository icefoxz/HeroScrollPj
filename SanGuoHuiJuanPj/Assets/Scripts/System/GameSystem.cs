using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.HttpUnitScripts;
using Assets.Scripts.Utl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public enum GameScene
    {
        PreloadScene,
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
    public DataTable dataTable;
    public static Configuration Configuration { get; private set; }

    public static GameResources GameResources { get; private set; }
    public Canvas systemCanvas;
    public static bool IsInit { get; private set; }
    public static UnityAction OnWarSceneInit;
    public static UnityAction OnMainSceneInit;
    private Queue<Func<bool>> InitQueue;

    public static MapService MapService { get; private set; }

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Init()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Configuration = configuration;
        LoginUi = loginUiController;
        TimeSystemControl = timeSystemControl;
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;

        //InitEnqueue(dataTable.Init);
        Parallel.Invoke(dataTable.Init);

        InitQueue = new Queue<Func<bool>>();

        InitEnqueue(Configuration.Init);
        InitEnqueue(() =>
        {
            GameResources = new GameResources();
            GameResources.Init();
        });
        InitEnqueue(() => AudioController0.instance.MusicSwitch(GamePref.PrefMusicPlay));
        InitEnqueue(() => AudioController1.instance.MusicSwitch(GamePref.PrefMusicPlay));
        InitEnqueue(() =>
        {
            playerDataForGame.Init();
            IsInit = true;
        });
        StartCoroutine(InitCo());
    }

    IEnumerator InitCo()
    {
        while (InitQueue.Count > 0)
        {
            var action = InitQueue.Dequeue();
            yield return new WaitUntil(action.Invoke);
        }
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
            case GameScene.PreloadScene:
                //OnBeginSceneLoaded();
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        InitEnqueue(EffectsPoolingControl.instance.Init);
        InitEnqueue(StartSceneUIManager.instance.Init);
        InitEnqueue(StartSceneToServerCS.instance.Init);
        InitEnqueue(BarrageUiController.instance.Init);
        InitEnqueue(FightForManagerForStart.instance.Init);
        InitEnqueue(FightControlForStart.instance.Init);
        StartCoroutine(InitCo());
    }

    protected void InitEnqueue(Action action)
    {
        InitQueue.Enqueue(Func);
        bool Func()
        {
            action.Invoke();
            return true;
        }
    }

    public void RegNextSceneLoadAction(UnityAction action) => SceneLoadActions.Add(action);

    public void BeginAllServices()
    {
        MapService = new MapService();
        MapService.Init();
    }
}
