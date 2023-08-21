using DG.Tweening;
using SCN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Trunk : ItemDrag
{
    private Tweener scaleItemTween;
    private Tweener fadeTween;
    private Vector3 startLocalPos;
    private bool isShown;
    private int idxBoard;
    private bool isPlugging;
    private Sprite curMainSprite;
    private Vector2 _endPos;

    public Sprite Sprite { get => itemImg.sprite; }
    public bool IsShown { get => isShown; }
    public bool IsPlugging { get => isPlugging; }

    private void Start()
    {
        startLocalPos = transform.localPosition;
    }

    private void OnDestroy()
    {
        if (scaleItemTween != null) scaleItemTween?.Kill();
        if (fadeTween != null) fadeTween?.Kill();
    }
    public void AssignItem(int _id, Sprite sprite)
    {
        id = _id;
        itemImg.sprite = sprite;
        itemImg.SetNativeSize();
        curMainSprite = sprite;
    }
    public void AssignTut(Sprite sprite)
    {
        itemImg.sprite = sprite;
        itemImg.SetNativeSize();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (!canDrag) return;
        transform.parent.SetAsLastSibling();
        EventDispatcher.Instance.Dispatch(new EventKey.OnBeginDragItem { trunk = this });
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (!canDrag) return;
        EventDispatcher.Instance.Dispatch(new EventKey.OnDragItem { trunk = this });
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (!canDrag) return;
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragItem { trunk = this });
    }

    public void GetTut()
    {
        transform.SetAsLastSibling();
        if (fadeTween != null) fadeTween?.Kill();
        fadeTween = itemImg.DOFade(0, 0f);
        fadeTween = itemImg.DOFade(0.8f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    public void StopTut()
    {
        itemImg.sprite = curMainSprite;
        itemImg.SetNativeSize();

        if (fadeTween != null) fadeTween?.Kill();
        fadeTween = itemImg.DOFade(1, 0.5f).OnComplete(() =>
        {
        });
    }

    public void OnPlugin(System.Action OnComplete)
    {
        canDrag = false;
        isPlugging = true;
        curMainSprite = itemImg.sprite;
        if (moveTween != null)
        {
            moveTween?.Kill();
        }
        if (scaleTween != null)
        {
            scaleTween?.Kill();
            transform.localScale = startScale;
        }

        moveTween = transform.DOMove(endTrans.position, 0.5f);
        scaleTween = transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1)
            .OnComplete(() =>
            {
                transform.SetParent(endParent);
                moveTween = transform.DOPunchPosition(Vector3.down * 150, 0.5f, 1)
                    .SetEase(Ease.Flash)
                    .OnComplete(() =>
                    {
                        isPlugging = false;
                        OnComplete?.Invoke();
                    });
            });
    }

    public void OnHide()
    {
        isShown = false;
        canDrag = false;
        transform.localScale = Vector3.zero;
    }

    public void OnShow(Transform _endTrans, System.Action OnComplete)
    {
        canDrag = false;
        isShown = true;

        if (scaleItemTween != null)
        {
            scaleItemTween?.Kill();
            transform.localScale = Vector3.zero;
        }
        if (moveTween != null) moveTween?.Kill();
        transform.SetParent(_endTrans);
        transform.localPosition = Vector3.zero;

        scaleItemTween = transform.DOScale(startScale, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                canDrag = true;
                OnComplete?.Invoke();
            });
    }
    public void OnMoveDown(System.Action OnComplete)
    {
        canDrag = false;
        isShown = false;
        _endPos = new Vector2(transform.position.x, UISetupManager.Instance.outsideDown.position.y);
        moveTween = transform.DOMove(_endPos, 0.5f)
        .SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            OnComplete?.Invoke();
        });
    }
}
