using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MopItem : BackItem
{
    protected override void Start()
    {
        base.Start();
        isComparePos = true;
        isDrag = true;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        GameManager.instance.GetCurrentPosition(transform);
        EventManager.OnDragMop?.Invoke(transform.position);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        EventManager.OnEndDragMop?.Invoke();

        MoveToStartPos();
    }
 
}
