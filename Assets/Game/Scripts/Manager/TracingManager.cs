using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SCN;

public class TracingManager : MonoBehaviour
{
    public Sprite currentPencil;
	/// <summary>
	/// The path fill image.
	/// </summary>
	private Image pathFillImage;
	private EnglishTracingBook.Path path;
	/// <summary>
	/// The click postion.
	/// </summary>
	private Vector3 clickPostion;

	/// <summary>
	/// The direction between click and shape.
	/// </summary>
	private Vector2 direction;

	/// <summary>
	/// The current angle , angleOffset and fill amount.
	/// </summary>
	private float angle, angleOffset, fillAmount;

	/// <summary>
	/// The clock wise sign.
	/// </summary>
	private float clockWiseSign;
	private float targetQuarter;
	private RaycastHit2D hit2d;
	public static CompoundShape compoundShape;
	public Shape shape;
	private bool canTracing = false;
	[SerializeField] private Transform spawnTrans;
	Tween tweenDelay;
	private int countSession = 1;
	// public LearnWriteGameplay learnWriteGameplay;

	bool soundPlaying;
	
    // Update is called once per frame
    private void Start()
    {
		// SpawnLevel(0, shape);
		//	CommonUtil.FindChildByTag(path.transform, "Fill").GetComponent<Image>().color = Color.blue;
	}
	private void OnDestroy()
    {
	}


    public void SpawnLevel(int idType,Shape level_ = null, CompoundShape compoundShape_ = null)
    {
        if (shape != null)
            shape.gameObject.SetActive(false);
        shape = Instantiate(level_, spawnTrans);
        shape.GetComponent<Animator>().enabled = false;
		shape.transform.DOScale(2.5f, 0).OnComplete(() =>
        {
			shape.Spell();
			AnimShapeBounce(shape.transform, 2.5f);
			countSession += 1;
			canTracing = true;
			StartAutoTracing(shape);
		});

		var images = shape.transform.GetComponentsInChildren<Image>();
		for (int i = 0; i < images.Length; i++)
		{
			if (i == 0)
			{
				images[i].color = Color.black;
				images[i].DOFade(0.5f, 0);
			}
			else
			{
				images[i].DOFade(0, 0);
			}
		}

		var startPos = shape.transform.GetComponentsInChildren<BoxCollider2D>();
        EventManager.OnUpdateStartPosAlpha?.Invoke(startPos[0].transform.position);
	}
	private void AnimShapeBounce(Transform shape_, float scale_)
    {
		shape_.DOScale(new Vector3(scale_ - .2f, scale_ + .2f), 0.15f).SetEase(Ease.Flash).OnComplete(() =>
		{
			shape_.DOScale(new Vector3(scale_ + .2f, scale_ - .2f), 0.15f).SetEase(Ease.Flash).OnComplete(() =>
			{
				shape_.DOScale(new Vector3(scale_ - .2f, scale_ + .2f), 0.15f).SetEase(Ease.Flash).OnComplete(() =>
				{
					shape_.DOScale(Vector3.one * scale_, 0.15f).SetEase(Ease.Flash);
					Debug.Log("Tween Scale: " + shape_.transform.localScale);
				});
			});
		});
	}
	public void StartAutoTracing(Shape s)
	{
		if (s == null)
		{
			return;
		}

		//Hide Numbers for other shapes , if we have compound shape
		if (compoundShape != null)
		{
			foreach (Shape ts in compoundShape.shapes)
			{
				if (s.GetInstanceID() != ts.GetInstanceID())
					ts.ShowPathNumbers(-1);

			}
		}
		s.ShowPathNumbers(s.GetCurrentPathIndex());
		s.GetComponent<Animator>().enabled = false;
		AnimShapeBounce(s.transform, s.transform.localScale.x);
		s.Spell();
	}
	public void OnClose()
    {
		tweenDelay?.Kill();
		canTracing = false;
	}
    void Update()
    {
		if (!canTracing || shape == null || shape.completed) return;
		if (Input.GetMouseButtonDown(0))
		{
			if (!shape.completed)
				//	brightEffect.GetComponent<ParticleEmitter> ().emit = true;

				hit2d = Physics2D.Raycast(GetCurrentPlatformClickPosition(Camera.main), Vector2.zero);
			if (hit2d.collider != null)
			{
				Debug.Log(hit2d.collider.tag);
				if (hit2d.transform.tag == "Start")
				{
					OnStartHitCollider(hit2d);
					shape.CancelInvoke();
					shape.DisableTracingHand();
					Debug.Log("Enable Hand");
				}
				else if (hit2d.transform.tag == "Collider")
				{
					shape.DisableTracingHand();
					Debug.Log("Enable Hand");
				}
			}

		}
		else if (Input.GetMouseButtonUp(0))
		{
			Debug.Log("Disable Hand");
			shape.Invoke("EnableTracingHand", 1);
			ResetPath();
		}

		if ( path == null || pathFillImage == null)
		{
			return;
		}

		if (path.completed)
		{
			return;
		}
		Debug.Log("path.fillMethod " + path.fillMethod);

		EventManager.OnScratch?.Invoke(true);

		if (path.fillMethod == EnglishTracingBook.Path.FillMethod.Radial)
		{
			RadialFill();
		}
		else if (path.fillMethod == EnglishTracingBook.Path.FillMethod.Linear)
		{
			LinearFill();
		}
		else if (path.fillMethod == EnglishTracingBook.Path.FillMethod.Point)
		{
			PointFill();
		}
	}
	private void OnStartHitCollider(RaycastHit2D hit2d)
	{
	//	SoundManager.instance.PlaySfx(SoundManager.SfxType.Drawing);
		path = hit2d.transform.GetComponentInParent<EnglishTracingBook.Path>();
		Debug.Log("Hit collider");

		pathFillImage = CommonUtil.FindChildByTag(path.transform, "Fill").GetComponent<Image>();

		if (path.completed || !shape.IsCurrentPath(path))
		{
			ReleasePath();
		}
		else
		{
			path.StopAllCoroutines();
		//	CommonUtil.FindChildByTag(path.transform, "Fill").GetComponent<Image>().color = Color.blue;
		//	CommonUtil.FindChildByTag(path.transform, "Fill").GetComponent<Image>().sprite = currentPencil;
		//	CommonUtil.FindChildByTag(path.transform, "Fill").GetComponent<Image>().SetNativeSize();
		}

		if (path != null)
			if (!path.shape.enablePriorityOrder)
			{
				shape = path.shape;
			}
	}
	private Vector3 GetCurrentPlatformClickPosition(Camera camera)
	{
		Vector3 clickPosition = Vector3.zero;
		Debug.Log("Application.isMobilePlatform " + Application.isMobilePlatform);
		if (Application.isMobilePlatform)
		{//current platform is mobile
			if (Input.touchCount != 0)
			{
				Touch touch = Input.GetTouch(0);
				clickPosition = touch.position;
			}
		}
		else
		{//others
			clickPosition = Input.mousePosition;
		}
		clickPosition = camera.ScreenToWorldPoint(clickPosition);//get click position in the world space
		clickPosition.z = 0;
		return clickPosition;
	}
	private void RadialFill()
	{
		clickPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		direction = clickPostion - path.transform.position;

		angleOffset = 0;
		clockWiseSign = (pathFillImage.fillClockwise ? 1 : -1);

		if (pathFillImage.fillOrigin == 0)
		{//Bottom
			angleOffset = 0;
		}
		else if (pathFillImage.fillOrigin == 1)
		{//Right
			angleOffset = clockWiseSign * 90;
		}
		else if (pathFillImage.fillOrigin == 2)
		{//Top
			angleOffset = -180;
		}
		else if (pathFillImage.fillOrigin == 3)
		{//left
			angleOffset = -clockWiseSign * 90;
		}

		angle = Mathf.Atan2(-clockWiseSign * direction.x, -direction.y) * Mathf.Rad2Deg + angleOffset;

		if (angle < 0)
			angle += 360;

		angle = Mathf.Clamp(angle, 0, 360);
		angle -= path.radialAngleOffset;

		if (path.quarterRestriction)
		{
			if (!(angle >= 0 && angle <= targetQuarter))
			{
				pathFillImage.fillAmount = 0;
				return;
			}

			if (angle >= targetQuarter / 2)
			{
				targetQuarter += 90;
			}
			else if (angle < 45)
			{
				targetQuarter = 90;
			}

			targetQuarter = Mathf.Clamp(targetQuarter, 90, 360);
		}

		fillAmount = Mathf.Abs(angle / 360.0f);
		pathFillImage.fillAmount = fillAmount;
		CheckPathComplete();
	}

	/// <summary>
	/// Linear fill method.
	/// </summary>
	private void LinearFill()
	{
		clickPostion = Camera.main.ScreenToWorldPoint( Input.mousePosition);

		Vector3 rotation = path.transform.eulerAngles;
		rotation.z -= path.offset;

		Rect rect = CommonUtil.RectTransformToScreenSpace(path.GetComponent<RectTransform>());

		Vector3 pos1 = Vector3.zero, pos2 = Vector3.zero;

		if (path.type == EnglishTracingBook.Path.ShapeType.Horizontal)
		{
			pos1.x = path.transform.position.x - Mathf.Sin(rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
			pos1.y = path.transform.position.y - Mathf.Cos(rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;

			pos2.x = path.transform.position.x + Mathf.Sin(rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
			pos2.y = path.transform.position.y + Mathf.Cos(rotation.z * Mathf.Deg2Rad) * rect.width / 2.0f;
		}
		else
		{

			pos1.x = path.transform.position.x - Mathf.Cos(rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
			pos1.y = path.transform.position.y - Mathf.Sin(rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;

			pos2.x = path.transform.position.x + Mathf.Cos(rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
			pos2.y = path.transform.position.y + Mathf.Sin(rotation.z * Mathf.Deg2Rad) * rect.height / 2.0f;
		}

		pos1.z = path.transform.position.z;
		pos2.z = path.transform.position.z;

		if (path.flip)
		{
			Vector3 temp = pos2;
			pos2 = pos1;
			pos1 = temp;
		}

		clickPostion.x = Mathf.Clamp(clickPostion.x, Mathf.Min(pos1.x, pos2.x), Mathf.Max(pos1.x, pos2.x));
		clickPostion.y = Mathf.Clamp(clickPostion.y, Mathf.Min(pos1.y, pos2.y), Mathf.Max(pos1.y, pos2.y));
		fillAmount = Vector2.Distance(clickPostion, pos1) / Vector2.Distance(pos1, pos2);
		pathFillImage.fillAmount = fillAmount;
		CheckPathComplete();
	}

	/// <summary>
	/// Point fill.
	/// </summary>
	private void PointFill()
	{
		pathFillImage.fillAmount = 1;
		CheckPathComplete();
	}

	/// <summary>
	/// Checks wehther path completed or not.
	/// </summary>
	private void CheckPathComplete()
	{
		if (fillAmount >= path.completeOffset)
		{

			path.completed = true;
			path.AutoFill();
			path.SetNumbersVisibility(false);
			ReleasePath();
			if (CheckShapeComplete())
			{
				shape.completed = true;
				Debug.Log("Shape Complete");
				shape.gameObject.SetActive(false);
				OnShapeComplete();
				
			}
			else
			{
				Debug.Log("Correct Path");
			}

			shape.ShowPathNumbers(shape.GetCurrentPathIndex());

			hit2d = Physics2D.Raycast(GetCurrentPlatformClickPosition(Camera.main), Vector2.zero);
			if (hit2d.collider != null)
			{
				if (hit2d.transform.tag == "Start")
				{
					if (shape.IsCurrentPath(hit2d.transform.GetComponentInParent<EnglishTracingBook.Path>()))
					{
						OnStartHitCollider(hit2d);
					}
				}
			}
		}
	}

	/// <summary>
	/// Check whether the shape completed or not.
	/// </summary>
	/// <returns><c>true</c>, if shape completed, <c>false</c> otherwise.</returns>
	private bool CheckShapeComplete()
	{
		bool shapeCompleted = true;
		EnglishTracingBook.Path[] paths = shape.GetComponentsInChildren<EnglishTracingBook.Path>();
		foreach (EnglishTracingBook.Path path in paths)
		{
			if (!path.completed)
			{
				shapeCompleted = false;
				break;
			}
		}
		return shapeCompleted;
	}
	private void OnShapeComplete()
	{
		bool allDone = true;

		List<Shape> shapes = new List<Shape>();

		if (compoundShape != null)
		{
			shapes = compoundShape.shapes;
			allDone = compoundShape.IsCompleted();

			if (!allDone)
			{
				shape = compoundShape.shapes[compoundShape.GetCurrentShapeIndex()];
				StartAutoTracing(shape);

			}
		}
		else
		{
			shapes.Add(shape);
		}

		if (allDone)
		{
			if (compoundShape != null)
			{
				compoundShape.PlayAnim();
				for (int i = 0; i < compoundShape.shapes.Count; i++)
				{
				//	compoundShape.shapes[i].GetComponent<Animator>().enabled = false;
				//	AnimShapeBounce(compoundShape.shapes[i].transform, compoundShape.shapes[i].transform.localScale.x);
					Debug.Log("Compound Shape: " + compoundShape.shapes[i].name);
				}
				AnimShapeBounce(shape.transform, shape.transform.localScale.x);
				tweenDelay = DOVirtual.DelayedCall(3f, () => {
					if (countSession >= 2)
					{
					/*	if (AdsManager.Instance.HasInters)
						{
							AdsManager.Instance.ShowInterstitial(() =>
							{
								FirebaseManager.Instance.WatchInter("Tracing");
								countSession = 0;
								OnClose();
							});
						}
						else
						{
							OnClose();
						}*/
					}
					else
					{
						OnClose();
					}
				});
			}
			else
			{
				shape.Spell();
				AnimShapeBounce(shape.transform, 2.5f);
				tweenDelay = DOVirtual.DelayedCall(1f, () =>
				{
					if (countSession >= 2)
					{
					/*	if (AdsManager.Instance.HasInters)
						{
							FirebaseManager.Instance.WatchInter("Tracing");
							AdsManager.Instance.ShowInterstitial(() =>
							{
								countSession = 0;
								OnClose();
							});
						}
						else
						{
							OnClose();
						}*/
					}
					else
					{
						OnClose();
					}
				});
			}
		}
		else
		{
		}
	}
	private void ResetPath()
	{
		canTracing = true;
		if (path != null)
			path.Reset();
		ReleasePath();
		ResetTargetQuarter();
	}
	private void ResetTargetQuarter()
	{
		targetQuarter = 90;
	}

	/// <summary>
	/// Release the path.
	/// </summary>
	private void ReleasePath()
	{
		path = null;
		pathFillImage = null;
	}

}
