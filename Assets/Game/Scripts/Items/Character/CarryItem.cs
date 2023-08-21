using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarryItem : BackItem
{
    protected override void Start()
    {
        base.Start();
        isDrag = true;
        isInBag = true;
        isCarryItem = true;
        isStandTable = true;
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragBackItem { carryItem = this, backItem = this });
    }
}
