using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SCN.Ads;
using SCN.FirebaseLib.FA;

namespace SCN.Ads
{
    public class AdsManager : MonoBehaviour
    {
        static AdsManager _instance;
        public static AdsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (new GameObject("AdsManager")).AddComponent<AdsManager>();
                    _instance.Setup();
                }

                return _instance;
            }
        }

        void Setup()
        {
            DontDestroyOnLoad(gameObject);

            // Admob
            _instance.admobControl = _instance.gameObject.AddComponent<AdsAdmob>();
            _instance.admobControl.Setup();

            // Woa ads
            _instance.woaAdsControl = _instance.gameObject.AddComponent<WoaAdsManager>();
            _instance.woaAdsControl.Setup();

            _instance.inter_interval = admobControl.Config.Inter_Interval;
            _instance.rv_interval = admobControl.Config.Rv_interval;

            admobControl.OnRewardAdLoaded += Event_OnRewardAvailable;
        }

        /// <summary>
        /// Use to listening when the reward video ad is available, usefull for turn on some ad button.
        /// </summary>
        public Action OnRewardAvailable;

        void Event_OnRewardAvailable()
        {
            OnRewardAvailable?.Invoke();
        }

		readonly bool enableLog = false;
        float inter_interval;
        float rv_interval;
        float nextTimeInter;
        AdsAdmob admobControl;
        WoaAdsManager woaAdsControl;

        public AdsAdmob AdmobControl => admobControl;
        public WoaAdsManager WoaAdsControl => woaAdsControl;

        #region Remove Ads
        const string RemoveAdsKey = "NO_ADS";
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

            GAManager.Instance.TrackBuyRemoveAds();
        }
        #endregion

        /// <summary>
        /// Call this to preload ads to show ingame. Should be call at the first application script
        /// </summary>
        public void Preload() { }

        public void Preload(Transform parent)
        {
            transform.SetParent(parent);
        }

        public bool HasInters
        {
            get
            {
                if (IsRemovedAds || Time.time <= nextTimeInter)
                {
                    return false;
                }

                return admobControl.HasInter || woaAdsControl.HasAds;
            }
        }

        public bool HasRewardVideo
        {
			get
			{
                return admobControl.HasRewardVideo || woaAdsControl.HasAds;
            }
        } 

        public bool HasRewardInter
        {
            get
            {
                if (IsRemovedAds)
                {
                    return false;
                }

                return admobControl.HasInter;
            }
        }

        #region Banner
        public void ShowBanner()
        {
            if (IsRemovedAds)
            {
                return;
            }

            admobControl.ShowBanner();
        }

        public void HideBanner()
        {
            if (IsRemovedAds)
            {
                return;
            }

            admobControl.HideBanner();
        }

        public void DestroyBanner()
        {
            if (IsRemovedAds)
            {
                return;
            }

            admobControl.DestroyBanner();
        }

        public float GetBannerHeight()
        {
            return admobControl.BannerHeight;
        }
        #endregion

        #region Interstitial
        /// <param name="showPos">Vi tri show ads, dung de tracking</param>
        /// <param name="onShowAdFinished">Call sau khi show ads hoan thanh, call ngay lap tuc khi khong co ads</param>
        public void ShowInterstitial(Action<bool> onShowAdFinished = null)
        {
			if (!HasInters) // Khong du dieu kien show bat ki ads nao
			{
                onShowAdFinished?.Invoke(false);
                return;
			}

            var showAdsCount = PlayerPrefs.GetInt(ConstValue.ShowAdCountKey, 0);
            var showWoaAdsCount = PlayerPrefs.GetInt(ConstValue.ShowWoaAdCountKey, 0);
            var hasClick = PlayerPrefs.GetInt(ConstValue.ClickWoaAdCountKey, 0) != 0;

            if (woaAdsControl.HasAdsFromServer) // Co woa ads tai tu tren Server => chac chan show duoc ads
			{
                var adCount = woaAdsControl.AdsStats.Ad_count;

                // cu 3 lan show ads thi se co 1 lan show Q/C cheo, so luong show quang cao cheo ko duoc vuot qua so luot quy dinh
                if (showAdsCount % 3 == 2 && showWoaAdsCount < adCount && !hasClick)
				{
                    ShowWoa(); // Show ads lay tren Server neu da show du Admob
                }
                else
                {
                    ShowAdmob(); // Show admob neu chua du so luong, neu ko request duoc admob thi se show Woa default
                }
            }
            else // Khong co woa ads server => show ads Admob, neu ko request duoc admob thi se show Woa default
            {
                ShowAdmob();
            }
            
            void ShowAdmob()
			{
				if (admobControl.HasInter)
                {
                    PlayerPrefs.SetInt(ConstValue.ShowAdCountKey, showAdsCount + 1);
                    AddNextTimeInter(inter_interval);
                    admobControl.ShowInterstitial(onShowAdFinished);
                }
				else
				{
                    ShowWoa();
				}
            }

            void ShowWoa()
			{
                if (woaAdsControl.ShowInterstitial(onShowAdFinished))
                {
                    PlayerPrefs.SetInt(ConstValue.ShowAdCountKey, showAdsCount + 1);
                    PlayerPrefs.SetInt(ConstValue.ShowWoaAdCountKey, showWoaAdsCount + 1);

                    AddNextTimeInter(inter_interval);
                }
            }
        }
        #endregion

        #region Reward video
        /// <param name="showPos">Vi tri show ads, dung de tracking</param>
        /// <param name="onSuccess">Khi xem ads thanh cong</param>
        /// <param name="onClosed">Khi ads het hoac tat ads giua chung</param>
        public void ShowRewardVideo( Action onSuccess = null, Action onClosed = null)
        {
            if (!HasRewardVideo) // Khong du dieu kien show bat ki ads nao
            {
                onClosed?.Invoke();
                return;
            }

            var showAdsCount = PlayerPrefs.GetInt(ConstValue.ShowAdCountKey, 0);
            var showWoaAdsCount = PlayerPrefs.GetInt(ConstValue.ShowWoaAdCountKey, 0);
            var hasClick = PlayerPrefs.GetInt(ConstValue.ClickWoaAdCountKey, 0) != 0;

            if (woaAdsControl.HasAdsFromServer) // Co woa ads tai tu tren Server => chac chan show duoc ads
            {
                var adCount = woaAdsControl.AdsStats.Ad_count;

                // cu 3 lan show ads thi se co 1 lan show Q/C cheo, so luong show quang cao cheo ko duoc vuot qua so luot quy dinh
                if (showAdsCount % 3 == 2 && showWoaAdsCount < adCount && !hasClick)
                {
                    ShowWoa(); // Show ads lay tren Server neu da show du Admob
                }
                else
                {
                    ShowAdmob(); // Show admob neu chua du so luong, neu ko request duoc admob thi se show Woa default
                }
            }
            else // Khong co woa ads server => show ads Admob, neu ko request duoc admob thi se show Woa default
            {
                ShowAdmob();
            }

            void ShowAdmob()
			{
				if (admobControl.HasRewardVideo)
				{
                    PlayerPrefs.SetInt(ConstValue.ShowAdCountKey, showAdsCount + 1);
                    AddNextTimeInter(rv_interval);
                    admobControl.ShowRewardVideo( onSuccess, onClosed);
                }
				else
				{
                    ShowWoa();
                }
			}

            void ShowWoa()
			{
                if (woaAdsControl.ShowRewardVideo( onSuccess, onClosed))
                {
                    PlayerPrefs.SetInt(ConstValue.ShowAdCountKey, showAdsCount + 1);
                    PlayerPrefs.SetInt(ConstValue.ShowWoaAdCountKey, showWoaAdsCount + 1);

                    AddNextTimeInter(rv_interval);
                }
            }
        }
        #endregion


        void AddNextTimeInter(float time)
		{
            nextTimeInter = Time.time + time;
        }
        
        public static void Log(string title, string msg, bool enableLog)
        {
            if (enableLog)
            {
                Debug.Log($"<color=green>Ads log [{title}]:</color> {msg}");
            }
        }

        public static void LogError(string title, string msg)
        {
            Debug.LogError($"<color=red>Ads log [{title}]:</color> {msg}");
        }
    }

    public static class ConstValue
    {
        public const string Banner = "Banner";
        public const string Interstitial = "Interstitial";
        public const string RewardVideo = "RewardVideo";
        public const string RewardInter = "RewardInter";

        // Ad mob
        public const string AdmobAds = "admob";

        // Woa
        public const string WoaAds = "woa_ads";

        public const string Token = "token";

        public const string AdCount = "ad-count"; // show tat ca cac ads
        public const string AdImpression = "ad-impression"; // show WOA ads
        public const string AdClick = "ad-click";

        public const string AdGet = "ad-get";

        // Default asset
        public const string DefaultTitleKey = "default_title_woa";
        public const string DefaultUrlGameKey = "default_url_woa";

        public const string DefaultIconPath = "default_icon.png";
        public const string DefaultBannerPath = "default_banner.png";
        public const string DefaultVideoClipPath = "default_video.mp4";

        // So lan user xem quang cao
        public const string ShowAdCountKey = "show_ads_count";
        // So lan user xem quang cao cheo
        public const string ShowWoaAdCountKey = "show_woa_ads_count";
        // User da click quang cao bao nhieu lan roi
        public const string ClickWoaAdCountKey = "click_woa_ads_count";

        public static string GetVideoClipShortPath(int order)
		{
            return $"temp_video_{order}.mp4";
		}
    }

    [Serializable]
    public enum BannerSizeOp
    {
        SmartBanner,
        Banner_320x50,
        MediumRectangle_300x250,
        IABBanner_468x60,
        Leaderboard_728x90
    }
}
