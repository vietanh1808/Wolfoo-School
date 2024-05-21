using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using SCN.Common;
using SCN.FirebaseLib.FCM;
using Newtonsoft.Json.Linq;

namespace SCN.Ads
{
    public static class WoaAdsNetworking
    {
        static MonoBehaviour mono;

        static bool enableLog;

        static DataPost dataPost;
        static bool isDataPostable;

        static Coroutine waitPostDataCorou;

        /// <summary>
        /// Mac dinh gui tracking tren thiet bi
        /// </summary>
        public static bool IsSendTracking
        {
            get
            {
                if (!Application.isEditor)
                {
                    return true;
                }
                else
                {
                    return WoaAdsManager.Config.SendTrackingWhileEditor;
                }
            }
        }

        public static void Init(MonoBehaviour mono, bool enableLog, System.Action<bool, string> onComplete)
		{
            WoaAdsNetworking.mono = mono;
            WoaAdsNetworking.enableLog = enableLog;

            PostData(ConstValue.Token, onComplete);
        }

		public static void PostData(string cmd, System.Action<bool, string> onComplete)
        {
            if (cmd != ConstValue.AdGet && !IsSendTracking)
            {
                onComplete?.Invoke(true, "No send tracking");
                return;
            }

            if (waitPostDataCorou != null)
            {
                AdsManager.Instance.WoaAdsControl.StopCoroutine(waitPostDataCorou);
            }

            waitPostDataCorou = AdsManager.Instance.WoaAdsControl.StartCoroutine(WaitPostData(() =>
            {
                string data = JsonUtility.ToJson(dataPost);
                mono.StartCoroutine(RequestServer(cmd, RequestMethob.POST, data
                    , WoaAdsManager.Config.ApiKey, onComplete));
            }));
        }

        static IEnumerator WaitPostData(System.Action onComplete)
		{
			if (!isDataPostable)
			{
                FileLib.DownloadText(AdsManager.Instance.WoaAdsControl, "http://ip-api.com/json/", (b, str) =>
                {
                    AdsManager.Log(ConstValue.WoaAds, "Get ip: " + (b ? "Success" : "Fail"), WoaAdsManager.Config.EnableLog);
					if (b)
					{
                        var ipUser = JsonTool.DeserializeObject<IpUser>(str);
                        dataPost = new DataPost(ipUser.CountryCode, ipUser.Region);
                    }
					else
					{
                        dataPost = new DataPost("Unknow", "Unknow");
					}

                    isDataPostable = true;
                });

                yield return new WaitUntil(() => isDataPostable);
                onComplete?.Invoke();
            }
			else
			{
                onComplete?.Invoke();
			}
		}

        static IEnumerator RequestServer(string cmd, RequestMethob requestMethob
            , string data, string header, System.Action<bool, string> onComplete)
        {
            AdsManager.Log($"Request-{cmd}", data, WoaAdsManager.Config.EnableLog);
            var urlRequest = WoaAdsManager.Config.URL + "/" + cmd + "/" + "?access_token=" + header;
            var www = new UnityWebRequest(urlRequest, requestMethob.ToString());

            AdsManager.Log("URL", urlRequest, WoaAdsManager.Config.EnableLog);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            var downloadedText = www.downloadHandler.text;
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                AdsManager.LogError(ConstValue.WoaAds, www.error);
                onComplete?.Invoke(false, www.error);
            }
            else if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                AdsManager.LogError(ConstValue.WoaAds, www.downloadHandler.text);
                onComplete?.Invoke(false, downloadedText);
            }
            else
            {
                AdsManager.Log(ConstValue.WoaAds, downloadedText, enableLog);

                JObject obj = JObject.Parse(downloadedText);
                var code = (int)obj["code"];

                if (code == 200)
                {
                    onComplete?.Invoke(true, downloadedText);
                }
                else
                {
                    onComplete?.Invoke(false, downloadedText);
                }
            }
        }

        [System.Serializable]
        public class IpUser
		{
            [SerializeField] string status;
            [SerializeField] string country;
            [SerializeField] string countryCode;
            [SerializeField] string region;
            [SerializeField] string regionName;
            [SerializeField] string city;
            [SerializeField] string zip;
            [SerializeField] float lat;
            [SerializeField] float lon;
            [SerializeField] string timezone;
            [SerializeField] string isp;
            [SerializeField] string org;
            [SerializeField] string @as;
            [SerializeField] string query;

			public string Status => status;
			public string Country => country;
			public string CountryCode => countryCode;
			public string Region => region;
			public string RegionName => regionName;
			public string City => city;
			public string Zip => zip;
			public float Lat => lat;
			public float Lon => lon;
			public string Timezone => timezone;
			public string Isp => isp;
			public string Org => org;
			public string As => @as; 
			public string Query => query;
		}

        [System.Serializable]
        public class DataPost
        {
            [SerializeField] string device_id;
            [SerializeField] string email;
            [SerializeField] string country_code;
            [SerializeField] string region_code;
            [SerializeField] string year_of_birth;
            [SerializeField] string gender;
            [SerializeField] string device_type;
            [SerializeField] string brand;
            [SerializeField] string manufacturer;
            [SerializeField] string os_system;
            [SerializeField] string os_version;
            [SerializeField] string screen_resolution;
            [SerializeField] string game_store_id;
            [SerializeField] string fb_token;
            [SerializeField] string fb_topic;

			public DataPost(string country_code, string region_code)
			{
				Device_id = SystemInfo.deviceUniqueIdentifier;
				Email = "user@company.com";
				Country_code = country_code;
				Region_code = region_code;
				Year_of_birth = "2020";
				Gender = "m";
				Device_type = SystemInfo.deviceType.ToString();
				Brand = "Unknow";
				Manufacturer = "Unknow";
				Os_system = SystemInfo.operatingSystem;
				Os_version = "Unknow";
				Screen_resolution = $"{Screen.width}x{Screen.height}";
				Game_store_id = WoaAdsManager.Config.GameStoreId.ToString();
				Fb_token = FirebaseMessageManager.Instance.UserToken;
				Fb_topic = "Game";
			}
            
            public string Device_id { get => device_id; set => device_id = value; }
			public string Email { get => email; set => email = value; }
			public string Country_code { get => country_code; set => country_code = value; }
			public string Region_code { get => region_code; set => region_code = value; }
			public string Year_of_birth { get => year_of_birth; set => year_of_birth = value; }
			public string Gender { get => gender; set => gender = value; }
			public string Device_type { get => device_type; set => device_type = value; }
			public string Brand { get => brand; set => brand = value; }
			public string Manufacturer { get => manufacturer; set => manufacturer = value; }
			public string Os_system { get => os_system; set => os_system = value; }
			public string Os_version { get => os_version; set => os_version = value; }
			public string Screen_resolution { get => screen_resolution; set => screen_resolution = value; }
			public string Game_store_id { get => game_store_id; set => game_store_id = value; }
			public string Fb_token { get => fb_token; set => fb_token = value; }
			public string Fb_topic { get => fb_topic; set => fb_topic = value; }

			static string GetRatioScreen()
            {
                var realRatio = Screen.width > Screen.height ?
                    Screen.width / Screen.height :
                    Screen.height / Screen.width;

                var d1 = System.Math.Abs(realRatio - 16f / 9);
                var d2 = System.Math.Abs(realRatio - 4f / 3);

                return d1 < d2 ? "16:9" : "4/3";
            }
        }

        public enum RequestMethob
        {
            POST = 0,
            GET = 1,
            PUT = 2
        }
    }
}