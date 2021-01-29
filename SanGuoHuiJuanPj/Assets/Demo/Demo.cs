//
//

using UnityEngine;
using Donews.mediation;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{

    [SerializeField]
    private Text logLabel;

    void Awake()
    {
        // 注意Debug仅为demo使用，在开发过程中无论正式与测试，都请使用Release
        SDK.apiType = SDK.ApiType.Release;
        SDK.StartService();
        _ = SDK.AllSDKVersionInfo;
    }

    private SplashAd splashAd;

    public void LoadSplashAd()
    {
         
#if UNITY_IOS
        string placeId = "4422";
#elif UNITY_ANDROID
        string placeId = "5234";
#else
        string placeId = "0";
#endif
        SplashAdListener listener = new SplashAdListener(this);
        SplashAd splashAd = SplashAd.LoadSplashAd(placeId, listener);
        this.splashAd = splashAd;
    }

    private sealed class SplashAdListener : ISplashAdListener
    {

        private readonly Demo demo;

        public SplashAdListener(Demo demo)
        {
            this.demo = demo;
        }

        private void showInfo(string info)
        {
            Debug.Log(info);
            this.demo.logLabel.text = info;
        }

        public void SplashAdDidLoadFaild(int errorCode, string errorMsg)
        {
            string info = "splash error:" + errorCode + ", msg:" + errorMsg;
            this.showInfo(info);
        }

        public void SplashAdDidLoadSuccess()
        {
            this.showInfo("splash load success");
        }

        public void SplashAdDidClickCloseButton()
        {
            this.showInfo("splash did click close button");
        }

        public void SplashAdDidClicked()
        {
            this.showInfo("splash did clicked");
        }

        public void SplashAdDidClose()
        {
            this.showInfo("splash did close");
        }

        public void SplashAdExposured()
        {
            this.showInfo("splash did exposured");
        }

        public void SplashAdWillClose()
        {
            this.showInfo("splash will close");
        }
    }

    private InterstitialAd interstitialAd;

    public void LoadInterstitialAd()
    {
#if UNITY_IOS
        string placeId = "4424";
#elif UNITY_ANDROID
        string placeId = "4068";
#else
        string placeId = "0";
#endif
        InterstitialAdListener listener = new InterstitialAdListener(this);
        InterstitialAd interstitialAd = InterstitialAd.LoadInterstitialAd(placeId, listener);
        this.interstitialAd = interstitialAd;
    }

    private sealed class InterstitialAdListener : IInterstitialAdListener
    {
        private readonly Demo demo;

        public InterstitialAdListener(Demo demo)
        {
            this.demo = demo;
        }

        private void showInfo(string info)
        {
            Debug.Log(info);
            this.demo.logLabel.text = info;
        }

        public void InterstitialDidLoadFaild(int errorCode, string errorMsg)
        {
            string info = "interstitial error:" + errorCode + ", msg:" + errorMsg;
            this.showInfo(info);
        }

        public void InterstitialDidLoadSuccess()
        {
            this.showInfo("interstitial did load success");
        }

        public void InterstitialAdWillVisible()
        {
            this.showInfo("interstitial will visible");
        }

        public void InterstitialAdExposured()
        {
            this.showInfo("interstitial exposured");
        }

        public void InterstitialAdDidClicked()
        {
            this.showInfo("interstitial did clicked");
        }

        public void InterstitialAdDidClose()
        {
            this.showInfo("interstitial did close");
        }

        public void InterstitialAdDidClosedDetails()
        {
            this.showInfo("interstitial did closed details");
        }

    }

    private RewardedVideoAd rewardedVideoAd;
    private bool isRewardedVideoAdLoadFinish = false;

    public void LoadRewardedVideoAd()
    {
#if UNITY_IOS
        string placeId = "4428";
#elif UNITY_ANDROID
        string placeId = "4164";
#else
        string placeId = "0";
#endif
        RewardedVideoAdListener listener = new RewardedVideoAdListener(this);
        RewardedVideoAd rewardedVideoAd = RewardedVideoAd.LoadRewardedVideoAd(placeId, listener);
        this.rewardedVideoAd = rewardedVideoAd;
    }

    public void ShowRewardedVideoAd()
    {
        if (this.rewardedVideoAd == null)
        {
            string info = "还没有加载RewardedVideoAd， 请先加载";
            Debug.Log(info);
            this.logLabel.text = info;
        }
        else if (!isRewardedVideoAdLoadFinish)
        {
            string info = "正在加载RewardedVideoAd资源， 请稍后";
            Debug.Log(info);
            this.logLabel.text = info;
        }
        else
        {
            this.rewardedVideoAd.ShowRewardedVideoAd();
        }
    }

    private sealed class RewardedVideoAdListener : IRewardedVideoAdListener
    {
        private readonly Demo demo;

        public RewardedVideoAdListener(Demo demo)
        {
            this.demo = demo;
        }

        private void showInfo(string info)
        {
            Debug.Log(info);
            this.demo.logLabel.text = info;
        }

        public void RewardVideoAdDidLoadSuccess()
        {
            this.showInfo("rewardVideo did load success");
        }

        public void RewardVideoAdDidLoadFaild(int errorCode, string errorMsg)
        {
            string info = "rewardVideo error:" + errorCode + ", msg:" + errorMsg;
            this.showInfo(info);
        }

        public void RewardVideoAdVideoDidLoad()
        {
            this.showInfo("rewardVideo video did load");
            this.demo.isRewardedVideoAdLoadFinish = true;
        }

        public void RewardVideoAdWillVisible()
        {
            this.showInfo("rewardVideo will visible");
        }

        public void RewardVideoAdExposured()
        {
            this.showInfo("rewardVideo exposured");
            this.demo.isRewardedVideoAdLoadFinish = false;
        }

        public void RewardVideoAdDidClose()
        {
            this.showInfo("rewardVideo did close");
        }

        public void RewardVideoAdDidClicked()
        {
            this.showInfo("rewardVideo did clicked");
        }

        public void RewardVideoAdDidRewardEffective()
        {
            this.showInfo("rewardVideo did reward effective");
        }

        public void RewardVideoAdDidPlayFinish()
        {
            this.showInfo("rewardVideo did play finish");
        }
    }

    private BannerAd bannerAd;

    public void LoadBannerAd()
    {
#if UNITY_IOS
        string placeId = "4426";
#elif UNITY_ANDROID
        string placeId = "4072";
#else
        string placeId = "0";
#endif
        BannerAdListener listener = new BannerAdListener(this);

        BannerAd.Layout layout;
        layout.width = -1;
        layout.height = 360;
        layout.top = -1;
        layout.left = 40;
        layout.right = 40;
        layout.bottom = 40;

        BannerAd bannerAd = BannerAd.LoadBannerAd(placeId, layout, listener);
        this.bannerAd = bannerAd;
    }

    private sealed class BannerAdListener : IBannerAdListener
    {
        private readonly Demo demo;

        public BannerAdListener(Demo demo)
        {
            this.demo = demo;
        }

        private void showInfo(string info)
        {
            Debug.Log(info);
            this.demo.logLabel.text = info;
        }

        public void BannerAdDidLoadSuccess()
        {
            this.showInfo("banner did load success");
        }

        public void BannerAdDidLoadFaild(int errorCode, string errorMsg)
        {
            string info = "banner error:" + errorCode + ", msg:" + errorMsg;
            this.showInfo(info);
        }

        public void BannerAdExposured()
        {
            this.showInfo("banner exposured");
        }

        public void BannerAdDidClicked()
        {
            this.showInfo("banner did clicked");
        }

        public void BannerAdDidClickClose()
        {
            this.showInfo("banner did click close");
        }

        public void BannerAdDidShowDetails()
        {
            this.showInfo("banner did show details");
        }

        public void BannerAdDidCloseDetails()
        {
            this.showInfo("banner did close details");
        }
    }
}
