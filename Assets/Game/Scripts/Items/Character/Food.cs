using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Food : BackItem
{
    protected override void Start()
    {
        base.Start();
        isDrag = true;
    }

    public override void GetEndDragBackItem(BackItem item, int id_)
    {
        base.GetEndDragBackItem(item, id_);

        if(id == id_)
        {
      //      MoveToGround();
            return;
        }
    }

}
