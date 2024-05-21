using SCN.Ads.Format;
using SCN.Common;
using SCN.FirebaseLib.FCM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace SCN.Ads
{
    public class WoaAdsManager : SafeCallBack
    {
        [SerializeField] AdGetData adsStats;
        Action updateAdsAssets;
        static WoaAdsConfig config;
        public static WoaAdsConfig Config
        {
            get
            {
                if (config == null)
                {
                    config = Resources.Load<WoaAdsConfig>(WoaAdsConfig.AssetName);
                }

                return config;
            }
        }

        [SerializeField] AdAsset[] listAdAssets;
        [SerializeField] AdAsset defaultAssets;

        InterstitialsAdsFormat interAds;
        VideoRewardAdsFormat videoRewardAds;

        bool isInforAdsAble;
        bool isRequestingInfor;

        /// <summary>
        /// Able neu co it nhat 1 Assets duoc tai ve tu server
        /// </summary>
        bool isAdsAssetsAble;
        bool isRequestingAdsAssets;

        /// <summary>
        /// Able neu co default asset
        /// </summary>
        bool isDefaultAssetsAble;

        public bool IsDefaultAssetsAble => isDefaultAssetsAble;

        public AdGetData AdsStats => adsStats;
        public bool IsInforAdsAble => isInforAdsAble;

        public bool HasAdsFromServer
        {
			get
			{
                // Co the show duoc ads khi khong bi block ads
                // va thoa man 1 trong 2 dieu kien: tai duoc assets hoac co asset default
                if (!Config.IsBlockAds && IsAdsAssetsAble)
				{
                    return true;
				}

                return false;
			}
        }

        /// <summary>
        /// Co ads tai duoc tu Server hoac ads duoc luu san
        /// </summary>
        public bool HasAds => IsDefaultAssetsAble || HasAdsFromServer;

		public InterstitialsAdsFormat InterAds
        {
            get
            {
                if (interAds == null)
                {
                    GameObject prefab;
					if (Master.IsPortrait)
					{
                        prefab = LoadSource.LoadObject<GameObject>("Interstitials ads portrait");
                    }
					else
					{
                        prefab = LoadSource.LoadObject<GameObject>("Interstitials ads landscape");
                    }

                    interAds = Instantiate(prefab, transform).GetComponent<InterstitialsAdsFormat>();
                }

                return interAds;
            }
        }
		public VideoRewardAdsFormat VideoRewardAds 
        {
            get 
            {
                if (videoRewardAds == null)
                {
                    GameObject prefab;
                    if (Master.IsPortrait)
					{
                        prefab = LoadSource.LoadObject<GameObject>("Video reward ads portrait");
                    }
					else
					{
                        prefab = LoadSource.LoadObject<GameObject>("Video reward ads landscape");
                    }

                    videoRewardAds = Instantiate(prefab, transform).GetComponent<VideoRewardAdsFormat>();
                }

                return videoRewardAds;
            }
        }

        public AdAsset[] ListAdAssets { get => listAdAssets; set => listAdAssets = value; }
        public bool IsAdsAssetsAble { get => isAdsAssetsAble; set => isAdsAssetsAble = value; }
        public Action UpdateAdsAssets { get => updateAdsAssets; set => updateAdsAssets = value; }

        public void Setup()
        {
            if (Config == null)
            {
                Debug.LogWarning("Not found: Woa Ads config");
                return;
            }
            if (Config.IsBlockAds)
            {
                return;
            }

            AdsManager.Log(ConstValue.WoaAds, WoaAdsNetworking.IsSendTracking ?
                "Send data to WOA server" : "Not send data to WOA server", Config.EnableLog);

            LoadDefaultAssets();

            AdsManager.Log(ConstValue.WoaAds, "Getting token ...", Config.EnableLog);
            FirebaseMessageManager.Instance.CheckAndGetToken(token =>
            {
                AdsManager.Log(ConstValue.WoaAds, $"Get token success: {token}", Config.EnableLog);
                RequestInforAndDownloadAdsAssets();
            });
        }

        void RequestInforAndDownloadAdsAssets()
		{
			if (!isInforAdsAble)
			{
                RequestAdsInfor(RequestAdsAssets);
            }
			else
			{
                RequestAdsAssets();
			}
		}

        /// <summary>
        /// Update user va yeu cau lay thong tin cua cac Game lien quan
        /// </summary>
        void RequestAdsInfor(Action OnComplete)
        {
			if (isRequestingInfor || isInforAdsAble)
			{
                return;
			}

            AdsManager.Log(ConstValue.WoaAds, "Updating user data ...", Config.EnableLog);
            isRequestingInfor = true;

            // Update user data
            WoaAdsNetworking.Init(this, Config.EnableLog, (success, st) =>
            {
                if (success)
				{
                    AdsManager.Log(ConstValue.WoaAds, "Updated user data => Requesting ads infor", Config.EnableLog);

                    // Download Ads Infor
                    WoaAdsNetworking.PostData(ConstValue.AdGet, (success, st) =>
                    {
                        if (success)
						{
                            AdsManager.Log(ConstValue.WoaAds, $"Requested ads infor", Config.EnableLog);
                            isInforAdsAble = true;

                            adsStats = JsonUtility.FromJson<AdGetData>(st);
                            OnComplete?.Invoke();
                        }
						else
						{
                            AdsManager.Log(ConstValue.WoaAds, $"Requesting ads infor fail: {st}", Config.EnableLog);
                            isRequestingInfor = false;
                        }
                    });
                }
				else
				{
                    AdsManager.Log(ConstValue.WoaAds, $"Updating user data fail: {st}", Config.EnableLog);
                    isRequestingInfor = false;
				}
            });
        }

        /// <summary>
        /// Yeu cau lay Asset cua cac Game lien quan tren Server
        /// </summary>
        void RequestAdsAssets()
        {
            if (isRequestingAdsAssets || IsAdsAssetsAble)
            {
                return;
            }

            AdsManager.Log(ConstValue.WoaAds, "Requesting ads assets ...", Config.EnableLog);
            isRequestingAdsAssets = true;

            if (ListAdAssets == null)
            {
                ListAdAssets = new AdAsset[adsStats.Data.Length];
            }

            for (int i = 0; i < ListAdAssets.Length; i++)
            {
                var index = i;
                if (ListAdAssets[i] == null)
                {
                    ListAdAssets[i] = new AdAsset(adsStats.Data[i]
                        , ConstValue.GetVideoClipShortPath(i), this, Config.EnableLog)
                    {
                        OnAble = () => 
                        {
                            AdsManager.Log(ConstValue.WoaAds
                                , $"Request ads assets success: {index}", Config.EnableLog);
                            if (IsAdsAssetsAble)
                            {
                                return;
                            }
                            IsAdsAssetsAble = true;
                            OverrideDefaultAssets(ListAdAssets[index], index);
                            UpdateAdsAssets?.Invoke();
                        }
                    };
                }

                if (ListAdAssets[i].IsDownloading)
                {
                    continue;
                }

                ListAdAssets[i].RequestDownload();
            }
        }

        void LoadDefaultAssets()
		{
            Sprite icon = null;
            Sprite banner = null;
            string urlVideoClip = string.Empty;
            string title = string.Empty;
            string urlGame = string.Empty;

            bool loadFail = false;

            if (FileLib.CheckFileOnDisk(ConstValue.DefaultIconPath))
            {
                icon = FileLib.ConvertBytesToSprite(
                    FileLib.LoadFileOnDisk(ConstValue.DefaultIconPath));
            }
            else
            {
                AdsManager.Log(ConstValue.WoaAds, "Load default asset fail: Icon", Config.EnableLog);
                loadFail = true;
            }

            if (FileLib.CheckFileOnDisk(ConstValue.DefaultBannerPath))
            {
                banner = FileLib.ConvertBytesToSprite(
                    FileLib.LoadFileOnDisk(ConstValue.DefaultBannerPath));
            }
            else
            {
                AdsManager.Log(ConstValue.WoaAds, "Load default asset fail: Banner", Config.EnableLog);
                loadFail = true;
            }

            if (FileLib.CheckFileOnDisk(ConstValue.DefaultVideoClipPath))
            {
                urlVideoClip = FileLib.ConvertShortPathToFullPath(ConstValue.DefaultVideoClipPath);
            }
            else
            {
                AdsManager.Log(ConstValue.WoaAds, "Load default asset fail: Video", Config.EnableLog);
                loadFail = true;
            }

            if (PlayerPrefs.HasKey(ConstValue.DefaultTitleKey))
			{
                title = PlayerPrefs.GetString(ConstValue.DefaultTitleKey);
			}
			else
			{
                AdsManager.Log(ConstValue.WoaAds, "Load default asset fail: Title", Config.EnableLog);
                loadFail = true;
            }

            if (PlayerPrefs.HasKey(ConstValue.DefaultUrlGameKey))
            {
                urlGame = PlayerPrefs.GetString(ConstValue.DefaultUrlGameKey);
            }
            else
            {
                AdsManager.Log(ConstValue.WoaAds, "Load default asset fail: Url install", Config.EnableLog);
                loadFail = true;
            }

			if (loadFail)
			{
                return;
			}

            defaultAssets = new AdAsset(icon, banner, urlVideoClip, title, urlGame);
            isDefaultAssetsAble = true;
            AdsManager.Log(ConstValue.WoaAds, "Load default asset success", Config.EnableLog);
        }

        void OverrideDefaultAssets(AdAsset adAsset, int order)
		{
            AdsManager.Log(ConstValue.WoaAds, "Override default assets", Config.EnableLog);
            for (int i = 0; i < ListAdAssets.Length; i++)
			{
				if (ListAdAssets[i].Able)
				{
                    PlayerPrefs.SetString(ConstValue.DefaultTitleKey, adAsset.NameGame);
                    PlayerPrefs.SetString(ConstValue.DefaultUrlGameKey, adAsset.UrlGame);

                    FileLib.SaveFileOnDisk(ConstValue.DefaultIconPath, FileLib.ConvertSpriteToBytes(adAsset.Icon));
                    FileLib.SaveFileOnDisk(ConstValue.DefaultBannerPath, FileLib.ConvertSpriteToBytes(adAsset.Banner));

                    if (FileLib.CheckFileOnDisk(ConstValue.DefaultVideoClipPath)) // Neu da co video default, thi xoa video do di
					{
                        FileLib.DeleteFile(ConstValue.DefaultVideoClipPath);
					}
                    FileLib.CopyFile(ConstValue.GetVideoClipShortPath(order), ConstValue.DefaultVideoClipPath); // Save video

                    return;
				}
			}
		}

#region Show ads
		public bool ShowInterstitial( Action<bool> onShowAdFinished = null)
        {
            if (Config.IsBlockAds)
            {
                onShowAdFinished?.Invoke(false);
                return false;
            }

            AdAsset asset = GetAdAsset();
            if (asset != null)
			{
                AdsManager.Log(ConstValue.Interstitial, "Show start..", Config.EnableLog);
                InterAds.Show(asset, () =>
                {
                    onShowAdFinished?.Invoke(true);
                });

                return true;
            }
			else
			{
                AdsManager.Log(ConstValue.Interstitial
                    , "Show failed: ad not ready. Invoke callback.", Config.EnableLog);
                onShowAdFinished?.Invoke(false);
                RequestInforAndDownloadAdsAssets();

                return false;
            }
        }

        public bool ShowRewardVideo( Action onSuccess, Action onClosed = null)
		{
            if (Config.IsBlockAds)
            {
                onSuccess?.Invoke();
                return false;
            }

            AdAsset asset = GetAdAsset();
            if (asset != null)
			{
                AdsManager.Log(ConstValue.RewardVideo, "Show start...", Config.EnableLog);
                VideoRewardAds.Show( asset, b =>
                {
                    if (b)
                    {
                        onSuccess?.Invoke();
                    }
                    onClosed?.Invoke();
                });

                return true;
            }
            else
            {
                AdsManager.Log(ConstValue.RewardVideo
                    , "Show failed: ad not ready. Invoke onClosed callback.", Config.EnableLog);
                onClosed?.Invoke();
                RequestInforAndDownloadAdsAssets();

                return false;
            }
        }
#endregion

        AdAsset GetAdAsset()
        {
            if (IsAdsAssetsAble) // Khi co it nhat 1 Asset duoc download tu tren Server ve thanh cong
            {
                var listAdsIndex = new List<int>();
                for (int i = 0; i < ListAdAssets.Length; i++)
                {
                    if (ListAdAssets[i].Able)
                    {
                        listAdsIndex.Add(i);
                    }
                }

                if (listAdsIndex.Count == 0) // Bug
                {
                    return GetDefaultAssetIfHas();
                }
				else // Tra ve random 1 trong so cac Asset duoc download ve
				{
                    return ListAdAssets[RandomMaster.RandomInList(listAdsIndex)];
                }
            }

            return GetDefaultAssetIfHas();
        }

        AdAsset GetDefaultAssetIfHas()
        {
            if (isDefaultAssetsAble) // Tra ve Asset default neu khong lay duoc tu Server, dong thoi Request tai lai
            {
                RequestInforAndDownloadAdsAssets();
                return defaultAssets;
            }
            else // Khong co Asset nao de tra ve nua
            {
                return null;
            }
        }

        /// <summary>
        /// Infor ads lay duoc tren Server
        /// </summary>
        [System.Serializable]
        public class AdGetData
        {
            [SerializeField] string code;
            [SerializeField] int ad_count;

            [SerializeField] AdsInfor[] data;

            public string Code => code;
            public int Ad_count => ad_count;
            public AdsInfor[] Data => data;

			[System.Serializable]
            public class AdsInfor
            {
                [SerializeField] int id;
                [SerializeField] int ad_game_store_id;
                [SerializeField] string icon;
                [SerializeField] string title;
                [SerializeField] string @short;
                [SerializeField] string video_url;
                [SerializeField] string website;
                [SerializeField] string banner;
                [SerializeField] string install_url;

				public int Id => id;
				public int Ad_game_store_id => ad_game_store_id;
				public string Icon => icon;
				public string Title => title;
				public string Short => @short;
				public string Video_url => video_url;
				public string Website => website;
				public string Banner => banner;
				public string Install_url => install_url;
			}
        }

        /// <summary>
        /// Asset de hien thi ads trong Game
        /// </summary>
        [System.Serializable]
        public class AdAsset
		{
            public Action OnAble;

            [SerializeField] Sprite icon;
            [SerializeField] Sprite banner;
            [SerializeField] string fullPathVideoClip;

			readonly DownloadTex downloadIcon;
            readonly DownloadTex downloadBanner;
            readonly DownloadVideoClip downloadVideoClip;

            [SerializeField] string nameGame;
            [SerializeField] string urlGame;

            [SerializeField] bool able;
			readonly bool[] checks = new bool[3];

            [SerializeField] bool isDownloading;

            public Sprite Icon => icon;
			public Sprite Banner  => banner;
            public string FullPathVideoClip => fullPathVideoClip;
            public string NameGame => nameGame;
            public string UrlGame => urlGame;

            public bool Able => able;
            public bool IsDownloading => isDownloading;

            /// <summary>
            /// Dung de khoi tao Asset lay tu server
            /// </summary>
            public AdAsset(AdGetData.AdsInfor data, string videoShortPath
                , SafeCallBack safeCallBackControl, bool enableLog)
			{
                nameGame = data.Title;
                urlGame = data.Install_url;

                downloadIcon = new DownloadTex(data.Icon, safeCallBackControl, enableLog, "icon");
                downloadBanner = new DownloadTex(data.Banner, safeCallBackControl, enableLog, "banner");
                downloadVideoClip = new DownloadVideoClip(data.Video_url, videoShortPath
                    , safeCallBackControl, enableLog, "video clip");

                downloadIcon.OnComplete = sp =>
                {
                    SetIcon(sp);
                };
                downloadBanner.OnComplete = sp =>
                {
                    SetBanner(sp);
                };
                downloadVideoClip.OnComplete = fullPath =>
                {
                    SetUrlVideoClip(fullPath);
                };
			}

            /// <summary>
            /// Dung de khoi tao Asset lay tu default
            /// </summary>
            public AdAsset(Sprite icon, Sprite banner, string fullPathVideoClip
                , string nameGame, string urlGame)
			{
                this.icon = icon;
                this.banner = banner;
                this.fullPathVideoClip = fullPathVideoClip;
                this.nameGame = nameGame;
                this.urlGame = urlGame;

                able = true;
			}

            public void RequestDownload()
			{
                isDownloading = true;

                downloadIcon.RequestDownload();
                downloadBanner.RequestDownload();
                downloadVideoClip.RequestDownload();
            }

            void CheckAbleAll()
            {
                for (int i = 0; i < checks.Length; i++)
                {
                    if (!checks[i])
                    {
                        return;
                    }
                }

                able = true;
                isDownloading = false;
                OnAble?.Invoke();
            }

#region Set value
            void SetIcon(Sprite icon)
			{
                this.icon = icon;
                checks[0] = true;
                CheckAbleAll();
            }

            void SetBanner(Sprite banner)
			{
                this.banner = banner;
                checks[1] = true;
                CheckAbleAll();

            }

            void SetUrlVideoClip(string urlVideoClip)
            {
                this.fullPathVideoClip = urlVideoClip;
                checks[2] = true;
                CheckAbleAll();
            }
#endregion

            public class DownloadTex : DownloadAssets
            {
                public Action<Sprite> OnComplete;
				readonly string url;

                public DownloadTex(string url, SafeCallBack safeCallBackControl, bool enableLog, string assetName)
                    : base(safeCallBackControl, enableLog, assetName)
                {
                    this.url = url;
                }

				protected override void DoRequestDownload()
				{
                    FileLib.DownloadTexture(safeCallBackControl, url, (b, sp) =>
                    {
						if (b)
						{
                            OnComplete?.Invoke(sp);
                            OnDownloadSuccess();
                        }
						else
						{
                            OnDownloadFail();
						}
                    });
                }
			}

            public class DownloadVideoClip : DownloadAssets
			{
                public System.Action<string> OnComplete;
				readonly string url;
				readonly string shortPath;

                public DownloadVideoClip(string url, string shortPath
                    , SafeCallBack safeCallBackControl, bool enableLog, string assetName)
                    : base(safeCallBackControl, enableLog, assetName)
                {
                    this.url = url;
                    this.shortPath = shortPath;
                }

				protected override void DoRequestDownload()
				{
                    FileLib.DownloadAndSaveVideo(safeCallBackControl, url, shortPath, (b, vc) =>
                    {
                        if (b)
                        {
                            OnComplete?.Invoke(vc);
                            OnDownloadSuccess();
                        }
                        else
                        {
                            OnDownloadFail();
                        }
                    });
                }
			}

            public class DownloadAssets
			{
                public readonly static int[] retryTimes = new int[14]
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

                bool isDownloaded;
                bool isRequesting;
                int retryTurn = 0;
				readonly string assetName;

				readonly bool enableLog;
                protected readonly SafeCallBack safeCallBackControl;

                public bool IsDownloaded => isDownloaded;

                public DownloadAssets(SafeCallBack safeCallBackControl, bool enableLog, string assetName)
				{
                    this.safeCallBackControl = safeCallBackControl;
                    this.enableLog = enableLog;
                    this.assetName = assetName;
				}

                /// <summary>
                /// Yeu cau download asset
                /// </summary>
                public void RequestDownload()
                {
                    if (isRequesting)
                    {
                        return;
                    }
                    if (retryTurn >= retryTimes.Length)
                    {
                        retryTurn = retryTimes.Length - 1;
                    }
                    int num = retryTimes[retryTurn];
                    isRequesting = true;
                    AdsManager.Log(ConstValue.WoaAds
                        , $"Will Request after {num}s, retry={retryTurn}, name={assetName}", enableLog);
                    safeCallBackControl.DelayCallback(num, delegate
                    {
                        if (InternetManager.IsInternetAvailable)
                        {
                            DoRequestDownload();
                        }
                        else
                        {
                            AdsManager.LogError(ConstValue.WoaAds, $"Request {assetName}: Waiting for internet...");
                            InternetManager.WaitInternet(safeCallBackControl, DoRequestDownload);
                        }
                    });
                }

                /// <summary>
                /// Thuc hien download asset
                /// </summary>
                protected virtual void DoRequestDownload()
                {

                }

                /// <summary>
                /// Khi download thanh cong
                /// </summary>
                protected void OnDownloadSuccess()
                {
                    AdsManager.Log(ConstValue.WoaAds, $"Download success: {assetName}", enableLog);
                    retryTurn = 0;

                    isRequesting = false;
                    isDownloaded = true;
                }

                /// <summary>
                /// Khi download that bai
                /// </summary>
                protected void OnDownloadFail()
                {
                    AdsManager.LogError(ConstValue.WoaAds, $"Download fail: {assetName}");
                    isRequesting = false;

                    retryTurn++;
                    RequestDownload();
                }
            }
        }
    }
}