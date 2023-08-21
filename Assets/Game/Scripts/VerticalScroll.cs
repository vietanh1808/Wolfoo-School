using DG.Tweening;
using SCN.UIExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VerticalScroll : VerticalScrollInfinity
{
    private bool isHide = true;
    private Tweener rotateTween;
    private Tween delayTween;
    private Vector3 startPos;

    public bool IsHide { get => isHide; }

    private void Awake()
    {
		velocity = 0;
		startPos = transform.position;
	}
    private void OnDestroy()
    {
		if (rotateTween != null) rotateTween?.Kill();
		if (delayTween != null) delayTween?.Kill();
    }
    public void Spawn(float time = 2f, System.Action OnComplete = null)
    {
		BlockInput(true);
		isHide = false;
		if(rotateTween != null) rotateTween.Kill();

		rotateTween = transform.DORotate(Vector3.zero, 1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
			velocity = 15;
		});
		delayTween = DOVirtual.DelayedCall(time, () =>
		{
			velocity = 0;
			BlockInput(false);
			OnComplete?.Invoke();
		});
    }
	public void Hide(System.Action OnComplete = null)
    {
		isHide = true;
		if(rotateTween != null) rotateTween.Kill();
		rotateTween = transform.DORotate(Vector3.forward * 90, 1).OnComplete(() =>
		{
			velocity = 0;
			OnComplete?.Invoke();
		});
    }
	public void MoveOut(Direction direction, float time = 0.5f, System.Action OnComplete = null)
    {
		isHide = true;
		var _endPos = Vector2.zero;
		switch (direction)
		{
			case Direction.Left:
				_endPos = new Vector2(UISetupManager.Instance.outsideLeft.position.x, transform.position.y);
				break;
			case Direction.Right:
				_endPos = new Vector2(UISetupManager.Instance.outsideLeft.position.x * -1, transform.position.y);
				break;
			case Direction.Up:
				_endPos = new Vector2(transform.position.x, UISetupManager.Instance.outsideDown.position.y * -1);
				break;
			case Direction.Down:
				_endPos = new Vector2(transform.position.x, UISetupManager.Instance.outsideDown.position.y);
				break;
		}

		if (time == 0)
		{
			transform.position = _endPos;
			OnComplete?.Invoke();
			return;
		}

		if (rotateTween != null) rotateTween.Kill();
		rotateTween = transform.DOMove(_endPos, 1)
		.SetEase(Ease.InBack)
		.OnComplete(() =>
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
			OnComplete?.Invoke();
		});
	}
	public void MoveInBack(float time = 2f, System.Action OnComplete = null)
	{
		BlockInput(true);
		isHide = false;

		if (rotateTween != null) rotateTween.Kill();
		rotateTween = transform.DOMove(startPos, 1f)
		.SetEase(Ease.OutBack)
		.OnComplete(() =>
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
			velocity = 15;
		});
		delayTween = DOVirtual.DelayedCall(time, () =>
		{
			velocity = 0;
			BlockInput(false);
			OnComplete?.Invoke();
		});
	}
}
