using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cabinet : BackItem
{
    [SerializeField] List<Transform> frontDeskTrans;
    private float xRange;
    private bool isDoorOpen;

    protected override void Start()
    {
        base.Start();
        xRange = frontDeskTrans[0].GetComponent<RectTransform>().rect.width / 100 / 3;
    }

    protected override void GetClickBackItem(EventKey.OnClickBackItem item)
    {
        base.GetClickBackItem(item);
        if(item.door != null)
        {
            isDoorOpen = item.door.CurType == Door.Type.Open;
        }
    }

 //   public override void GetEndDragBackItem(BackItem item, int id_)
  //  {
  //      base.GetEndDragBackItem(item, id_);

        //if (id == id_)
        //{
        //    return;
        //}
        //if (!isDoorOpen) return;
        //if (Mathf.Abs(item.transform.position.x - frontDeskTrans[0].position.x) > xRange) return;

        //var curIdx = -1;
        //foreach (var trans in frontDeskTrans)
        //{
        //    if (item.transform.position.y > trans.position.y)
        //    {
        //        curIdx = frontDeskTrans.IndexOf(trans);
        //    }
        //}

        //if (curIdx < 0) return;
        //if (item.transform.position.y > frontDeskTrans[curIdx].position.y)
        //{
        //    if (item.IsBook || item.IsPencil)
        //    {
        //        EventManager.OnStandTable?.Invoke(id_, item);
        //    }

        //    if (item.IsWolfoo) return;

        //    item.transform.SetParent(transform);
        //    item.MoveToDesk(frontDeskTrans[curIdx].localPosition);
        //}
  //  }
}
