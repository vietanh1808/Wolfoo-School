using DG.Tweening;
using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Leaf : ItemDrag
{
    private Tweener fadeTween;

    private void Awake()
    {
        itemImg = GetComponent<Image>();
    }

    private void OnDestroy()
    {
        if (fadeTween != null) fadeTween?.Kill();
    }
    public void AssignItem(int _id, Sprite sprite)
    {
        id = _id;
        itemImg.sprite = sprite;
        itemImg.SetNativeSize();
    }
    public void AssignItem(int _id)
    {
        id = _id;
    }
    public void InitStartScale(Vector3 _scale)
    {
        transform.localScale = _scale;
        startScale = _scale;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (!canDrag) return;
        transform.parent.SetAsLastSibling();
        EventDispatcher.Instance.Dispatch(new EventKey.OnBeginDragItem { leaf = this });
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (!canDrag) return;
        EventDispatcher.Instance.Dispatch(new EventKey.OnDragItem { leaf = this });
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (!canDrag) return;
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragItem { leaf = this });
    }

    public void GetTut()
    {
        transform.SetAsLastSibling();
        if (fadeTween != null) fadeTween?.Kill();
        fadeTween = itemImg.DOFade(0.8f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    public void StopTut()
    {
        if (fadeTween != null) fadeTween?.Kill();
        fadeTween = itemImg.DOFade(0, 0.5f);
    }
}
