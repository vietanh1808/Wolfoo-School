using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.Ads
{
    public partial class Bootstrap : MonoBehaviour
    {
        partial void PreloadAds();
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            AdsManager.Instance.Preload(transform);
        }
    }
}
