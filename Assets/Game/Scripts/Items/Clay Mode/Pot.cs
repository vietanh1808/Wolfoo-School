using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pot : ItemMove
{
    [SerializeField] Image maskImg;
    [SerializeField] Transform flowerZone;
    [SerializeField] Transform flowerCloneZone;

    private Sprite curMainSprite;
    private Sprite curMaskSprite;
    private Vector2 curFlowerPos;
    private Vector2 curMaskPos;
    private Tweener fadeTween;
    private bool isHasFlower;

    public Transform FlowerZone { get => flowerZone; }
    public Transform FlowerCloneZone { get => flowerCloneZone; }

    private void Start()
    {
        curMainSprite = itemImg.sprite;
        curMaskSprite = maskImg.sprite;
        curFlowerPos = flowerZone.localPosition;
        curMaskPos = maskImg.transform.localPosition;
    }

    private void OnEnable()
    {
        EventDispatcher.Instance.RegisterListener<EventKey.OnBeginDragItem>(GetBeginDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnEndDragItem>(GetEndDragItem);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener<EventKey.OnBeginDragItem>(GetBeginDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnEndDragItem>(GetEndDragItem);
    }

    private void GetBeginDragItem(EventKey.OnBeginDragItem item)
    {
        if(item.potItem != null)
        {
            itemImg.sprite = item.potItem.ItemSprite;
            itemImg.SetNativeSize();

            if (fadeTween != null) fadeTween?.Kill();
            fadeTween = itemImg.DOFade(0, 0);
            fadeTween = itemImg.DOFade(0.8f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            maskImg.gameObject.SetActive(false);
        }
    }

    private void GetEndDragItem(EventKey.OnEndDragItem item)
    {
        if (item.potItem != null)
        {
            flowerZone.localPosition = curFlowerPos;
            maskImg.transform.localPosition = curMaskPos;

            if (fadeTween != null) fadeTween?.Kill();
            fadeTween = itemImg.DOFade(isHasFlower ? 1 : 0, 0.5f).OnComplete(() =>
            {
                itemImg.sprite = curMainSprite;
                maskImg.sprite = curMaskSprite;
                itemImg.SetNativeSize();
                maskImg.SetNativeSize();

                maskImg.gameObject.SetActive(isHasFlower);
            });
        }
    }
    public void OnChangeSkin(Sprite mainSprite, Sprite maskSprite, Vector2 _localFlowerPos, Vector2 _localMaskPos)
    {
        isHasFlower = true;
        itemImg.sprite = mainSprite;
        maskImg.sprite = maskSprite;
        itemImg.SetNativeSize();
        maskImg.SetNativeSize();

        maskImg.transform.localPosition = _localMaskPos;
        if (moveTween != null) moveTween?.Kill();
        moveTween = flowerZone.transform.DOLocalMove(_localFlowerPos + Vector2.up * 100, 0.25f).OnComplete(() =>
        {
            moveTween = flowerZone.transform.DOLocalMove(_localFlowerPos, 0.25f);
        });
        
        curMainSprite = mainSprite;
        curMaskSprite = maskSprite;
        curFlowerPos = _localFlowerPos;
        curMaskPos = _localMaskPos;

        if (fadeTween != null) fadeTween?.Kill();
        fadeTween = itemImg.DOFade(1, 0.5f).OnComplete(() =>
        {
            maskImg.gameObject.SetActive(true);
        });
    }
}
