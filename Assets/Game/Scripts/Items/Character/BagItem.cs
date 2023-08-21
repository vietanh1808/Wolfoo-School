using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using SCN;

public class BagItem : BackItem
{
    [SerializeField] List<Sprite> sprites;
    int curIdx = 0;
    private float distance_;

    int count = 0;
    List<BackItem> curItems = new List<BackItem>();
    private Vector2 range;
    private bool isItemJumpOutSide;

    protected override void Start()
    {
        base.Start();
        skinType = SkinBackItemType.Bag;
        isClick = true;
        isStandTable = true;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        count = 0;
    }
    protected override void GetEndDragItem(EventKey.OnEndDragBackItem item)
    {
        base.GetEndDragItem(item);
        if (item.backItem == null || item.backItem == this) return;
        if (id == item.id)
        {
            return;
        }

        if (curIdx == 0) return;
        if (!item.backItem.IsInBag) return;

        distance_ = Vector2.Distance(item.backItem.transform.position, transform.position);
        if (distance_ <= 2)
        {
            item.backItem.SetNotMoveToGround();
            item.backItem.DisableDrag();
            item.backItem.transform.SetAsLastSibling();
            item.backItem.transform.DOScale(item.backItem.transform.localScale / 2, 0.5f);
            item.backItem.transform.DOJump(transform.position + Vector3.up * 0.2f, 1, 1, 0.5f).OnComplete(() =>
            {
                item.backItem.transform.SetParent(transform);
                item.backItem.gameObject.SetActive(false);
            });
            curItems.Add(item.backItem);
        }
        else
        {
       //     item.backItem.MoveToGround();
        }
    }
    //public override void GetEndDragBackItem(BackItem item, int id_)
    //{
        //if(id == id_)
        //{
        //    MoveToGround();
        //    return;
        //}

        //if ( curIdx == 0) return;
        //if (!item.IsInBag) return;

        //distance_ = Vector2.distance_(item.transform.position, transform.position);
        //if(distance_ <= 2)
        //{
        //    item.SetNotMoveToGround();
        //    item.DisableDrag();
        //    item.transform.SetAsLastSibling();
        //    item.transform.DOScale(item.transform.localScale / 2, 0.5f);
        //    item.transform.DOJump(transform.position + Vector3.up * 0.2f, 1, 1, 0.5f).OnComplete(() =>
        //    {
        //        item.transform.SetParent(transform);
        //        item.gameObject.SetActive(false);
        //    });
        //    curItems.Add(item);
        //}
        //else
        //{
        //    item.MoveToGround();
        //}
    //}
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragBackItem { backItem = this, bag = this });
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (!isItemJumpOutSide)
        {
            range = new Vector2(transform.position.x - 4, transform.position.x + 4);
            count++;
            if (count > 2)
            {
                count = 0;
                if (curItems.Count > 0)
                {
                    isItemJumpOutSide = true;
                    foreach (var item in curItems)
                    {
                        float rd = UnityEngine.Random.Range(range.x, range.y);
                        item.JumpOutSide(new Vector3(rd, transform.position.y, 0));
                    }
                    curItems.Clear();
                    isItemJumpOutSide = false;
                }
            }
        }

        OnPunchScale();
        curIdx = 1 - curIdx;
        image.sprite = sprites[curIdx];
        image.SetNativeSize();
    }
}
