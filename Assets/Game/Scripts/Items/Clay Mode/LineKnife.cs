using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineKnife : ItemMove
{
    [SerializeField] Sprite verticalSprite;
    [SerializeField] Sprite horizontalSprite;
    [SerializeField] Transform verticalBladeZone;
    [SerializeField] Transform horizontalBladeZone;

    Transform compareTrans;

    public Transform CompareTrans { get => compareTrans; }

    //public override void OnBeginDrag(PointerEventData eventData)
    //{
    //    base.OnBeginDrag(eventData);
    //}
    //public override void OnDrag(PointerEventData eventData)
    //{
    //    base.OnDrag(eventData);
    //}
    //public override void OnEndDrag(PointerEventData eventData)
    //{
    //    base.OnEndDrag(eventData);
    //}
    private void Start()
    {
    }

    public void OnChangeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Horizontal:
                itemImg.sprite = horizontalSprite;
                itemImg.SetNativeSize();
                compareTrans = horizontalBladeZone;
                break;
            case Direction.Vertical:
                itemImg.sprite = verticalSprite;
                itemImg.SetNativeSize();
                compareTrans = verticalBladeZone;
                break;
        }
    }

    public enum Direction
    {
        Horizontal,
        Vertical,
    }
}
