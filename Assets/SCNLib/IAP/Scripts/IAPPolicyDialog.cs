using DG.Tweening;
using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCN.IAP
{
	public class IAPPolicyDialog : MonoBehaviour
	{
		#region Init
		public static IAPPolicyDialog Instance 
		{
			get 
			{
				if (_instance == null)
				{
					_instance = Instantiate(Resources.Load<GameObject>("IAP canvas"))
						.GetComponent<IAPPolicyDialog>();
					_instance.Setup();
				}
					
				return _instance;
			}
		}
		static IAPPolicyDialog _instance;

		void Setup()
		{
			// Setup
			_blockArea.SetActive(false);
			_dialog.SetActive(false);
			_noticeText.Setup(this);

			// Canvas
			var canvas = GetComponent<Canvas>();
			canvas.worldCamera = Camera.main;
			canvas.sortingLayerName = sortingLayer;
			canvas.sortingOrder = orderInLayer;

			_blockBtn = false;
			_currentRetry = _retryTime;

			// Add listener
			_closeBtn.onClick.RemoveAllListeners();
			_closeBtn.onClick.AddListener(()=> 
			{
				if (_blockBtn)
				{
					return;
				}
				CloseDialog();
			});

			for (int i = 0; i < _options.childCount; i++)
			{
				var index = i;

				_options.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
				_options.GetChild(i).GetComponent<Button>().onClick.AddListener(()=> 
				{
					if (_blockBtn)
					{
						return;
					}
					OnClickOptions(index);
				});
			}

			timer = new Timer(this)
			{
				OnDone = () =>
				{
					_currentRetry++;
					if (_currentRetry > _retryTime)
					{
						_currentRetry = _retryTime;
					}

					CdRetryTime();
				}
			};
		}
		#endregion

		System.Action _onSuccess;

		[SerializeField] GameObject _blockArea;

		[SerializeField] GameObject _dialog;
		[SerializeField] Button _closeBtn;
		[SerializeField] Text _calculationText;
		[SerializeField] Text _answerText;

		[SerializeField] Transform _options;

		[SerializeField] NoticeText _noticeText;

		int _currentCorrectAnswer;
		bool _blockBtn;

		[SerializeField] int _currentRetry;
		[SerializeField] Timer timer;

		[Space]
		[Header("Setings")]

		[Tooltip("So lan chon lai dap an")]
		[SerializeField] int _retryTime = 3;

		[Tooltip("Thoi gian cho lam thu tiep theo")]
		[SerializeField] int _nextTryTime = 60;

		[Tooltip("Layer dialog")]
		[SerializeField] string sortingLayer;
		[Tooltip("Set sorting de dialog co order cao nhat")]
		[SerializeField] int orderInLayer;

		public void OpenDialog(System.Action onSuccess)
		{
			if (_currentRetry == 0)
			{
				_noticeText.ShowTryLater();
				return;
			}

			_onSuccess = onSuccess;

			_blockArea.SetActive(true);
			_dialog.SetActive(true);

			_dialog.transform.DOKill();
			_dialog.transform.localScale = Vector3.one * 0.7f;
			_dialog.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

			RandomQuest();
		}

		void CloseDialog()
		{
			_blockArea.SetActive(false);
			_dialog.SetActive(false);
		}

		void RandomQuest()
		{
			var _numb0 = Random.Range(3, 10);
			var _numb1 = Random.Range(3, 10);
			var _answer = _numb0 * _numb1;
			_currentCorrectAnswer = Random.Range(0, _options.childCount);

			var _tempList = new List<int>();
			for (int i = 0; i < 10; i++)
			{
				if (i == 5) continue;

				_tempList.Add(_answer - 5 + i);
			}

			var _random = new RandomNoRepeat<int>(_tempList);

			for (int i = 0; i < _options.childCount; i++)
			{
				_options.GetChild(i).GetChild(0).GetComponent<Text>().text = i == _currentCorrectAnswer ?
					_answer.ToString() : _random.Random().ToString();
			}

			_calculationText.text = _numb0 + " x " + _numb1 + " =";
			_answerText.text = "";
		}

		void OnClickOptions(int order)
		{
			_answerText.text = _options.GetChild(order).GetChild(0).GetComponent<Text>().text;
			_blockBtn = true;

			_ = StartCoroutine(DelayCallMaster.WaitAndDoIE(0.5f, () =>
			{
				_blockBtn = false;
				if (order == _currentCorrectAnswer)
				{
					_currentRetry = _retryTime;
					timer.Stop();

					CloseDialog();
					_onSuccess?.Invoke();
				}
				else
				{
					_currentRetry--;
					if (_currentRetry == _retryTime - 1)
					{
						CdRetryTime();
					}

					if (_currentRetry == 0)
					{
						CloseDialog();
						_noticeText.ShowTryLater();
					}
					else
					{
						_noticeText.ShowIncorrect();
						RandomQuest();
					}
				}
			}));
		}

		void CdRetryTime()
		{
			if (_currentRetry < _retryTime)
			{
				timer.Start(_nextTryTime, true);
			}
		}
	}
}
