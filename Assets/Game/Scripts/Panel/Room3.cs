using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room3 : Panel
{
    [SerializeField] Transform groundTrans;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] PlayerPanel mainPanelPb;
    [SerializeField] Image coverImg;
    [SerializeField] List<RectTransform> anchors;
    [SerializeField] float velocity = 0.01f;
    [SerializeField] Transform flowerZone;
    [SerializeField] Button clayModeBtn;
    [SerializeField] PanelType nextMode;

    // [SerializeField] string tagName;
    [SerializeField] Button backBtn;
    private float distanceLeft;
    private float distanceRight;
    private int nextFlowerIdx;
    private Tween delayTween;

    public Transform GroundTrans { get => groundTrans; }
    public ScrollRect ScrollRect { get => scrollRect; }

    private void Awake()
    {
        var panel = Instantiate(mainPanelPb, transform);
        panel.AssignPanel(PanelType.Room3);
    }
    void Start()
    {
        EventDispatcher.Instance.RegisterListener<EventKey.OnModeComplete>(GetModeComplete);
        backBtn.onClick.AddListener(OnBack);
        clayModeBtn.onClick.AddListener(OnPlayMode);

        clayModeBtn.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 4).SetLoops(-1);

        DOVirtual.DelayedCall(0.1f, () =>
        {
            GUIManager.instance.room3 = this;
            EventManager.GetMainPanel?.Invoke(scrollRect.content.gameObject, groundTrans.gameObject);
        });

     //   coverImg.gameObject.SetActive(false);
        DOVirtual.Float(0, 1, 5, OnUpdateScroll).SetEase(Ease.Linear).OnComplete(() =>
        {
            DOVirtual.Float(1, 0, 2, OnUpdateScroll).OnComplete(() =>
            {
                coverImg.gameObject.SetActive(false);
            });
        });
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener<EventKey.OnModeComplete>(GetModeComplete);
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

    private void OnPlayMode()
    {
        EventManager.OnPlaygame?.Invoke(this, nextMode);
    }

    private void GetModeComplete(EventKey.OnModeComplete item)
    {
        if(item.flowerPot != null)
        {
            if(nextFlowerIdx >= flowerZone.childCount)
            {
                nextFlowerIdx = 0;
            }
            if(flowerZone.GetChild(nextFlowerIdx).childCount > 0)
                flowerZone.GetChild(nextFlowerIdx).GetChild(0).gameObject.SetActive(false);
            item.flowerPot.transform.SetParent(flowerZone.GetChild(nextFlowerIdx));
            item.flowerPot.transform.localPosition = Vector3.zero;
            item.flowerPot.transform.SetAsFirstSibling();
            item.flowerPot.transform.localScale = Vector3.one * 0.7f;

            GUIManager.instance.rainbowFx.transform.position = item.flowerPot.transform.position;
            GUIManager.instance.rainbowFx.Play();

            delayTween = DOVirtual.DelayedCall(2, () =>
            {
                GUIManager.instance.rainbowFx.Stop();
            });
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Hoow);

            nextFlowerIdx++;
        }
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
}
