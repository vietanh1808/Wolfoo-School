using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Piano : BackItem
{
    [SerializeField] PianoAnimation pianoAnim;
    bool isPLayingMusic;
    private Tweener scalePianoTween;

    protected override void Start()
    {
    //    pianoAnim.gameObject.SetActive(false);
        base.Start();
        image.gameObject.SetActive(false);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (isPLayingMusic) return;
        isPLayingMusic = true;

        pianoAnim.OpenAnim();
        SoundManager.instance.PlayOtherBackMusic(OtherBackSoundType.Piano);

        scalePianoTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.5f).SetEase(Ease.OutBounce);
    }
    public override void GetEndDragBackItem(BackItem item, int id_)
    {
        base.GetEndDragBackItem(item, id_);

        if (!isPLayingMusic) return;
        isPLayingMusic = false;

        if(id != id_)
        {
            if (scalePianoTween != null)
            {
                scalePianoTween?.Kill();
                transform.localScale = startScale;
            }

            pianoAnim.CloseAnim();
            SoundManager.instance.PlayIngame();
        }
    }
}
