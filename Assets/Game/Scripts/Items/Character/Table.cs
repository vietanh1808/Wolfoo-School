using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : BackItem
{
    [SerializeField] List<Transform> frontDeskTrans;
    private float distance_;
    private float xRange;
    private BackItem compareItem;

    protected override void Start()
    {
        base.Start();
        //xRange = frontDeskTrans[0].GetComponent<RectTransform>().rect.width / 100 /3;
    }

    protected override void GetEndDragItem(EventKey.OnEndDragBackItem item)
    {
        base.GetEndDragItem(item);

        if (item.backItem == this) return;
        compareItem = item.backItem;

        if (compareItem == null || !item.backItem.IsStandTable) return;
   //     if (Vector2.Distance(compareItem.transform.position, transform.position) > 2) return;

        if (compareItem.transform.position.y < frontDeskTrans[0].position.y ||
            compareItem.transform.position.x < frontDeskTrans[0].position.x ||
            compareItem.transform.position.x > frontDeskTrans[1].position.x)
        {
            return;
        }

        if (compareItem.transform.position.y > frontDeskTrans[1].position.y)
        {
            item.backItem.MoveToDesk(new Vector3(compareItem.transform.position.x, frontDeskTrans[1].position.y, 0), transform);
        }
        else
        {
            item.backItem.MoveToDesk(item.backItem.transform.position, transform);
        }
        if (item.backItem.IsBook)
        {
            item.book.OnStandTable();
        }
        if (item.backItem.IsPencil)
        {
            item.pencil.OnStandTable();
        }
    }

#if UNITY_EDITOR
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(((RectTransform)frontDeskTrans[0].transform).position, ((RectTransform)frontDeskTrans[1].transform).position);
    //    if (compareItem != null)
    //    {
    //        var min = frontDeskTrans[1].transform.position;
    //        var max = frontDeskTrans[0].transform.position;
    //        Gizmos.color = compareItem.transform.position.y < max.y &&
    //            compareItem.transform.position.y > min.y ?
    //            Color.green : Color.red;
    //        Gizmos.DrawLine(compareItem.transform.position, frontDeskTrans[0].transform.position);
    //        Gizmos.color = compareItem.transform.position.y < max.y &&
    //         compareItem.transform.position.y > min.y ?
    //         Color.green : Color.red;
    //        Gizmos.DrawLine(compareItem.transform.position, frontDeskTrans[1].transform.position);

    //        if (compareItem.transform.position.y < frontDeskTrans[0].position.y ||
    //             compareItem.transform.position.x > frontDeskTrans[0].position.x ||
    //             compareItem.transform.position.x < frontDeskTrans[1].position.x)
    //        {
    //            Gizmos.color = Color.white;
    //            Gizmos.DrawLine(compareItem.transform.position, frontDeskTrans[1].transform.position);
    //        }
    //        else
    //        {
    //            Gizmos.color = Color.black;
    //            Gizmos.DrawLine(compareItem.transform.position, frontDeskTrans[1].transform.position);
    //        }
    //    }
    //}
#endif
}
