using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN.Common;
using UnityEngine.UI;
using DG.Tweening;

namespace SCN.Ads
{
	public class RewardInterstitialPopup : MonoBehaviour
	{
		#region Init
		public static RewardInterstitialPopup Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Instantiate(Resources.Load<GameObject>("Reward inter popup"))
						.GetComponent<RewardInterstitialPopup>();
					_instance.Setup();
				}

				return _instance;
			}
		}
		static RewardInterstitialPopup _instance;

		void Setup()
		{
			// Setup
			_blockArea.SetActive(false);
			_dialog.SetActive(false);

			// Canvas
			var canvas = GetComponent<Canvas>();
			canvas.worldCamera = Camera.main;
		//	canvas.sortingLayerName = AdsManager.Instance.AdmobControl.Config.SortingLayerRewardInterPopup;
			//canvas.sortingOrder = AdsManager.Instance.AdmobControl.Config.OrderInLayerRewardInterPopup;

			// Add listener
			_closeBtn.onClick.RemoveAllListeners();
			_closeBtn.onClick.AddListener(Callback_OnClickCloseBtn);

			_runAdsBtn.onClick.RemoveAllListeners();
			_runAdsBtn.onClick.AddListener(Callback_OnClickRunAdsBtn);

			timer = new Timer(this)
			{
				OnDone = () => 
				{
					Close();
					_onShowAds?.Invoke();
				},
				OnTimeChange = Event_OnTimeChange
			};
		}
		#endregion

		System.Action _onShowAds;

		[SerializeField] GameObject _blockArea;

		[SerializeField] GameObject _dialog;

		[SerializeField] Button _closeBtn;

		[SerializeField] Button _runAdsBtn;
		[SerializeField] Text _runAdsText;

		[SerializeField] Transform _bannerTrans;

		[SerializeField] Timer timer;

		/// <summary>
		/// Mo dialog xac nhan truoc khi show Reward Ads
		/// </summary>
		/// <param name="onShowAds">Khi nguoi dung doi het 5s, hoac bam truc tiep vao xem ads luon</param>
		/// <param name="bannerName">Lua chon show banner nao, trong truong hop game nhieu loai banner</param>
		public void OpenDialog(System.Action onShowAds, string bannerName = "Default")
		{
			_onShowAds = onShowAds;

			Open(bannerName);
			timer.Start(5, true);
		}

		void Open(string bannerName)
		{
			_blockArea.SetActive(true);
			_dialog.SetActive(true);
			_bannerTrans.gameObject.SetActive(true);
			for (int i = 0; i < _bannerTrans.childCount; i++)
			{
				var banner = _bannerTrans.GetChild(i).gameObject;
				banner.SetActive(banner.name == bannerName);
			}

			_dialog.transform.DOKill();
			_dialog.transform.localScale = Vector3.one * 0.7f;
			_dialog.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
		}

		void Close()
		{
			_blockArea.SetActive(false);
			_dialog.SetActive(false);

			_bannerTrans.gameObject.SetActive(false);
		}

		void Callback_OnClickCloseBtn()
		{
			Close();
			timer.Stop(false);
		}

		void Callback_OnClickRunAdsBtn()
		{
			Close();
			timer.Stop(true);
		}

		void Event_OnTimeChange(int time)
		{
			_runAdsText.text = "Video starting in " + time + "...";
		}
	}
}