using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInRefrigerator : BackItem
{
    protected override void Start()
    {
        base.Start();
        isInBag = true;
        isFood = true;
    }
    public void AssignItem()
    {
        content = GUIManager.instance.mainPanel.ScrollRect.content.gameObject;
        ground = GUIManager.instance.mainPanel.GroundTrans.gameObject;
        base.Start();
        isInBag = true;
        isFood = true;
        isDrag = true;
        isComparePos = true;
    }

    public override void GetEndDragBackItem(BackItem item, int id_)
    {
        base.GetEndDragBackItem(item, id_);

        if(id == id_)
        {
        //    MoveToGround();
            return;
        }
    }

}
