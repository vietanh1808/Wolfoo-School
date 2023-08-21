using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class GarbageCanItem : BackItem
{
    [SerializeField] List<Transform> holes;
    [SerializeField] List<Button> trashCanLidBtns;
    [SerializeField] List<Sprite> openSprites;
    [SerializeField] List<Sprite> closeSprites;

    private float distance;
    List<bool> isOpens = new List<bool>() { true, true, true };

    enum ColorTrashCan
    {
        Blue,
        Yellow,
        Green,
    }

    protected override void Start()
    {
        base.Start();

        trashCanLidBtns[(int)ColorTrashCan.Blue].onClick.AddListener(() => OnClickTrashCanlid(ColorTrashCan.Blue));
        trashCanLidBtns[(int)ColorTrashCan.Yellow].onClick.AddListener(() => OnClickTrashCanlid(ColorTrashCan.Yellow));
        trashCanLidBtns[(int)ColorTrashCan.Green].onClick.AddListener(() => OnClickTrashCanlid(ColorTrashCan.Green));
    }

    private void OnClickTrashCanlid(ColorTrashCan color)
    {
        int idx = (int)color;
        isOpens[idx] = !isOpens[idx];
        if (!isOpens[idx])
        {
            trashCanLidBtns[idx].transform.rotation = Quaternion.Euler(Vector3.zero);
            trashCanLidBtns[idx].image.sprite = closeSprites[idx];
            trashCanLidBtns[idx].image.SetNativeSize();
        }
        else
        {
            trashCanLidBtns[idx].transform.rotation = Quaternion.Euler(Vector3.right * 180);
            trashCanLidBtns[idx].image.sprite = openSprites[idx];
            trashCanLidBtns[idx].image.SetNativeSize();
        }
    }

    protected override void GetEndDragItem(EventKey.OnEndDragBackItem item)
    {
        base.GetEndDragItem(item);
        if (item.backItem == this) return;
        if (item.trash == null)
        {
            return;
        }
        int lastIdx = -1;
        float minDistance = 10;
        for (int i = 0; i < holes.Count; i++)
        {
            if (!isOpens[i]) continue;

            distance = Vector2.Distance(item.trash.transform.position, holes[i].position);
            if (distance <= 1)
            {
                if (minDistance > distance)
                {
                    minDistance = distance;
                    lastIdx = i;
                }
            }
        }

        if (lastIdx != -1)
        {
            item.trash.GetJumpToGarbage(holes[lastIdx].position);
         //   EventManager.OnJumpToGarbage?.Invoke(holes[lastIdx].position, item);
            return;
        }
        else
        {
          //  item.MoveToGround();
        }
    }

    //public override void GetEndDragBackItem(BackItem item, int id_)
    //{
    //    base.GetEndDragBackItem(item, id_);


    //    if (item.IsTrash) return;
    //    if (id == id_)
    //    {
    //       // MoveToGround();
    //        return;
    //    }

    //    int lastIdx = -1;
    //    float minDistance = 10;
    //    for (int i = 0; i < holes.Count; i++)
    //    {
    //        if (!isOpens[i]) continue;

    //        distance = Vector2.Distance(item.transform.position, holes[i].position);
    //        if(distance <= 1)
    //        {
    //            if(minDistance > distance)
    //            {
    //                minDistance = distance;
    //                lastIdx = i;
    //            }
    //        }
    //    }

    //    if(lastIdx != -1)
    //    {
    //        EventManager.OnJumpToGarbage?.Invoke(holes[lastIdx].position, item);
    //        return;
    //    }
    //    else
    //    {
    //        item.MoveToGround();
    //    }
    //}

}
