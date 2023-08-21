using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class MainPanel : Panel
{
    [SerializeField] Transform groundTrans;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] PlayerPanel mainPanelPb;
    [SerializeField] Image coverImg;
    [SerializeField] Button backBtn;
    [SerializeField] List<RectTransform> anchors;
    [SerializeField] float velocity;

    private float distanceLeft;
    private float distanceRight;
    private Tween delayTween;

    public Transform GroundTrans { get => groundTrans; }
    public ScrollRect ScrollRect { get => scrollRect; }

    private void Awake()
    {
        var panel = Instantiate(mainPanelPb, transform);
        panel.AssignPanel(PanelType.Main);
    }
    void Start()
    {
        base.Start();
        backBtn.onClick.AddListener(OnBack);
        coverImg.gameObject.SetActive(true);

        DOVirtual.DelayedCall(0.1f, () =>
        {
            GUIManager.instance.mainPanel = this;
            EventManager.GetMainPanel?.Invoke(scrollRect.content.gameObject, groundTrans.gameObject);
        });

        DOVirtual.Float(0, 1, 5, OnUpdateScroll).SetEase(Ease.Linear).OnComplete(() =>
        {
            DOVirtual.Float(1, 0, 2, OnUpdateScroll).OnComplete(() =>
            {
                coverImg.gameObject.SetActive(false);
            });
        });
        //coverImg.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        EventManager.OnDragBackItem += GetDragBackItem;

        if (delayTween != null) delayTween?.Kill();
        delayTween = DOVirtual.DelayedCall(0.2f, () =>
        {
            GUIManager.instance.AssignPanel(scrollRect.content.gameObject, groundTrans.gameObject);
        });
    }
    private void OnDisable()
    {
        EventManager.OnDragBackItem += GetDragBackItem;
    }

    private void GetDragBackItem(Transform curTrans)
    {
        distanceLeft = curTrans.position.x - anchors[0].position.x;
        distanceRight = anchors[1].position.x - curTrans.position.x;

        if( distanceLeft < 2)
        {
            if (scrollRect.horizontalScrollbar.value == 0) return;
            scrollRect.horizontalScrollbar.value -= velocity;
        }

        if(distanceRight < 2)
        {
            if (scrollRect.horizontalScrollbar.value == 1) return;
            scrollRect.horizontalScrollbar.value += velocity;
        }
    }

    private void OnUpdateScroll(float value)
    {
        scrollRect.horizontalScrollbar.value = value;
    }
    private void OnBack()
    {
        EventManager.OnBackPanel?.Invoke(this, PanelType.Intro, false);
    }

    void LoadRoom()
    {
    }
}
