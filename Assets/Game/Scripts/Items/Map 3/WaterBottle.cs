using DG.Tweening;
using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaterBottle : BackItem
{
    [SerializeField] WaterBottleAnimation bottleAnimation;
    [SerializeField] Transform sprinklerZone;
    [SerializeField] Transform compareZone;

    private Vector3 offset;

    public Transform CompareZone { get => compareZone; }

    override protected void Start()
    {
        isDrag = true;
        isComparePos = true;
        canDrag = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (!canDrag) return;
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragBackItem { waterBottle = this, backItem = this });
    }

    public void OnPourWater(Vector3 _endPos, System.Action OnPour,  System.Action OnComplete = null)
    {
        if (!canDrag) return;
        canDrag = false;
        canMoveToGround = false;

        offset = transform.position - sprinklerZone.position;
        DisableDrag();
        tweenMove = transform.DOMove(_endPos + offset, 0.5f).OnComplete(() =>
        {
            delayTween = DOVirtual.DelayedCall(
                bottleAnimation.GetTimeAnimation(WaterBottleAnimation.AnimState.Play) - 0.2f, () =>
            {
                OnPour?.Invoke();
            });
            bottleAnimation.PlayAnim(() =>
            {
               // OnPour?.Invoke();
                canDrag = true;
                canMoveToGround = true;

                MoveToGround();
                OnComplete?.Invoke();
            });
        });
    }
}
