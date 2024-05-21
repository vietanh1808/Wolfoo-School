using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.Ads.Format
{
	public class InterstitialsAdsFormat : AdsFormatBase
	{
		System.Action onInterSuccess;

		[SerializeField] KeepRatio bannerImage;

		[SerializeField] Button downloadBtn;

		[SerializeField] Button closeBtn;
		[SerializeField] Text closeText;
		[SerializeField] Text closeTimeText;
		[SerializeField] GameObject checkMarkCloseObj;

		[SerializeField] Color closeTextColor;

		bool isInit = false;
		bool isCloseable;

		Timer closeAdsTimer;

		const int closeTime = 5;

		public void Show( WoaAdsManager.AdAsset asset, System.Action onComplete = null)
		{
			if (!isInit)
			{
				isInit = true;
				Init();
			}

			isCloseable = false;
			urlGame = asset.UrlGame;

			gameObject.SetActive(true);

			iconImage.sprite = asset.Icon;
			nameText.text = asset.NameGame;

			bannerImage.GetComponent<Image>().sprite = asset.Banner;
			bannerImage.Size = asset.Banner.bounds.size;
			bannerImage.Resize();

			closeAdsTimer.Start(closeTime, true);
			onInterSuccess = onComplete;

			TrackingShowAds(true);
		}

		protected override void Callback_OpenStore()
		{
			TrackingClickAds(true);
			base.Callback_OpenStore();
		}

		void Init()
		{
			bannerImage.GetComponent<Button>().onClick.RemoveAllListeners();
			downloadBtn.onClick.RemoveAllListeners();
			closeBtn.onClick.RemoveAllListeners();

			bannerImage.GetComponent<Button>().onClick.AddListener(Callback_OpenStore);
			downloadBtn.onClick.AddListener(Callback_OpenStore);
			closeBtn.onClick.AddListener(Callback_CloseAds);

			closeAdsTimer = new Timer(this);
			closeAdsTimer.OnStart = ActionTimer_OnStart;
			closeAdsTimer.OnTimeChange = ActionTimer_OnTime;
			closeAdsTimer.OnDone = ActionTimer_OnDone;
		}

		void ActionTimer_OnStart()
		{
			PauseGame();

			closeText.color = closeTextColor;
			closeTimeText.gameObject.SetActive(true);
			checkMarkCloseObj.SetActive(false);
		}

		void ActionTimer_OnTime(int time)
		{
			closeTimeText.text = time.ToString();
		}

		void ActionTimer_OnDone()
		{
			closeText.color = Color.white;
			closeTimeText.gameObject.SetActive(false);
			checkMarkCloseObj.SetActive(true);

			isCloseable = true;
		}

		void Callback_CloseAds()
		{
			if (!isCloseable)
			{
				return;
			}

			ResumeGame();
			gameObject.SetActive(false);
			onInterSuccess?.Invoke();
		}

#if UNITY_EDITOR
		[ContextMenu(nameof(AssignObject))]
		void AssignObject()
		{
			inforBtn = Master.GetChildByName(gameObject, "Infor game");
			iconImage = Master.GetChildByName(inforBtn, "Icon image").GetComponent<Image>();
			nameText = Master.GetChildByName(inforBtn, "Name text").GetComponent<Text>();

			var content = Master.GetChildByName(gameObject, "Content");
			bannerImage = Master.GetChildByName(content, "Banner image").GetComponent<KeepRatio>();
			downloadBtn = Master.GetChildByName(content, "Download btn").GetComponent<Button>();

			closeBtn = Master.GetChildByName(content, "Close btn").GetComponent<Button>();
			closeText = Master.GetChildByName(closeBtn.gameObject, "Close text").GetComponent<Text>();
			closeTimeText = Master.GetChildByName(closeBtn.gameObject, "Time text").GetComponent<Text>();
			checkMarkCloseObj = Master.GetChildByName(closeBtn.gameObject, "Check mark");

			_ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
			(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}
#endif
	}
}