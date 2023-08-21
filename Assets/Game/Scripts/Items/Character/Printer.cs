using DG.Tweening;
using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Printer : BackItem
{
    [SerializeField] Transform paperOutZone;
    [SerializeField] Transform paperInZone;
    [SerializeField] Image[] lidImgs;
    [SerializeField] CarryItem paperOutPb;
    [SerializeField] Image paperPhoto;
    [SerializeField] Transform bodyZone;

    private int countShakeRotate;
    private CarryItem paperPrinted;
    private Tweener rotateTween;
    private Tween delayItemTween;
    private Tweener moveTween;
    private int stateIdx;
    private Tweener fadeTween;

    private void Awake()
    {
        isDrag = false;
        isDance = true;
        lidImgs[0].gameObject.SetActive(true);
        lidImgs[1].gameObject.SetActive(false);
        paperPhoto.DOFade(0, 0);
    }
    private void Start()
    {
        canClick = false;
     //   delayItemTween = DOVirtual.DelayedCall(2, () =>
     //   {
            AssignItem();
     //   });
    }
    private void OnDestroy()
    {
        if (moveTween != null) moveTween?.Kill();
        if (rotateTween != null) rotateTween?.Kill();
        if (delayItemTween != null) delayItemTween?.Kill();
    }


    void AssignItem()
    {
        fadeTween = paperPhoto.DOFade(1, 1).OnComplete(() =>
        {
            canClick = true;
        });
    }

    public void OnPrinting(System.Action OnComplete)
    {
        canClick = false;
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Printing);
        tweenScale = bodyZone.DOPunchScale(new Vector3(-0.2f, 0.1f, 0), 0.5f, 4);
        rotateTween = bodyZone.DORotate(Vector3.forward * 3, 0.15f).OnComplete(() =>
        {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Printing);
            rotateTween = bodyZone.DORotate(Vector3.forward * -3, 0.15f).OnComplete(() =>
            {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Printing);
                rotateTween = bodyZone.DORotate(Vector3.zero, 0.25f).OnComplete(() =>
                {
                    countShakeRotate++;
                    delayItemTween = DOVirtual.DelayedCall(0.25f, () =>
                    {
                        if (countShakeRotate == 3)
                        {
                            OnExportPicture(() =>
                            {
                                OnClickLid();
                                AssignItem();
                                OnComplete?.Invoke();
                            });
                        }
                        else
                        {
                            OnPrinting(OnComplete);
                        }
                    });
                });
            });
        });
    }

    public void OnExportPicture(System.Action OnComplete)
    {
        moveTween = paperPrinted.transform.DOMove(paperOutZone.GetChild(1).position, 1)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                paperPrinted.transform.SetParent(transform);
                OnComplete?.Invoke();
            });
    }
    void OnClickLid()
    {
        lidImgs[stateIdx].gameObject.SetActive(false);
        stateIdx = 1 - stateIdx;
        lidImgs[stateIdx].gameObject.SetActive(true);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (!canClick) return;

        countShakeRotate = 0;
        OnClickLid();
        paperPrinted = Instantiate(paperOutPb, paperOutZone.GetChild(0));
        paperPrinted.GetMainPanel(GUIManager.instance.CurPanel, GUIManager.instance.CurGround);
        paperPhoto.DOFade(0, 0);
        OnPrinting(null);

        //      EventDispatcher.Instance.Dispatch(new EventKey.OnClickBackItem { printer = this });
    }
}
