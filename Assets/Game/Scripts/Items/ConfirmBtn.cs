using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmBtn : MonoBehaviour
{
    [SerializeField] Button btnZone;
    
    float time = 1;

    private bool isShow;
    private Vector3 startBtnScale;
    private Tweener rotateTween;
    private Tween delayTween;
    private Tweener moveTween;

    public bool IsShowed { get => isShow; }

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(Vector3.forward * -90);
    }

    private void Start()
    {
        startBtnScale = transform.localPosition;
        btnZone.onClick.AddListener(OnClick);
    }
    private void OnDestroy()
    {
        if (delayTween != null) delayTween?.Kill();
        if (rotateTween != null) rotateTween?.Kill();
        if (moveTween != null) moveTween?.Kill();
    }

    private void OnClick()
    {
        Hide();
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
        EventDispatcher.Instance.Dispatch(new EventKey.OnClickItem { confirmBtn = this });
    }

    public void Show()
    {
        isShow = true;
        if (rotateTween != null)
        {
            rotateTween?.Kill();
        }
        btnZone.interactable = true;
        rotateTween = transform.DORotate(Vector3.forward * 20, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            rotateTween = transform.DORotate(Vector3.forward * -20, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                rotateTween = transform.DORotate(Vector3.forward * 10, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    rotateTween = transform.DORotate(Vector3.forward * -10, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        rotateTween = transform.DORotate(Vector3.forward * 5, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            rotateTween = transform.DORotate(Vector3.zero, 0.5f)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                ScaleTut();
                            });
                        });
                    });
                });
            });
        });
    }

    void ScaleTut()
    {
        moveTween = transform.DOPunchPosition(Vector3.down * 15, 0.5f, 4)
            .SetDelay(2)
            .OnComplete(() =>
            {
                ScaleTut();
            });
    }

    public void Hide()
    {
        isShow = false;
        btnZone.interactable = false;
        if (rotateTween != null)
        {
            rotateTween?.Kill();
        }
        if (moveTween != null)
        {
            moveTween?.Kill();
        }
        transform.localPosition = startBtnScale;
        rotateTween = transform.DORotate(Vector3.forward * 90, 1);
    }
}
