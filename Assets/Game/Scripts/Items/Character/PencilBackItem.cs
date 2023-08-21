using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using SCN;

public class PencilBackItem : BackItem
{
    [SerializeField] bool isStand = true;
    private Tweener rotateHorizontalTween;
    private Tweener rotateVertialTween;
    private bool isStanding;

    protected override void Start()
    {
        base.Start();
        isDrag = true;
        isPencil = true;
        isComparePos = true;
        isCarryItem = true;
        isInBag = true;
        isStandTable = true;
    }

    public void OnStandTable()
    {
        isStanding = true;

        if (rotateHorizontalTween != null)
        {
            rotateHorizontalTween?.Kill();
        }
        if(rotateVertialTween != null)
        {
            rotateVertialTween?.Kill();
        }
        rotateVertialTween = transform.DORotate(Vector3.zero, 0.5f);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        isStanding = false;
        if (rotateHorizontalTween != null)
        {
            rotateHorizontalTween?.Kill();
        }
        rotateVertialTween = transform.DORotate(Vector3.zero, 0.5f);
    }
    //public override void GetEndDragBackItem(BackItem item, int id_)
    //{
    //    base.GetEndDragBackItem(item, id_);
    //    if (id != id_)
    //    {
    //        return;
    //    }

    //    MoveToGround();
    //}
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
            rotateHorizontalTween = transform.DORotate(Vector3.forward * 90, 0.5f);
        }
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragBackItem { pencil = this, backItem = this });
    }
}
