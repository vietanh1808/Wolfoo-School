using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Peeler : ItemDrag
{
    [SerializeField] PeelerAnimation peelerAnimation;

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        peelerAnimation.ExcuteAnim();
        if (!canDrag) return;
        EventDispatcher.Instance.Dispatch(new EventKey.OnBeginDragItem { peeler = this });
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (!canDrag) return;
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Draw);
        EventDispatcher.Instance.Dispatch(new EventKey.OnDragItem { peeler = this });
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        peelerAnimation.IdleAnim();
        if (!canDrag) return;
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragItem { peeler = this });
    }


}
