using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeFlowerItem : ItemDrag, IPointerDownHandler
{
    private Tweener scaleItemTween;
    private bool canClick;

    private void Start()
    {
        EventDispatcher.Instance.RegisterListener<EventKey.OnClickItem>(GetClickItem);
    }

    private void OnDestroy()
    {
        if (scaleItemTween != null) scaleItemTween?.Kill();

        EventDispatcher.Instance.RemoveListener<EventKey.OnClickItem>(GetClickItem);
    }

    private void GetClickItem(EventKey.OnClickItem item)
    {
        if(item.flowerItem != null)
        {
            canClick = false;
        }
    }

    public void AssignClick()
    {
        canClick = true;
    }

    public void AssignItem(int _id, Sprite _sprite)
    {
        id = _id;
        itemImg.sprite = _sprite;
        itemImg.SetNativeSize();
    }

    public void JumpOutBox(Vector3 _endPos, Transform _endParent)
    {
        scaleItemTween = transform.DOScale(Vector3.one, 0.5f);
        JumpToEndPos(_endPos, () =>
        {
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
            transform.SetParent(_endParent);
        }, 5);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canClick) return;
        EventDispatcher.Instance.Dispatch(new EventKey.OnClickItem { flowerItem = this });
    }
}
