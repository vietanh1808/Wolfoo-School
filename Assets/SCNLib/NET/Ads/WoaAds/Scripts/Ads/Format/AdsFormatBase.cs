using DG.Tweening;
using SCN.FirebaseLib.FA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.Ads.Format
{
	public class AdsFormatBase : MonoBehaviour
	{
		[SerializeField] protected GameObject inforBtn;
		[SerializeField] protected Image iconImage;
		[SerializeField] protected Text nameText;

		protected float currentTimeScale;

		protected string urlGame;

        protected void TrackingShowAds(bool isInterAds, string showPos = "")
		{
			if (isInterAds)
			{
                GAManager.Instance.TrackShowInterAds(showPos, ConstValue.WoaAds);
            }
			else
			{
                GAManager.Instance.TrackShowRewardVideoAds(showPos, ConstValue.WoaAds);
			}

            WoaAdsNetworking.PostData(ConstValue.AdImpression, null);
            WoaAdsNetworking.PostData(ConstValue.AdCount, null);
        }

        protected void TrackingClickAds(bool isInterAds)
		{
			if (isInterAds)
			{
                GAManager.Instance.TrackClickInterAds();
            }
			else
			{
                GAManager.Instance.TrackClickVideoRewardAds();
			}

            var clickTime = PlayerPrefs.GetInt(ConstValue.ClickWoaAdCountKey, 0);
            PlayerPrefs.SetInt(ConstValue.ClickWoaAdCountKey, clickTime + 1);
            WoaAdsNetworking.PostData(ConstValue.AdClick, null);
        }

        protected virtual void Callback_OpenStore()
		{
            Application.OpenURL(urlGame);
		}

        protected void PauseGame()
        {
            currentTimeScale = Time.timeScale;
            Time.timeScale = 0;

            var audioSources = FindObjectsOfType<AudioSource>();
            foreach (var item in audioSources)
            {
                if (item != null && item.isPlaying) item.Pause();
            }
        }
        protected void ResumeGame()
        {
            Time.timeScale = currentTimeScale;

            var audioSources  = FindObjectsOfType<AudioSource>();
            foreach (var item in audioSources)
            {
				item?.UnPause();
            }
        }

        public Tweener TweenChangeAlphaImage(Image image, float start, float to, float duration
            , bool isUsingUnscaleTime = false)
        {
            var color = image.color;
            color.a = start;
            image.color = color;
            return image.DOFade(to, duration).SetUpdate(isUsingUnscaleTime);
        }
    }
}