using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WoodHorse : BackItem
{
    [SerializeField] WoodHorseAnimation anim;
    [SerializeField] Transform sitZone;
    private float distance;

    private void Start()
    {
        isClick = true;
        anim.PlayIdle();
    }

    protected override void GetEndDragItem(EventKey.OnEndDragBackItem item)
    {
        base.GetEndDragItem(item);
        if (item.backItem == this) return;
        if (item.wolfoo != null)
        {
            distance = Vector2.Distance(item.wolfoo.transform.position, transform.position);
            if(distance < 2)
            {
                item.wolfoo.OnSitDownHorse(sitZone);
                anim.PlayAnim(true);
            }
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        anim.PlayAnim(false);
    }
}
