using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreLoadController : MonoBehaviour
{
    public SplashImage[] SplashImages;
    public Image DisplayImage;
    public Image FadeImage;
    public GameSystem GameSystemPrefab;
    public GameObject CoroutineObj;
    void Start()
    {
        DisplayImage.gameObject.SetActive(false);
        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization()
    {
        var spList = SplashImages.ToList();
        //AsyncOperation ao = null;
        //
        //ao = SceneManager.LoadSceneAsync((int) GameSystem.GameScene.StartScene, LoadSceneMode.Additive);
        //ao.allowSceneActivation = false;

        var co1 = CoroutineObj.AddComponent<CoObj>();
        co1.Set(GameSystemInit);
        co1.StartAction();
        yield return new WaitForSeconds(0.5f);
        foreach (var sp in spList)
        {
            if(!DisplayImage.gameObject.activeSelf)
                DisplayImage.gameObject.SetActive(true);
            DisplayImage.sprite = sp.Image;
            FadeImage.DOFade(0, sp.Duration);
            yield return new WaitForSeconds(sp.Duration);
            FadeImage.DOFade(1, sp.Duration);
            yield return new WaitForSeconds(sp.Duration);
        }

        yield return new WaitUntil(() => GameSystem.IsInit);
        //ao.allowSceneActivation = true;
        //SceneManager.UnloadSceneAsync(0);
        SceneManager.LoadScene((int) GameSystem.GameScene.StartScene);
    }

    private void GameSystemInit()
    {
        var gs = Instantiate(GameSystemPrefab);
        gs.Init();
    }
}

[Serializable]
public class SplashImage
{
    public Sprite Image;
    [Range(1,5)] public int Duration = 2;
}