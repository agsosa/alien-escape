using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// TODO: Rewrite Banner requests. Copy interstitial?

public class ADManager : MonoBehaviourSingleton<ADManager>
{
    string LiveBannerID = "";
    string TestBannerID = "ca-app-pub-3940256099942544/6300978111";

    string LiveInterstitialID = "";
    string TestInterstitialID = "ca-app-pub-3940256099942544/1033173712";

    string LiveVideoRewardID = "";
    string TestVideoRewardID = "ca-app-pub-3940256099942544/5224354917";

    public float NextBannerRequest = 0;
    public float NextInterstitialRequest = 0;

    private BannerView bannerView;
    private RewardBasedVideoAd rewardBasedVideo;
    private InterstitialAd interstitial;

    public float UpdateRate = 0.1f;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        Debug.LogError("Initializing admanager");
        SceneManager.Instance.OnSceneLoaded += OnSceneLoadHandler;

        MobileAds.SetiOSAppPauseOnBackground(true);

        MobileAds.Initialize(initStatus => {
                Debug.LogError("MobileAds is initialized!");
                RequestRewardedVideo();
                if (DataManager.Instance.PlayerData.NoAds.Value != 10000)
                {
                    RequestBanner();
                    RequestInterstitial();
                }
        });

        // InvokeRepeating("HideBannerCheckInterval", 0, UpdateRate);
    }

    private void OnDisable()
    {
        SceneManager.Instance.OnSceneLoaded -= OnSceneLoadHandler;
    }

    private void OnSceneLoadHandler(SceneLoadType type)
    {
        // Initialize the Google Mobile Ads SDK.

        if (type != SceneLoadType.FIRST_LOAD)
        {
            if (bannerView == null || Time.time > NextBannerRequest)
            {
                RequestBanner();
            }
        }
    }

    #region AD BANNER

    bool IsBannerLoaded = false;
    bool loadingBanner = false;
    AdRequest request;

    private void RequestBanner()
    {
        if (DataManager.Instance.PlayerData.NoAds.Value != 10000)
        {
#if UNITY_ANDROID
            string adUnitId = !PreLoadSceneManager.IsTest ? LiveBannerID : TestBannerID;
#elif UNITY_IPHONE
            string adUnitId = "unexpected_platform";
#else
            string adUnitId = "unexpected_platform";
#endif
            Debug.LogError("RequestBanner()");

            if (loadingBanner) return;

            if (Time.time < NextBannerRequest)
            {
                if (IsBannerLoaded) return;
            }

            if (bannerView != null)
            {
                bannerView.Destroy();
            }

            // Create a 320x50 banner at the top of the screen.
            AdSize adSize = new AdSize(250, 32);
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
            NextBannerRequest = Time.time + 900;

            // Create an empty ad request.
            if (request == null)
            {
                request = new AdRequest.Builder()/*.AddTestDevice("F1133C3F461326F642594824949A9133")*/.Build();
            }

            // Load the banner with the request.
            IsBannerLoaded = false;
            loadingBanner = true;
            bannerView.LoadAd(request);

            // Called when an ad request has successfully loaded.
            bannerView.OnAdLoaded += HandleOnAdLoaded;
            // Called when an ad request failed to load.
            bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        }
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
            if (DataManager.Instance.PlayerData.NoAds.Value != 10000)
            {
                Debug.LogError("HandleAdLoaded event received");
                loadingBanner = false;
                IsBannerLoaded = true;
                bannerView.Show();
            }
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        loadingBanner = false;
        IsBannerLoaded = false;
        Debug.LogError("Banner failed to load due to " + args.Message);
        /*await Task.Delay(300);
        IsBannerLoaded = false;
        if (banner_retries < 4)
        {
            Debug.LogError("Retrying banner load");
            bannerView.LoadAd(request);
            banner_retries++;
        }
        if (banner_retries >= 4)
        {
            loadingBanner = false;
        }*/
    }

    public void DestroyBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
            bannerView.Destroy();
        }
    }
    #endregion

    #region Video rewards

    public delegate void OnVideoRewardSuccess();
    public OnVideoRewardSuccess OnReward;

    void InitializeRewardBasedVideo()
    {
        if (rewardBasedVideo == null)
        {
            rewardBasedVideo = RewardBasedVideoAd.Instance;
            rewardBasedVideo.OnAdFailedToLoad += HandleOnRewardedVideoAdFailedToLoad;
            rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        }
    }

    AdRequest videoRequest;
    void RequestRewardedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = !PreLoadSceneManager.IsTest ? LiveVideoRewardID : TestVideoRewardID;
#elif UNITY_IPHONE
        string adUnitId = "unexpected_platform";
#else
        string adUnitId = "unexpected_platform";
#endif
        InitializeRewardBasedVideo();

        if (videoRequest == null)
        {
            videoRequest = new AdRequest.Builder()/*.AddTestDevice("F1133C3F461326F642594824949A9133")*/.Build();
        }
        rewardBasedVideo.LoadAd(videoRequest, adUnitId);
    }

    /*
     *         await Task.Delay(500);
        UnityThreadHelper.executeInUpdate(() =>
        {
            Debug.LogError("(Interstitial) HandleFailedToReceiveAd event received with message: "
                            + args.Message);
            if (interstitial_retries <= 4)
            {
                Debug.LogError("Retrying interstitial load");
                RequestInterstitial();
                interstitial_retries++;
            }
        });*/

    int rewardvideo_retries = 0;
    public async void HandleOnRewardedVideoAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        await Task.Delay(200);
            Debug.LogError("(rewarded video) HandleFailedToReceiveAd event received with message: "
                            + args.Message);
            if (rewardvideo_retries <= 4)
            {
                Debug.LogError("Retrying rewarded video load");
                RequestRewardedVideo();
                rewardvideo_retries++;
            }
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        UnityThreadHelper.executeInUpdate(() => {
            if (OnReward != null)
            {
                OnReward();
                OnReward = null;
            }
        });
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
            RequestRewardedVideo();
    }

    public void TryShowVideoReward(OnVideoRewardSuccess OnReward)
    {
        bool request = false;
        if (rewardBasedVideo != null && rewardBasedVideo.IsLoaded())
        {
            try
            {
                this.OnReward = OnReward;
                rewardBasedVideo.Show();
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception while opening rewarded video " + ex.Message);
                GlobalUIManager.Instance.ShowErrorPopup(GameUtils.GetTranslatedText("ErrorWhileLoadingRewardedVideo"), 58);
                request = true;
            }
        }
        else
        {
            GlobalUIManager.Instance.ShowErrorPopup(GameUtils.GetTranslatedText("ErrorWhileLoadingRewardedVideo"), 59);
            request = true;
        }

        if (request) RequestRewardedVideo();
    }
    #endregion

    #region Interstitials
    bool loadingAd = false;
    AdRequest InterstitialRequest;

    public void RequestInterstitial()
    {
        if (DataManager.Instance.PlayerData.NoAds.Value != 10000)
        {
#if UNITY_ANDROID
            string adUnitId = !PreLoadSceneManager.IsTest ? LiveInterstitialID : TestInterstitialID;
#elif UNITY_IPHONE
        string adUnitId = "unexpected_platform";
#else
        string adUnitId = "unexpected_platform";
#endif

            if (loadingAd) return;

            if (interstitial == null)
            {
                this.interstitial = new InterstitialAd(adUnitId);
                // Event handle
                this.interstitial.OnAdLoaded += HandleOnInterstitialLoaded;
                this.interstitial.OnAdFailedToLoad += HandleOnInterstitialFailedToLoad;
                this.interstitial.OnAdClosed += HandleOnInterstitialClosed;
            }

            if (interstitial.IsLoaded()) return;

            if (InterstitialRequest == null)
            {
                InterstitialRequest = new AdRequest.Builder()/*.AddTestDevice("F1133C3F461326F642594824949A9133")*/.Build();
            }

            //AdRequest.Builder.addTestDevice("F1133C3F461326F642594824949A9133")

            loadingAd = true;
            this.interstitial.LoadAd(InterstitialRequest);
        }
    }


    public void HandleOnInterstitialLoaded(object sender, EventArgs args)
    {
        loadingAd = false;
    }

    public void HandleOnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        loadingAd = false;
        Debug.LogError("Interstitial failed to load due to " + args.Message);
    }

    public void HandleOnInterstitialClosed(object sender, EventArgs args)
    {
            Debug.LogError("HandleAdClosed event received");
             RequestInterstitial();
    }

    public void TryShowInterstitial()
    {
        ShowOrRequestInterstitial();
    }

    void ShowOrRequestInterstitial() // Will return true if interstitial is shown
    {
        if (DataManager.Instance.PlayerData.NoAds.Value == 10000) return;

        RequestInterstitial();

        if (interstitial != null && interstitial.IsLoaded())
        {
            Debug.LogError("Showing interstitial");
            interstitial.Show();
        }
        
    }
    #endregion
}
