using SCN.FirebaseLib.FA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.Ads.CrossPromo
{
    public class NativeCrossPromo : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button btn;
        [SerializeField] private string url;
        private void Start()
        {
            btn.onClick.AddListener(OnClickIcon);
            AdsManager.Instance.WoaAdsControl.UpdateAdsAssets += InitNativeAds;
        }
        private void OnEnable()
        {
            InitNativeAds();
        }
        private void InitNativeAds()
        {
            GAManager.Instance.TrackShowNative();
            if (AdsManager.Instance.WoaAdsControl.IsAdsAssetsAble)
            {
                var Assets = AdsManager.Instance.WoaAdsControl.ListAdAssets[0];
                icon.sprite = Assets.Banner;
                url = Assets.UrlGame;
            }
        }
        private void OnClickIcon()
        {
            Application.OpenURL(url);
            WoaAdsNetworking.PostData(ConstValue.AdClick, null);
            GAManager.Instance.TrackClickNative();
        }
    }
}
