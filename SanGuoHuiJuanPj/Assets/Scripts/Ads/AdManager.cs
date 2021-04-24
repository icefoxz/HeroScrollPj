using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AdManager : AdControllerBase
{
    public AdAgent adAgentPrefab;
    public int AdmobRetry = 3;
    private bool isInit;
    public AdAgent AdAgent { get; private set; }
    public DoNewAdController DoNewAdController { get; private set; }
    public AdmobController AdmobController { get; private set; }
    public const string MainCanvas = "MainCanvas";
    public override AdAgent.States Status => AdmobController.Status;

    void Start()
    {
        if (isInit) throw XDebug.Throw<AdManager>("Duplicate init!");
        isInit = true;
        var mainCanvas = GameObject.FindGameObjectWithTag(MainCanvas);
        DoNewAdController = gameObject.AddComponent<DoNewAdController>();
        AdmobController = gameObject.AddComponent<AdmobController>();
        AdAgent = Instantiate(adAgentPrefab, mainCanvas.transform);
        AdAgent.Init(this);
        SceneManager.sceneLoaded += SceneLoadRelocateCanvas;
    }

    private void SceneLoadRelocateCanvas(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) return;
        var mainCanvas = GameObject.FindGameObjectWithTag(MainCanvas);
        if (AdAgent != null) return;
        AdAgent = Instantiate(adAgentPrefab, mainCanvas.transform);
        AdAgent.Init(this);
    }

    public override void RequestShow(UnityAction<bool, string> requestAction)
    {
        admobRetryCount = 0;
        if (isAdmobLoaded)
        {
            AdmobController.RequestShow((success,msg) =>
            {
                requestAction(success, msg);
                StartRequestAdmob();
            });
            return;
        }
        DoNewAdController.RequestShow((success,msg) =>
        {
            requestAction(success, msg);
            StartRequestAdmob();
        });
    }

    public override void RequestLoad(UnityAction<bool, string> loadingAction) => loadingAction(true, string.Empty);

    #region 封装内部请求admob
    private bool isAdmobLoaded;
    private int admobRetryCount;

    private void StartRequestAdmob() => AdmobController.RequestLoad(AdmobCallBack);

    private void AdmobCallBack(bool success, string message)
    {
        isAdmobLoaded = success;
        if (success) return;
        admobRetryCount++;
        if(admobRetryCount>=AdmobRetry)return;
        StartRequestAdmob();
    }
    #endregion
}