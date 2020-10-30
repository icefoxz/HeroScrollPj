using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdUnion;

public class main : MonoBehaviour
{
    private AdUnionContext adContext;

    private Transform canvasForm;
    private Button initButton;
    private Button bannerButton;
    private Button splashButton;

    private Button loadInterstitialButton;
    private Button showInterstitialButton;

    private Button nativeButton;

    private Button loadVideoButton;
    private Button showVideoButton;

    private AdUnionBanner banner;
    private AdUnionInterstitial interstitial;

    private AdUnionVideo video;     //激励视频

    // Start is called before the first frame update
    void Start()
    {
        adContext = AdUnionContext.GetInstance(); 

        canvasForm = GameObject.Find("Canvas").transform;
        //获取控件对象设置时间监听
        initButton = canvasForm.Find("InitButton").GetComponent<Button>();
        bannerButton = canvasForm.Find("BannerButton").GetComponent<Button>();
        splashButton = canvasForm.Find("SplashButton").GetComponent<Button>();

        loadInterstitialButton = canvasForm.Find("LoadInterstitialButton").GetComponent<Button>();
        showInterstitialButton = canvasForm.Find("ShowInterstitialButton").GetComponent<Button>();

        nativeButton = canvasForm.Find("NativeButton").GetComponent<Button>();

        loadVideoButton = canvasForm.Find("LoadVideoButton").GetComponent<Button>();
        showVideoButton = canvasForm.Find("ShowVideoButton").GetComponent<Button>();

        initButton.onClick.AddListener(InitSDK);
        bannerButton.onClick.AddListener(LoadBanner);
        splashButton.onClick.AddListener(LoadSplash);

        loadInterstitialButton.onClick.AddListener(LoadInterstitial);
        showInterstitialButton.onClick.AddListener(ShowInterstitial);
        nativeButton.onClick.AddListener(LoadNative);

        loadVideoButton.onClick.AddListener(LoadVideo);
        showVideoButton.onClick.AddListener(ShowVideo);
    }

    public void InitSDK()
    {
        ToastUtils.showToast("InitSDK");
        AndroidJavaClass adUnionClass = new AndroidJavaClass("com.mob4399.adunion.AdUnionSDK");
        //应用id
        string appId = "3048";
        //调用初始化方法      
        adUnionClass.CallStatic("init", adContext.GetActivity(), appId, new OnAuInitListenerProxy());

    }

    public void LoadBanner()
    {
        LogUtils.print("加载banner广告");

        banner = new AdUnionBanner("4");
        banner.loadBanner();
    }

    public void LoadSplash()
    {
        LogUtils.print("加载开屏广告");
        adContext.RunOnUIThread(new AndroidJavaRunnable(() => {

            AdUnionSplash adunionSplash = new AdUnionSplash("10549");

        }));
    }

    public void LoadInterstitial()
    {
        interstitial = new AdUnionInterstitial("5892"); 
    }

    public void ShowInterstitial()
    {
        if(interstitial == null)
        {
            ToastUtils.showToast("请先初始化");
        }else
        {
            interstitial.show();
        }
        
    }

    public void LoadNative()
    {
        AdUnionNative nativeAd = new AdUnionNative("3378");
    }


    public void LoadVideo()
    {
        video = new AdUnionVideo("12730");
    }

    public void ShowVideo()
    {
        if(video == null )
        {
            ToastUtils.showToast("请先初始化");
        }
        else
        {
            video.show();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
