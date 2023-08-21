using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Trash : BackItem
{
    protected override void Start()
    {
        base.Start();
    }

    public void GetJumpToGarbage(Vector3 pos)
    {
        SetNotMoveToGround();
        DisableDrag();
        transform.DOScale(transform.localScale / 2, 0.5f);
        transform.DOJump(pos, 1, 1, 0.5f).OnComplete(() =>
        { 
            gameObject.SetActive(false);
        });
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragBackItem { trash = this, backItem = this });
    }

    //public override void GetEndDragBackItem(BackItem image_, int id_)
    //{
    //    base.GetEndDragBackItem(image_, id_);

    //    //if (id == id_)
    //    //{
    //    //    MoveToGround();
    //    //    return;
    //    //}
    //    // GetSiblingIndex(image_);
    //}

}
