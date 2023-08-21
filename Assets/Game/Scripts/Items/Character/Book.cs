using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using SCN;

public class Book : BackItem
{
    [SerializeField] Sprite openSprite;
    [SerializeField] bool isStand = true;
    private Tweener rotateHorizontalTween;
    private Tweener rotateVertialTween;
    private bool isStanding;

    protected override void Start()
    {
        base.Start();
        isDrag = true;
        isBook = true;
        isComparePos = true;
        isCarryItem = true;
        isInBag = true;
        isStandTable = true;
    }

    public void GetCarryBook()
    {
        image.sprite = openSprite;
        image.SetNativeSize();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        isStanding = false;
        if (rotateHorizontalTween != null)
        {
            rotateHorizontalTween?.Kill();
        }
        rotateVertialTween = transform.DORotate(Vector3.zero, 0.25f);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (isStanding) return;
        if (isStand)
        {
            if (rotateVertialTween != null)
            {
                rotateVertialTween?.Kill();
            }
            rotateHorizontalTween = transform.DORotate(Vector3.forward * 90, 0.25f);
        }
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragBackItem { book = this, backItem = this });
    }
    protected override void GetEndDragItem(EventKey.OnEndDragBackItem item)
    {
        base.GetEndDragItem(item);
    }
    //public override void OnEndDrag(PointerEventData eventData)
    //{
    //    base.OnEndDrag(eventData);
    //    if (isStanding) return;

    //    MoveToGround();

    //    if (isStand)
    //    {
    //        if (rotateVertialTween != null)
    //        {
    //            rotateVertialTween?.Kill();
    //        }
    //        rotateHorizontalTween = transform.DORotate(Vector3.forward * 90, 0.25f);
    //    }
    //}
    public void OnStandTable()
    {
        isStanding = true;

        if (rotateHorizontalTween != null)
        {
            rotateHorizontalTween?.Kill();
        }
        if (rotateVertialTween != null)
        {
            rotateVertialTween?.Kill();
        }
        rotateVertialTween = transform.DORotate(Vector3.zero, 0.5f);
    }
}
