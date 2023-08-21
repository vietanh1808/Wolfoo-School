using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStick : ItemMove
{
    [SerializeField] ParticleSystem starExploreFx;
    [SerializeField] ParticleSystem starTrailFx;

    private Tweener rotateItemTween;
    private Tweener moveItemTween;
    private Tweener scaleItemTween;

    private void Awake()
    {
        starExploreFx.Stop();
        starTrailFx.Stop();
    }
    private void OnDestroy()
    {
        if (rotateItemTween != null) rotateItemTween?.Kill();
        if (scaleItemTween != null) scaleItemTween?.Kill();
        if (moveItemTween != null) moveItemTween?.Kill();
    }

    public void OnMoveToFlower(System.Action OnMagic, System.Action OnMoveOut)
    {
        starTrailFx.Play();
        transform.rotation = Quaternion.Euler(Vector3.forward * 30);
        rotateItemTween = transform.DORotate(Vector3.forward * -30, 1.5f).SetEase(Ease.Linear);
        moveItemTween = transform.DOMove(startPos, 1.5f)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            scaleItemTween = transform.DOPunchScale(Vector3.one * 0.3f, 0.5f, 1)
            .SetEase(Ease.Flash)
            .OnComplete(() =>
            {
                starExploreFx.Play();
                GUIManager.instance.PlayLighting(transform);
                SoundManager.instance.PlayOtherSfx(SfxOtherType.Magic);
                OnMagic?.Invoke();
                moveItemTween = transform.DOMoveY(UISetupManager.Instance.outsideDown.position.y, 1f)
                .SetDelay(1)
                .OnComplete(() =>
                {
                    OnMoveOut?.Invoke();
                });
            });
        });
    }
}
