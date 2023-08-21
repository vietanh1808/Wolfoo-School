using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room2 : Panel
{
    [SerializeField] Transform groundTrans;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] PlayerPanel mainPanelPb;
    [SerializeField] Image coverImg;
    [SerializeField] List<RectTransform> anchors;
    [SerializeField] float velocity = 0.01f;

    // [SerializeField] string tagName;
    [SerializeField] Button backBtn;
    private float distanceLeft;
    private float distanceRight;
    private Tween delayTween;

    private void Awake()
    {
        var panel = Instantiate(mainPanelPb, transform);
        panel.AssignPanel(PanelType.Room2);
    }
    void Start()
    {
        backBtn.onClick.AddListener(OnBack);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            GUIManager.instance.room2 = this;
            EventManager.GetMainPanel?.Invoke(scrollRect.content.gameObject, groundTrans.gameObject);
        });

        DOVirtual.Float(0, 1, 5, OnUpdateScroll).SetEase(Ease.Linear).OnComplete(() =>
        {
            DOVirtual.Float(1, 0, 2, OnUpdateScroll).OnComplete(() =>
            {
                coverImg.gameObject.SetActive(false);
            });
        });
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

        if (distanceLeft < 2)
        {
            if (scrollRect.horizontalScrollbar.value == 0) return;
            scrollRect.horizontalScrollbar.value -= velocity;
        }

        if (distanceRight < 2)
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
        EventManager.OnBackPanel?.Invoke(this, PanelType.Main, false);
    }

    void LoadRoom()
    {
     //   HUDSystem.Instance.Show<MainPanel>(null, UIPanels<HUDSystem>.ShowType.Duplicate);
        
    }
    public Transform GroundTrans { get => groundTrans; }
    public ScrollRect ScrollRect { get => scrollRect; }
}
