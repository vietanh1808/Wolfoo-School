using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SCN.Ads;
public class AdsManager : MonoBehaviour
{
    private float interval = 30f;
    private float lastTimeInter;
    private int levelNoAds=3;

    public float Inter_next_countplay = 0;
    public bool Inter_next_evenlevel = true;
    public bool Inter_next_oddlevel = true;
    public bool reward_free_item = true;
    public bool reward_free_coin = true;
    public bool inter_restart = true;
    public bool banner_id = true;
    public bool inter_next_level = true;
    private int lastLevelReplay=-1;
    private static AdsManager _instance;
    public static AdsManager Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = (new GameObject("AdsManager")).AddComponent<AdsManager>();
            return _instance;
        }
    }

    public static bool Initialized => _instance != null;

    /// <summary>
    /// Use to listening when the reward video ad is available, usefull for turn on some ad button.
    /// </summary>
    public event Action onRewardAvailable;

    [SerializeField] private bool useAdmob = true;
    private bool useAdUnityBackup;

    #region Remove Ads
    private const string RemoveAdsKey = "NO_ADS";
    /// <summary>
    /// If true, User no longer to see Banner or Interstitial ads, but still can see Reward video ads.
    /// </summary>
    public bool IsRemovedAds
    {
        get => PlayerPrefs.HasKey(RemoveAdsKey);
    }

    /// <summary>
    /// Remove all Banner & Interstital ads (Still keep reward video ads)
    /// <para>Call this when user buy Remove_Ads.</para>
    /// </summary>
    public void SetRemovedAds()
    {
        PlayerPrefs.SetInt(RemoveAdsKey, 1);
        DestroyBanner();
    }
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (useAdmob)
        {
            //useAdUnityBackup = AdmobConfig.Instance.UseAdUnityBackup;
            Debug.Log($"[Ads] Initializing... useUnity={useAdUnityBackup}, log={AdmobConfig.Instance.EnableLog}, test={AdmobConfig.Instance.UseTestID}");

            gameObject.AddComponent<AdsAdmob>();
            AdsAdmob.Instance.OnRewardAdLoaded += OnRewardAvailable;

           // if (useAdUnityBackup) EnableUnityAds();
        }
    }

    private void OnRewardAvailable()
    {
        onRewardAvailable?.Invoke();
    }

    /// <summary>
    /// Call this to preload ads to show ingame. Should be call at the first application script
    /// </summary>
    public void Preload() { }

    public void Preload(Transform parent)
    {
        transform.SetParent(parent);
    }

    /// <summary>
    /// Use for change setting at runtime, you can use remote config and call this method to enable/disable Unity Ad.
    /// </summary>
    /// <param name="enable"></param>
    //public void SetUnityAdBackup(bool enable)
    //{
    //    AdmobConfig.Instance.UseAdUnityBackup = enable;
    //    useAdUnityBackup = enable;
    //    if (enable)
    //    {
    //        EnableUnityAds();
    //    }
    //}

    //private void EnableUnityAds()
    //{
    //    if (gameObject.GetComponent<AdsUnity>() != null) return;

    //    gameObject.AddComponent<AdsUnity>();
    //    AdsUnity.Instance.Preload(AdmobConfig.Instance.AdUnityAppID, AdmobConfig.Instance.EnableLog, AdmobConfig.Instance.UseTestID);
    //    AdsUnity.Instance.OnRewardVideoAdLoaded += OnRewardVideoAvailable;
    //}

    public float countPlay = 0;
    public bool HasInters
    {
        get
        {
            //if (IsRemovedAds || PlayerData.LevelUnlock % 2 !=0 /*|| Time.time <= lastTimeInter*/ || !inter_next_level) return false;

            //return (useAdmob && AdsAdmob.Instance.HasInterNext) /*|| (useAdUnityBackup && AdsUnity.Instance.HasInterstitial)*/;
            if (IsRemovedAds) return false;
            return (useAdmob && AdsAdmob.Instance.HasInter);
        }
    }

    public bool HasRewardVideo
    {
        get
        {
            if (!reward_free_coin)
            {
                return false;
            }
            return (useAdmob && AdsAdmob.Instance.HasRewardVideo);
        }
    }



    public void ShowBanner()
    {
        if (!IsRemovedAds && useAdmob && banner_id)
        {
            AdsAdmob.Instance.ShowBanner();
        }
    }

    public void HideBanner()
    {
        if (!IsRemovedAds && useAdmob)
        {
            AdsAdmob.Instance.HideBanner();
        }
    }

    public void DestroyBanner()
    {
        if (useAdmob)
        {
            AdsAdmob.Instance.DestroyBanner();
        }
    }

    public float GetBannerHeight()
    {
        if (!IsRemovedAds && useAdmob) return AdsAdmob.Instance.BannerHeight;
        return 0;
    }

    public void ShowInterstitial(Action callback = null)
    {
        if (IsRemovedAds)
        {
         //   lastTimeInter = Time.time + interval;
            callback?.Invoke();
            return;
        }

        OnShowAdStart();
        callback += OnShowAdFinished;

        if (useAdmob && AdsAdmob.Instance.HasInter)
        {
            countPlay = 0;
            //   lastTimeInter = Time.time + interval;
            AdsAdmob.Instance.ShowInterstitial(callback);
        }
        else callback?.Invoke();
    }

    public void ShowRewardVideo(Action onSuccess, Action onClosed = null)
    {
        OnShowAdStart();
        onSuccess += OnShowAdFinished;
        onClosed += OnShowAdFinished;

        if (useAdmob && AdsAdmob.Instance.HasRewardVideo)
        {
            AdsAdmob.Instance.ShowRewardVideo(onSuccess, onClosed);
        }
        else onClosed?.Invoke();
    }
    private bool isAdShowing;

    private void OnShowAdStart()
    {
        isAdShowing = true;
    }

    private void OnShowAdFinished()
    {
        isAdShowing = false;
    }

    //private void OnApplicationFocus(bool focus)
    //{
    //    if (isAdShowing)
    //    {
    //        Debug.Log($"OnApplicationFocus: focus={focus}, isAdShowing={isAdShowing}");
    //        Application.runInBackground = !focus;
    //    }
    //}

}
