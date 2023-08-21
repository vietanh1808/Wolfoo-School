using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChairItem : BackItem
{
    private float distance_;
    [SerializeField] Transform sitTrans;

    protected override void Start()
    {
        base.Start();
        isDrag = false;
      //  skinType = SkinBackItemType.Chair;
    }
    protected override void GetEndDragItem(EventKey.OnEndDragBackItem item)
    {
        base.GetEndDragItem(item);
        if (item.backItem == this) return;
        if (item.wolfoo != null)
        {
            distance_ = Vector2.Distance(item.wolfoo.transform.position, sitTrans.position);
            if (distance_ <= 1)
            {
                item.wolfoo.OnSitToChair(sitTrans.position);
            }
        }
    }
    //public override void GetEndDragBackItem(BackItem item, int id_)
    //{
    //    base.GetEndDragBackItem(item, id_);
    //    if (id == id_)
    //    {
    //     //   MoveToGround();
    //        return;
    //    }

    //    if (item.IsWolfoo)
    //    {
    //        distance_ = Vector2.distance_(item.transform.position, sitTrans.position);
    //        if (distance_ <= 2)
    //        {
    //            EventManager.OnWolfooSitToChair?.Invoke(id_, item, sitTrans.position);
    //        }
    //    }
    //}
}
