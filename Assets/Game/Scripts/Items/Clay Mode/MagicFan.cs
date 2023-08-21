using DG.Tweening;
using SCN;
using SCN.Tutorial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MagicFan : ItemMove, IPointerClickHandler
{
    [SerializeField] FanAnimation fanAnimation;
    [SerializeField] ParticleSystem spikyFx;

    private bool canClick;
    private Tween delayTween;
    private void OnDestroy()
    {
        if (delayTween != null) delayTween?.Kill();
        TutorialManager.Instance.Stop();

    }

    public void AssignClick()
    {
        canClick = true;
        TutorialManager.Instance.StartPointer(transform);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClick) return;
        canClick = false;

        SoundManager.instance.PlayOtherSfx(SfxOtherType.MagicRainbow);
        TutorialManager.Instance.Stop();
        EventDispatcher.Instance.Dispatch(new EventKey.OnClickItem { fan = this });
    }

    public void OnThrowing(Vector3 _endPos, System.Action OnComplete)
    {
        fanAnimation.PlayAnim();
        spikyFx.Play();
        delayTween = DOVirtual.DelayedCall(spikyFx.main.duration + 1, () =>
        {
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
            fanAnimation.PlayIdle();
            OnComplete?.Invoke();
        });
    }
}
