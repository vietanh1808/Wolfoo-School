using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Rake : MonoBehaviour
{
    bool canDrag = true;
    private Tweener rotateVerticalTween;
    Vector3 startPos;
    private Tweener rotateHorizontalTween;
    private bool isBegin;
    private bool isEnd;
    private Tweener moveTween;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnMouseDrag()
    {
        GameManager.instance.GetCurrentPosition(transform);

        if (SoundManager.instance.Sfx.isPlaying) return;
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Draw);
    }
    private void OnMouseDown()
    {
        if (isBegin) return;
        isBegin = true;
        isEnd = false;

        if (rotateVerticalTween != null) rotateVerticalTween?.Kill();
        if (moveTween != null) moveTween?.Kill();

        rotateHorizontalTween = transform.DORotate(Vector3.forward * 90, 0.5f);
        EventManager.OnBeginDragDrake?.Invoke();
    }
    private void OnMouseUp()
    {
        if (isEnd) return;
        isEnd = true;
        isBegin = false;

        if (rotateHorizontalTween != null) rotateHorizontalTween?.Kill();
        rotateVerticalTween = transform.DORotate(Vector3.zero, 0.5f);

        EventManager.OnEndDragDrake?.Invoke();
        moveTween = transform.DOMove(startPos, 10).SetSpeedBased(true).OnComplete(() =>
        {
        });
    }
}
