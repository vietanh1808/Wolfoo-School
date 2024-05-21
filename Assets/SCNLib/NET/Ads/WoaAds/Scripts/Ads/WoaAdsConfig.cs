using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.Ads
{
	[CreateAssetMenu(fileName = AssetName, menuName = "SCN/Scriptable Objects/WoaAds Config")]
	public class WoaAdsConfig : ScriptableObject
	{
		public const string AssetName = "WoaAds config";

		[SerializeField] bool enableLog = false;
		[SerializeField] bool useDevServer = false;
		[SerializeField] bool sendTrackingWhileEditor = false;

		[Header("BLOCK REQUEST")]
		[SerializeField] bool isBlockAds;

		[SerializeField] bool useInterstitialAd = false;
		[SerializeField] bool useRewardVideoAd = false;

		[Header("Config")]
		[Tooltip("ID store tren WOA ads")]
		[SerializeField] int gameStoreIdAndroid;
		[Tooltip("ID store tren WOA ads")]
		[SerializeField] int gameStoreIdIos;
		[SerializeField] string apiKey;

		public bool EnableLog => enableLog;
		public bool UseInterstitialAd => useInterstitialAd;
		public bool UseRewardVideoAd => useRewardVideoAd;

		public bool IsBlockAds => isBlockAds;

		public int GameStoreId
		{
			get
			{
				if (Master.IsAndroid)
				{
					return gameStoreIdAndroid;
				}
				else
				{
					return gameStoreIdIos;
				}
			}
		}
		public string ApiKey => apiKey;
		
		public const string URL_Dev = "https://dev.woa.network/api/v1";
		public const string URL_Main = "https://ad.woa.network/api/v1";

		public string URL
		{
			get => useDevServer ? URL_Dev : URL_Main;
		}

		public bool SendTrackingWhileEditor => sendTrackingWhileEditor;
	}
}