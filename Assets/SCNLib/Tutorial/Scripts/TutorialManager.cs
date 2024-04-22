using DG.Tweening;
using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SCN.Tutorial 
{
	public class TutorialManager : MonoBehaviour
	{
		static TutorialManager instance;
		public static TutorialManager Instance
		{
			get
			{
				if (instance == null)
				{
					Setup();
				}

				return instance;
			}
			private set
			{
				instance = value;
			}
		}

        public float NoReactTime { get => noReactTime; set => noReactTime = value; }

        public static System.Action OnUserNoReact;

		static void Setup()
		{
			instance = Instantiate(LoadSource.LoadObject<GameObject>("Tutorial Canvas"))
				.GetComponent<TutorialManager>();

			// Canvas
			var canvas = instance.GetComponent<Canvas>();
			canvas.worldCamera = Camera.main;
			canvas.sortingLayerName = instance.sortingLayer;
			canvas.sortingOrder = instance.orderInLayer;

			instance.pointerTrans.gameObject.SetActive(false);
		}

		[SerializeField] RectTransform pointerTrans;
		[SerializeField] RectTransform imageTrans;

		Vector2 vt = new Vector2(100, -100);
		float moveTime = 0.5f;

		[Space]
		[Header("Setings")]

		[Tooltip("Layer cua ban tay")]
		[SerializeField] string sortingLayer;
		[Tooltip("Layer cua ban tay")]
		[SerializeField] int orderInLayer;
		[SerializeField] float noReactTime = 5;

		private void Awake()
		{
			SceneManager.sceneLoaded += Event_sceneLoaded;
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= Event_sceneLoaded;
		}

		private void Start()
		{
			AttachCamera();
		}

		void Event_sceneLoaded(Scene _null1, LoadSceneMode _null2)
		{
			AttachCamera();
		}

		void AttachCamera()
		{
			GetComponent<Canvas>().worldCamera = Camera.main;
		}

		#region Public methob
		/// <summary>
		/// Ngón tay chỉ vào vị trí nào đó cố định
		/// </summary>
		/// <param name="pos">Vị trí chỉ ngón tay</param>
		/// <param name="isRight">Bàn tay bên phải hay bên trái vị trí đó</param>
		public void StartPointer(Vector3 pos, bool isRight = true)
		{
			WaitUserNoReact(()=>
			{
				SetupPointer(pos, isRight);
				pointerTrans.position = pos;

				_ = StartCoroutine(IEWaitUserClick(() => 
				{
					StopPointer();
					StartPointer(pos, isRight);
				}));
			});
		}

		/// <summary>
		/// Ngón tay chỉ theo 1 GameObject, di chuyển follow nếu GameObject đó di chuyển
		/// </summary>
		/// <param name="transform">Object được follow</param>
		/// <param name="isRight">Bàn tay bên phải hay bên trái Object đó</param>
		public void StartPointer(Transform transform, bool isRight = true)
		{
			WaitUserNoReact(() =>
			{
				SetupPointer(transform.position, isRight);
				var corou = StartCoroutine(IEFollowTrans(transform));

				_ = StartCoroutine(IEWaitUserClick(() =>
				{
					StopCoroutine(corou);
					StopPointer();

					StartPointer(transform, isRight);
				}));
			});
		}
		IEnumerator IEFollowTrans(Transform trans)
		{
			while (true)
			{
				pointerTrans.position = trans.position;
				yield return null;
			}
		}

		/// <summary>
		/// Ngón tay chỉ từ vị trí A đến vị trí B
		/// </summary>
		/// <param name="startPos">Vị trí đầu</param>
		/// <param name="endPos">Vị trí cuối</param>
		/// <param name="isRight">Bàn tay quay bên phải hay bên trái</param>
		public void StartPointer(Vector3 startPos, Vector3 endPos, bool isRight = true)
		{
			WaitUserNoReact(() =>
			{
				SetupPointer(transform.position, isRight, false);
				MovePointer(startPos, endPos);
				
				_ = StartCoroutine(IEWaitUserClick(() =>
				{
					StopPointer();
					StartPointer(startPos, endPos, isRight);
				}));
			});
		}
		void MovePointer(Vector3 startPos, Vector3 endPos)
		{
			var duration = Vector3.Distance(startPos, endPos) / 2.7f;

			pointerTrans.position = startPos;
			_ = pointerTrans.DOMove(endPos, duration).SetEase(Ease.Linear)
				.SetUpdate(false).SetLoops(-1);
		}

		public void Stop()
		{
			_ = StartCoroutine(IEStop());
		}
		IEnumerator IEStop()
		{
			yield return new WaitForEndOfFrame();
			StopAllCoroutines();

			pointerTrans.gameObject.SetActive(false);
			pointerTrans.DOKill();
			imageTrans.DOKill();
		}
		#endregion

		Coroutine waitUserNoReactCorou;
		void WaitUserNoReact(System.Action onNoReact)
		{
			if (waitUserNoReactCorou != null)
			{
				StopCoroutine(waitUserNoReactCorou);
			}
			waitUserNoReactCorou = StartCoroutine(IEWaitUserNoReact(onNoReact));
		}
		IEnumerator IEWaitUserNoReact(System.Action onNoReact)
		{
			var timer = 0f;
			while (true)
			{
				if (CheckUserClick())
				{
					yield return new WaitForEndOfFrame();
					WaitUserNoReact(onNoReact);
					break;
				}
				timer += Time.unscaledDeltaTime;

				if (timer > noReactTime)
				{
					onNoReact?.Invoke();
					break;
				}

				yield return null;
			}
		}

		IEnumerator IEWaitUserClick(System.Action onUserClick)
		{
			while (true)
			{
				if (CheckUserClick())
				{
					onUserClick?.Invoke();
					break;
				}

				yield return null;
			}
		}

		bool CheckUserClick()
		{
#if UNITY_EDITOR || UNITY_WEBGL
			return Input.GetMouseButtonDown(0);
#else
			if (Input.touchCount > 0)
			{
				foreach (var touch in Input.touches)
				{
					if (touch.phase == TouchPhase.Began)
					{
						return true;
					}
				}
			}

			return false;
#endif
		}

		void SetupPointer(Vector3 position, bool isRight, bool isAnim = true)
		{
			pointerTrans.gameObject.SetActive(true);
			pointerTrans.position = position;
			pointerTrans.localScale = new Vector3(isRight ? 1 : -1, 1, 1);

			imageTrans.DOKill();

			if (isAnim)
			{
				AnimPointerImage();
			}
		}

		void AnimPointerImage()
		{
			imageTrans.anchoredPosition = vt;
			imageTrans.DOAnchorPos(Vector2.zero, moveTime).SetEase(Ease.Linear).SetUpdate(true).OnComplete(()=> 
			{
				imageTrans.DOAnchorPos(vt, moveTime).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
				{
					AnimPointerImage();
				});
			});
		}

		void StopPointer()
		{
			imageTrans.DOKill();
			pointerTrans.DOKill();

			pointerTrans.gameObject.SetActive(false);
		}
	}
}