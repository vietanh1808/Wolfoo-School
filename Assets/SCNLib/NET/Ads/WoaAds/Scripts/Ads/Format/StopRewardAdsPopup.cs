using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.Ads.Format
{
	public class StopRewardAdsPopup : MonoBehaviour
	{
		[SerializeField] Button resumeBtn;
		[SerializeField] Button stopBtn;

		public Button ResumeBtn => resumeBtn;
		public Button StopBtn => stopBtn;

		public void OpenPopup()
		{
			gameObject.SetActive(true);
		}

		public void ClosePopUp()
		{
			gameObject.SetActive(false);
		}
	}
}