using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyingGlass : ItemMove
{
    private Tweener scaleItemTween;
    private Tween delayItemTween;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }
    public void OnOpen(System.Action OnComplete)
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
        scaleItemTween = transform.DOScale(startScale, 1)
        .OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }
    public void OnClose(System.Action OnClose, System.Action OnComplete)
    {
        scaleItemTween = transform.DOScale(Vector3.zero, 1)
        .OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
        delayItemTween = DOVirtual.DelayedCall(0.5f, () =>
        {
            OnClose?.Invoke();
        });
    }
    private void OnDestroy()
    {
        if (scaleItemTween != null) scaleItemTween?.Kill();
    }
}
