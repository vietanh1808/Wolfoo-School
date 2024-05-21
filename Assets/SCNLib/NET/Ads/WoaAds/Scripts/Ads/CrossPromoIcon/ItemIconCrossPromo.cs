using SCN.FirebaseLib.FA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.Ads.CrossPromo
{
    public class ItemIconCrossPromo : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button btn;
        private string url;
        private void Start()
        {
            btn.onClick.AddListener(OnClickIcon);
        }
        private void OnEnable()
        {
            GAManager.Instance.TrackShowIcons();
        }
        public void InitializeItem(Sprite icon_, string url_)
        {
            icon.sprite = icon_;
            url = url_;
        }
        private void OnClickIcon()
        {
            Application.OpenURL(url);
            GAManager.Instance.TrackClickIcons();
        }
    }
}
