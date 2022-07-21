using System;
using UnityEngine;

public class AdsManager : MonoBehaviourSingletonPersistent<AdsManager>
{
    //public static AdsManager instance;

    public string currentAdPlacement;    

    //private const string ironSrcAppID = "11c6186ed";
    private const string ironSrcAppID = "13fc28a99";

    public static event Action<bool> OnIronSrcRewardVideoAvailable;
    public static event Action OnIronSrcRewardVideoClicked;
    public static event Action OnIronSrcRewardVideoFailed;
    public static event Action<string> OnIronSrcRewardVideoComplete;    

    public struct userAttributes { };
    public struct appAttributes { };

    /*private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);    
    }*/

    private void OnApplicationPause(bool pause)
    {
        IronSource.Agent.onApplicationPause(pause);
    }

    private void Start()
    {
        IronSource.Agent.init(ironSrcAppID, IronSourceAdUnits.REWARDED_VIDEO);        
        IronSource.Agent.setAdaptersDebug(true);
        IronSource.Agent.validateIntegration();        

        //RegisterBannerAds();
        //IronSource.Agent.displayBanner();
        RegisterRewardAds();
        //RegisterInterstitialAds();
        //IronSource.Agent.loadInterstitial();        
    }

    private void RegisterBannerAds()
    {
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
    }

    //Invoked once the banner has loaded
    private void BannerAdLoadedEvent()
    {
        IronSource.Agent.displayBanner();
    }
    //Invoked when the banner loading process has failed.
    //@param description - string - contains information about the failure.
    private void BannerAdLoadFailedEvent(IronSourceError error)
    {
    }
    // Invoked when end user clicks on the banner ad
    private void BannerAdClickedEvent()
    {
    }
    //Notifies the presentation of a full screen content following user click
    private void BannerAdScreenPresentedEvent()
    {
    }
    //Notifies the presented screen has been dismissed
    private void BannerAdScreenDismissedEvent()
    {
    }
    //Invoked when the user leaves the app
    private void BannerAdLeftApplicationEvent()
    {
    }


    #region Reward Ads
    private void RegisterRewardAds()
    {
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
    }

    private void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
    {
        OnIronSrcRewardVideoAvailable?.Invoke(canShowAd);
        Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
    }

    private void RewardedVideoAdOpenedEvent()
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
    }

    private void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
    {
        OnIronSrcRewardVideoComplete?.Invoke(ssp.getPlacementName());
        Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
        //userTotalCredits = userTotalCredits + ssp.getRewardAmount();
        //AmountText.GetComponent<UnityEngine.UI.Text>().text = "" + userTotalCredits;

        /*switch (ssp.getPlacementName())
        {
            case "RV_CoinsUpgrade":
                break;
        }*/

    }

    private void RewardedVideoAdClosedEvent()
    {
        Debug.Log("IronSrc: I got RewardedVideoAdClosedEvent");
    }

    private void RewardedVideoAdStartedEvent()
    {
        Debug.Log("IronSrc: I got RewardedVideoAdStartedEvent");
    }

    private void RewardedVideoAdEndedEvent()
    {
        Debug.Log("IronSrc: I got RewardedVideoAdEndedEvent");
    }

    private void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        OnIronSrcRewardVideoFailed?.Invoke();
        Debug.Log("IronSrc: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
    }

    private void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
    {
        OnIronSrcRewardVideoClicked?.Invoke();
        Debug.Log("IronSrc: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
    }

    public void ShowRewardAd()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
    }

    public void ShowRewardAd(string placementName)
    {
        currentAdPlacement = placementName;
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo(placementName);
        }
    }
    #endregion    

    private void OnDisable()
    {
        IronSourceEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent -= RewardedVideoAdClickedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent -= RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent -= RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent -= RewardedVideoAdShowFailedEvent;

        IronSourceEvents.onInterstitialAdReadyEvent -= InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent -= InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent -= InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent -= InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent -= InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent -= InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent -= InterstitialAdClosedEvent;

        IronSourceEvents.onBannerAdLoadedEvent -= BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent -= BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent -= BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent -= BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent -= BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent -= BannerAdLeftApplicationEvent;
    }

    private void OnDestroy()
    {
        IronSourceEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClickedEvent -= RewardedVideoAdClickedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent -= RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent -= RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent -= RewardedVideoAdShowFailedEvent;

        IronSourceEvents.onInterstitialAdReadyEvent -= InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent -= InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent -= InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent -= InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent -= InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent -= InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent -= InterstitialAdClosedEvent;

        IronSourceEvents.onBannerAdLoadedEvent -= BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent -= BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent -= BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent -= BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent -= BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent -= BannerAdLeftApplicationEvent;
    }

    #region Interstitial Ads
    private void RegisterInterstitialAds()
    {
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
    }

    private void InterstitialAdReadyEvent()
    {
    }

    private void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
    }

    private void InterstitialAdShowSucceededEvent()
    {
    }

    private void InterstitialAdShowFailedEvent(IronSourceError error)
    {
    }

    private void InterstitialAdClickedEvent()
    {
    }

    private void InterstitialAdOpenedEvent()
    {
    }

    private void InterstitialAdClosedEvent()
    {
        IronSource.Agent.loadInterstitial();
    }
    #endregion

    //===========================================================================================

    public void ShowInterstiatialAd()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
    }

    public void ShowInterstiatialAd(string placementName)
    {
        currentAdPlacement = placementName;
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial(placementName);
        }
    }

    //===========================================================================================
}
