using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

namespace SCN.Ads.Format
{
	public enum RewardVideoState
	{
		/// <summary>
		/// Khong duoc tat quang cao khi moi mo quang cao
		/// </summary>
		UnCloseable = 0,
		/// <summary>
		/// Duoc phep tat quang cao, nhung neu tat se mat Reward
		/// </summary>
		Closeable = 1,
		/// <summary>
		/// Da hoan tat xem quang cao
		/// </summary>
		Complete = 2
	}

    public class VideoRewardAdsFormat : AdsFormatBase
    {
		System.Action<bool> onVideoRewardSuccess;

        [SerializeField] KeepRatio bannerVideo_KeepRatio;
		[SerializeField] VideoPlayer bannerVideo_VideoPlayer;
		[SerializeField] KeepRatio bannerImage_KeepRatio;
		[SerializeField] Image bannerImage_Image;

		[SerializeField] Button downloadBtn;

		[SerializeField] Button closeBtn;

		[SerializeField] RectTransform close_Cover;
		[SerializeField] Text close_RemainText;
		[SerializeField] Text close_RewardText;
		[SerializeField] Image close_XIcon;

		[SerializeField] StopRewardAdsPopup stopAdsPopup;

		bool isInit = false;
		[SerializeField] RewardVideoState state;

		Timer closeAdsTimer;

		const int closeTime = 30;

		const float animTime = 0.15f;
		readonly Vector2 remainSize = new Vector2(530, 95);
		readonly Vector2 rewardSize = new Vector2(400, 95);

		Coroutine prepareVidCorou;

		public void Show( WoaAdsManager.AdAsset asset
			, System.Action<bool> onComplete = null)
		{
			if (!isInit)
			{
				isInit = true;
				Init();
			}

			state = RewardVideoState.UnCloseable;
			urlGame = asset.UrlGame;

			stopAdsPopup.ClosePopUp();
			gameObject.SetActive(true);
			bannerImage_Image.sprite = asset.Banner;
			bannerImage_KeepRatio.Size = asset.Banner.bounds.size;

			iconImage.sprite = asset.Icon;
			nameText.text = asset.NameGame;

			closeAdsTimer.Start(closeTime, true);
			onVideoRewardSuccess = onComplete;

			if (prepareVidCorou != null)
			{
				StopCoroutine(prepareVidCorou);
			}
			prepareVidCorou = StartCoroutine(PlayVideo(asset));

			TrackingShowAds(false);
		}

		protected override void Callback_OpenStore()
		{
			TrackingClickAds(false);
			base.Callback_OpenStore();
		}

		IEnumerator PlayVideo(WoaAdsManager.AdAsset asset) 
		{
			bannerVideo_VideoPlayer.url = asset.FullPathVideoClip;
			bannerVideo_VideoPlayer.Prepare();

			//Wait until video is prepared
			while (!bannerVideo_VideoPlayer.isPrepared)
			{
				yield return null;
			}

			bannerVideo_VideoPlayer.Play();

			var clipSize = new Vector2(
				bannerVideo_VideoPlayer.texture.width, bannerVideo_VideoPlayer.texture.height);

			bannerVideo_KeepRatio.Size = clipSize;
			bannerVideo_KeepRatio.Resize();
		}

		void Init()
		{
			bannerVideo_VideoPlayer.GetComponent<Button>().onClick.RemoveAllListeners();
			downloadBtn.onClick.RemoveAllListeners();
			closeBtn.onClick.RemoveAllListeners();

			bannerVideo_VideoPlayer.GetComponent<Button>().onClick.AddListener(Callback_OpenStore);
			downloadBtn.onClick.AddListener(Callback_OpenStore);
			closeBtn.onClick.AddListener(Callback_OnClickCloseAds);

			stopAdsPopup.ClosePopUp();
			stopAdsPopup.ResumeBtn.onClick.RemoveAllListeners();
			stopAdsPopup.ResumeBtn.onClick.AddListener(Callback_OnClickResumeAds);
			stopAdsPopup.StopBtn.onClick.RemoveAllListeners();
			stopAdsPopup.StopBtn.onClick.AddListener(Callback_OnClickStopImmediately);

			closeAdsTimer = new Timer(this);
			closeAdsTimer.OnStart = ActionTimer_OnStart;
			closeAdsTimer.OnTimeChange = ActionTimer_OnTime;
			closeAdsTimer.OnDone = ActionTimer_OnDone;
		}

		void ActionTimer_OnStart()
		{
			PauseGame();

			Master.ChangeAlpha(close_XIcon, 0.33f);
			bannerImage_Image.gameObject.SetActive(false);

			close_Cover.sizeDelta = remainSize;
			close_RemainText.gameObject.SetActive(true);
			close_RewardText.gameObject.SetActive(false);

			Master.ChangeAlpha(close_RemainText, 1);
			Master.ChangeAlpha(close_RewardText, 0);
		}

		void ActionTimer_OnTime(int time)
		{
			close_RemainText.text = $"{time} Seconds Remaining";

			if (time == closeTime - 5)
			{
				Master.ChangeAlpha(close_XIcon, 1);
				state = RewardVideoState.Closeable;
			}
		}

		void ActionTimer_OnDone()
		{
			if (bannerVideo_VideoPlayer.isPlaying)
			{
				bannerVideo_VideoPlayer.Stop();
			}
			
			bannerImage_Image.gameObject.SetActive(true);
			TweenChangeAlphaImage(bannerImage_Image, 0, 1, 0.25f, true);

			close_Cover.DOSizeDelta(rewardSize, animTime).SetUpdate(true).SetEase(Ease.Linear);

			close_RewardText.gameObject.SetActive(true);
			close_RewardText.DOFade(1, animTime).SetUpdate(true).SetEase(Ease.Linear);
			close_RemainText.DOFade(0, animTime).SetUpdate(true).SetEase(Ease.Linear);

			state = RewardVideoState.Complete;
		}

		void Callback_OnClickCloseAds()
		{
			if (state == RewardVideoState.UnCloseable)
			{
				return;
			}
			else if(state == RewardVideoState.Closeable)
			{
				PauseAds();
				stopAdsPopup.OpenPopup();
			}
			else if(state == RewardVideoState.Complete)
			{
				CloseAdsUnit(true);
			}
		}

		void Callback_OnClickStopImmediately()
		{
			CloseAdsUnit(false);
		}

		void Callback_OnClickResumeAds()
		{
			stopAdsPopup.ClosePopUp();
			ResumeAds();
		}

		void CloseAdsUnit(bool isSuccess)
		{
			ResumeGame();
			gameObject.SetActive(false);
			onVideoRewardSuccess?.Invoke(isSuccess);
		}

		void PauseAds()
		{
			closeAdsTimer.PauseClock = true;
			bannerVideo_VideoPlayer.Pause();
		}

		void ResumeAds()
		{
			closeAdsTimer.PauseClock = false;
			bannerVideo_VideoPlayer.Play();
		}

#if UNITY_EDITOR
		[ContextMenu(nameof(AssignObject))]
		void AssignObject()
		{
			inforBtn = Master.GetChildByName(gameObject, "Infor game");
			iconImage = Master.GetChildByName(inforBtn, "Icon image").GetComponent<Image>();
			nameText = Master.GetChildByName(inforBtn, "Name text").GetComponent<Text>();

			var content = Master.GetChildByName(gameObject, "Content");
			bannerVideo_KeepRatio = Master.GetChildByName(content, "Banner video").GetComponent<KeepRatio>();
			bannerVideo_VideoPlayer = Master.GetChildByName(content, "Banner video").GetComponent<VideoPlayer>();
			bannerImage_Image = Master.GetChildByName(content, "Banner image").GetComponent<Image>();
			bannerImage_KeepRatio = Master.GetChildByName(content, "Banner image").GetComponent<KeepRatio>();

			downloadBtn = Master.GetChildByName(content, "Download btn").GetComponent<Button>();

			var closeObj = Master.GetChildByName(gameObject, "Close btn");

			closeBtn = Master.GetChildByName(closeObj, "Cover").GetComponent<Button>();
			close_Cover = Master.GetChildByName(closeObj, "Cover").GetComponent<RectTransform>();
			close_RemainText = Master.GetChildByName(closeObj, "Remain text").GetComponent<Text>();
			close_RewardText = Master.GetChildByName(closeObj, "Reward text").GetComponent<Text>();
			close_XIcon = Master.GetChildByName(closeObj, "X icon").GetComponent<Image>();

			stopAdsPopup = Master.GetChildByName(gameObject, "End reward popup").GetComponent<StopRewardAdsPopup>();

			_ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
			(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}
#endif
	}
}