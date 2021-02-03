using System;
using System.Threading;
using Donews.mediation;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AdControllerBase : MonoBehaviour
{
    public AdAgent adAgentPrefab;
    private bool isInit;
    public AdAgent AdAgent { get; private set; }

    protected virtual void Init()
    {
        if (isInit) throw XDebug.Throw<DoNewAdController>("Duplicate init!");
        isInit = true;

        //StartCoroutine(UpdateEverySecond());
        DontDestroyOnLoad(gameObject);
        var mainCanvas = GameObject.FindGameObjectWithTag(DoNewAdController.MainCanvas);
        AdAgent = Instantiate(adAgentPrefab,mainCanvas.transform);
        AdAgent.Init(this);
        SceneManager.sceneLoaded += SceneLoadRelocateCanvas;
    }

    private void SceneLoadRelocateCanvas(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0) return;
        var mainCanvas = GameObject.FindGameObjectWithTag(DoNewAdController.MainCanvas);
        if (AdAgent != null) return;
        AdAgent = Instantiate(adAgentPrefab,mainCanvas.transform);
        AdAgent.Init(this);
    }

    public abstract AdModes Mode { get; }
}