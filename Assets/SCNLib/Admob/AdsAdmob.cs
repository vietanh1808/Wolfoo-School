using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using DG.Tweening;
namespace SCN.Ads
{
	public class AdsAdmob : MonoBehaviour
	{
		private BannerView bannerView;

		private InterstitialAd inter;

		private RewardedAd rewardVideo;

		private Queue<Action> safeCallback = new Queue<Action>();



		private Action onRewardSuccess;
		private Action onRewardClose;

		private Action onInterSuccess;

		private bool isRequestingBanner;

		private bool isRequestingInter;

		private bool isRequestingRewardVideo;

		private bool waitToShowBanner;

		private bool isBannerShowing;

		private readonly int[] retryTimes = new int[14]
		{
			0,
			2,
			5,
			10,
			20,
			60,
			60,
			120,
			120,
			240,
			240,
			400,
			400,
			600
		};

		protected int retryRewardCoin = 0;
		protected int retryRewardItem = 0;

		protected int retryInterNext = 0;
		protected int retryInterRestart = 0;

		protected int retryBanner = 0;

		private IEnumerator ieWaitInternet;

		public static AdsAdmob Instance
		{
			get;
			private set;
		}

		private bool IsInternetAvailable => (int)Application.internetReachability > 0;

		public float BannerHeight => (bannerView != null) ? bannerView.GetHeightInPixels() : 0f;

		public bool HasInter
		{
			get
			{
				if (inter != null && inter.IsLoaded())
				{
					return true;
				}
				RequestInterstitial();
				return false;
			}
		}
	
		public bool HasRewardVideo
		{
			get
			{
				if (rewardVideo != null && rewardVideo.IsLoaded())
				{
					return true;
				}
				RequestRewardVideo();
				return false;
			}
		}
	
		public event Action OnRewardAdLoaded;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				LogError("", "Initialize multiple times!");
				Destroy(gameObject);
				return;
			}
		
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
			AdmobConfig instance = AdmobConfig.Instance;
			List<String> deviceIds = new List<String>() { AdRequest.TestDeviceSimulator };
			//deviceIds.Add("9E16377F-CEED-408A-9479-C280FD7467E6");
			RequestConfiguration requestConfiguration =
			  new RequestConfiguration.Builder()
			  .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.True)
			  .SetTestDeviceIds(deviceIds).build();
			if (instance.UseRewardAd && string.IsNullOrEmpty(instance.RewardID))
			{
				LogError("RewardedVideo", "slot id was not config!");
			}
			if (instance.UseInterstitialAd && string.IsNullOrEmpty(instance.InterID))
			{
				LogError("Interstitial", "slot id was not config!");
			}
			if (instance.UseBannerAd && string.IsNullOrEmpty(instance.BannerID))
			{
				LogError("Banner", "slot id was not config!");
			}
			Debug.Log((object)"[Ads.Admob] SDK Initializing");
			Log("", $"Start Initializing: appID = {instance.AppID}, useBanner={instance.UseBannerAd}, useInterstitial={instance.UseInterstitialAd}, useRewardVideo={instance.UseRewardAd}");
			MobileAds.SetRequestConfiguration(requestConfiguration);
			MobileAds.Initialize((Action<InitializationStatus>)delegate (InitializationStatus status)
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Invalid comparison between Unknown and I4
				Dictionary<string, AdapterStatus> adapterStatusMap = status.getAdapterStatusMap();
				foreach (KeyValuePair<string, AdapterStatus> item in adapterStatusMap)
				{
					if ((int)item.Value.InitializationState == 1)
					{
						Debug.Log((object)("[Ads.Admob] Adapter: " + item.Key + " initialized."));
					}
					else
					{
						Debug.LogError((object)("[Ads.Admob] Adapter: " + item.Key + " not ready."));
					}
				}
				if (AdmobConfig.Instance.AutoShowBanner)
				{
					RequestBanner();
				}
				RequestRewardVideo();
				RequestInterstitial();
			});
			MobileAds.SetiOSAppPauseOnBackground(true);
		}
        private void Start()
        {
				RequestInterstitial();
		}

		private void Update()
		{
			while (safeCallback.Count > 0)
			{
				Action action = null;
				lock (safeCallback)
				{
					action = safeCallback.Dequeue();
				}
				action?.Invoke();
			}
		}

		private void SafeCallback(Action callback)
		{
			if (callback != null)
			{
				safeCallback.Enqueue(callback);
			}
		}

		private void DelayCallback(float delayTime, Action callback)
		{
			if (callback != null)
			{
				if (delayTime == 0f)
				{
					SafeCallback(callback);
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(IEDelayCallback(delayTime, callback));
				}
			}
		}

		private IEnumerator IEDelayCallback(float delayTime, Action callback)
		{
			yield return (object)new WaitForSecondsRealtime(delayTime);
			callback?.Invoke();
		}

		private void WaitInternet(Action callback)
		{
			if (callback != null)
			{
				((MonoBehaviour)this).StartCoroutine(IEWaitInternet(callback));
			}
		}

		private IEnumerator IEWaitInternet(Action callback)
		{
			if (ieWaitInternet == null)
			{
				ieWaitInternet = (IEnumerator)new WaitUntil((Func<bool>)(() => IsInternetAvailable));
			}
			yield return ieWaitInternet;
			callback?.Invoke();
		}

		private AdRequest CreateAdRequest()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return new AdRequest.Builder()
		.Build();
		}

		private void Log(string adType, string msg)
		{
			if (AdmobConfig.Instance.EnableLog)
			{
				Debug.Log((object)("[Ads.Admob." + adType + "] " + msg));
			}
		}

		private void LogError(string adType, string msg)
		{
			Debug.LogError((object)("[Ads.Admob." + adType + "] " + msg));
		}

		private void RequestBanner()
		{
			if (!AdmobConfig.Instance.UseBannerAd || isRequestingBanner)
			{
				return;
			}
			if (retryBanner >= retryTimes.Length)
			{
				retryBanner = retryTimes[retryTimes.Length - 1];
			}
			int num = retryTimes[retryBanner];
			isRequestingBanner = true;
			Log("Banner", $"Will Request after {num}s, retry={retryBanner}");
			DelayCallback(num, delegate
			{
				if (IsInternetAvailable)
				{
					DoRequestBanner();
				}
				else
				{
					LogError("Banner", "Request: Waiting for internet...");
					WaitInternet(DoRequestBanner);
				}
			});
		}

		private void DoRequestBanner()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			Log("Banner", "Request starting...");
			DestroyBanner();
			bannerView = new BannerView(AdmobConfig.Instance.BannerID, GetAdsize(), (AdPosition)(AdmobConfig.Instance.ShowBannerOnBottom ? 1 : 0));
			bannerView.OnAdLoaded +=OnBannerAdLoaded;
			bannerView.OnAdFailedToLoad+= OnBannerAdFailedToLoad;
			bannerView.OnAdOpening+= OnBannerAdOpened;
			bannerView.OnAdClosed+= OnBannerAdClosed;
			bannerView.LoadAd(CreateAdRequest());
		}

		private AdSize GetAdsize()
		{
			var configBanner = AdmobConfig.Instance.BannerSize;
            switch (configBanner)
            {
				case BannerSize.SmartBanner:
				return AdSize.SmartBanner;
					break;
				case BannerSize.Banner_320x50:
					return AdSize.Banner;
					break;
				case BannerSize.IABBanner_468x60:
					return AdSize.IABBanner;
					break;
				case BannerSize.Leaderboard_728x90:
					return AdSize.Leaderboard;
					break;
				default: return AdSize.SmartBanner;
			}
		}

		public void ShowBanner()
		{
			if (!isBannerShowing)
			{
				if (bannerView != null)
				{
					waitToShowBanner = false;
					isBannerShowing = true;
					bannerView.Show();
					Log("Banner", "Show Start.");
				}
				else
				{
					waitToShowBanner = true;
					RequestBanner();
				}
			}
		}

		public void HideBanner()
		{
			if (bannerView != null)
			{
				waitToShowBanner = false;
				isBannerShowing = false;
				bannerView.Hide();
			}
		}

		public void DestroyBanner()
		{
			if (bannerView != null)
			{
				waitToShowBanner = false;
				isBannerShowing = false;
				bannerView.Destroy();
				bannerView = null;
			}
		}

		private void RequestInterstitial()
		{
			if (!AdmobConfig.Instance.UseInterstitialAd || isRequestingInter)
			{
				return;
			}
			if (retryInterNext >= retryTimes.Length)
			{
				retryInterNext = retryTimes[retryTimes.Length - 1];
			}
			int num = retryTimes[retryInterNext];
			isRequestingInter = true;
			Log("Interstitial", $"Will Request after {num}s, retry={retryInterNext}");
			DelayCallback(num, delegate
			{
				if (IsInternetAvailable)
				{
					DoRequestInterstitial();
				}
				else
				{
					LogError("Interstitial", "Request: Waiting for internet...");
					WaitInternet(DoRequestInterstitial);
				}
			});
		}
		

		private void DoRequestInterstitial()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			Log("Interstitial", "Request starting...");
			//DestroyInter();
			inter = new InterstitialAd(AdmobConfig.Instance.InterID);
			inter.OnAdLoaded += OnInterLoaded;
			inter.OnAdFailedToLoad += OnInterFailedToLoad;
			inter.OnAdOpening += OnInterstitialAdOpened;
			inter.OnAdClosed += OnInterClosed;
			inter.LoadAd(CreateAdRequest());
		}

		public void ShowInterstitial(Action callback = null)
		{
			if (HasInter)
			{
				onInterSuccess = callback;
				Log("Interstitial", "Show start..");
				inter.Show();
			}
			else
			{
				Log("Interstitial", "Show failed: ad not ready. Invoke callback.");
				callback?.Invoke();
				RequestInterstitial();
			}
		}
		public void DestroyInter()
		{
			if (inter != null)
			{
				inter.Destroy();
				inter = null;
			}
		}
   
        #region RewardVideo
        private void RequestRewardVideo()
        {
            if (!AdmobConfig.Instance.UseRewardAd || isRequestingRewardVideo)
            {
                return;
            }
            if (retryRewardItem >= retryTimes.Length)
            {
				retryRewardItem = retryTimes[retryTimes.Length - 1];
            }
            int num = retryTimes[retryRewardItem];
			isRequestingRewardVideo = true;
            Log("RewardedVideo", $"Request after {num}s, retry={retryRewardItem}");
            DelayCallback(num, delegate
            {
                if (IsInternetAvailable)
                {
                    DoRequestReward();
                }
                else
                {
                    LogError("RewardedVideo", "Request: Waiting for internet...");
                    WaitInternet(DoRequestReward);
                }
            });
        }

        private void DoRequestReward()
        {
            //IL_001d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Expected O, but got Unknown
            Log("RewardedVideo", "Request starting...");
            rewardVideo = new RewardedAd(AdmobConfig.Instance.RewardID);
			rewardVideo.OnAdLoaded += OnRewardLoaded;
			rewardVideo.OnAdOpening += OnRewardOpening;
			rewardVideo.OnAdFailedToShow += OnRewardFailedToShow;
			rewardVideo.OnAdClosed += OnRewardClosed;
			rewardVideo.OnUserEarnedReward += OnRewardEarnedReward;
			rewardVideo.LoadAd(CreateAdRequest());
        }

        public void ShowRewardVideo(Action onSuccess, Action onClosed = null)
        {
            if (HasRewardVideo)
            {
				onRewardSuccess = onSuccess;
				onRewardClose = onClosed;
                Log("RewardedVideo", "Show start...");
				rewardVideo.Show();
            }
            else
            {
                Log("RewardedVideo", "Show failed: ad not ready. Invoke onClosed callback.");
                onClosed?.Invoke();
				RequestRewardVideo();
            }
        }
        private void OnRewardLoaded(object sender, EventArgs args)
        {
            Log("RewardedVideo", "OnAdLoaded.");
			retryRewardItem = 0;
			isRequestingRewardVideo = false;
            SafeCallback(this.OnRewardAdLoaded);
        }

        private void OnRewardFailedToLoad(object sender, AdErrorEventArgs args)
        {
            Debug.LogError("RewardedVideo" + "OnAdFailedToLoad: " + args.Message);
			isRequestingRewardVideo = false;
            if (AdmobConfig.Instance.RequestOnLoadFailed)
            {
				retryRewardItem++;
				RequestRewardVideo();
            }
        }

        private void OnRewardOpening(object sender, EventArgs args)
        {
            Log("RewardedVideo", "OnAdOpening...");
        }

        private void OnRewardFailedToShow(object sender, AdErrorEventArgs args)
        {
            LogError("RewardedVideo", "OnAdFailedToShow: " + args.Message + ".");
            SafeCallback(onRewardClose);
			RequestRewardVideo();
        }

        private void OnRewardClosed(object sender, EventArgs args)
        {
            Log("RewardedVideo", "OnAdClosed.");
            SafeCallback(onRewardClose);
			RequestRewardVideo();
        }

        private void OnRewardEarnedReward(object sender, Reward args)
        {
            Log("RewardedVideo", "OnEarnedReward successfully.");
            SafeCallback(onRewardSuccess);
        }
        #endregion
        #region InterBanner
        private void OnBannerAdLoaded(object sender, EventArgs args)
		{
			Log("Banner", "OnAdLoaded.");
			retryBanner = 0;
			isRequestingBanner = false;
			if (AdmobConfig.Instance.AutoShowBanner || waitToShowBanner)
			{
				ShowBanner();
			}
		}

		private void OnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
			LogError("Banner", "OnAdFailedToLoad: " + args.LoadAdError + ".");
			isRequestingBanner = false;
			retryBanner++;
			RequestBanner();
		}

		private void OnBannerAdOpened(object sender, EventArgs args)
		{
			Log("Banner", "OnAdOpened...");
		}

		private void OnBannerAdClosed(object sender, EventArgs args)
		{
			Log("Banner", "OnAdClosed.");
			isBannerShowing = false;
			if (AdmobConfig.Instance.AutoShowBanner)
			{
				RequestBanner();
			}
		}

		private void OnBannerAdLeftApplication(object sender, EventArgs args)
		{
			Log("Banner", "OnAdLeftApplication.");
		}

		private void OnInterLoaded(object sender, EventArgs args)
		{
			Log("Interstitial", "OnAdLoaded.");
			retryInterNext = 0;
			isRequestingInter = false;
		}


		private void OnInterFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
			LogError("Interstitial","OnAdFailedToLoad: " + args.LoadAdError + ".");
			isRequestingInter = false;
			if (AdmobConfig.Instance.RequestOnLoadFailed)
			{
				retryInterNext++;
				RequestInterstitial();
			}
		}
	
		private void OnInterstitialAdOpened(object sender, EventArgs args)
		{
			Log("Interstitial", "OnAdOpened...");
		}

		private void OnInterClosed(object sender, EventArgs args)
		{
			Time.timeScale = 1;
			Log("Interstitial", "OnAdClosed.");
			SafeCallback(onInterSuccess);
			RequestInterstitial();
		}
		private void OnInterstitialAdLeftApplication(object sender, EventArgs args)
		{
			Log("Interstitial", "OnAdLeftApplication.");
		}
        #endregion

    }
}

