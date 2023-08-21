using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using SCN;

public class BackItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    private bool isDragging;
    [SerializeField] protected bool isDrag;
    [SerializeField] protected bool isClick;
    [SerializeField] protected bool isComparePos;
    [SerializeField] protected bool isHandItem;
    [SerializeField] protected bool isFood;
    [SerializeField] protected bool isShapeGlocery;
    [SerializeField] protected bool isWolfoo;
    [SerializeField] protected bool isBook;
    [SerializeField] protected bool isInBag;
    [SerializeField] protected bool isTrash;
    [SerializeField] protected bool isPencil;
    [SerializeField] protected bool isCarryItem;
    [SerializeField] protected bool isDance;
    [SerializeField] Ease ease = Ease.OutBounce;
    [SerializeField] Image imageItem;
    [SerializeField] protected SkinBackItemType skinType;
    [SerializeField] float power;

    /// <summary>
    /// Not use this id for Save to Local Storage
    /// </summary>
    [HideInInspector] public int id;
    protected Image image;
    protected bool canClick = true;
    protected bool canDrag = true;
    private bool isMovingToDesk;
    protected bool canMoveToGround = true;
    protected bool isStandTable;
    protected GameObject content;
    protected GameObject ground;

    protected Vector3 startScale;
    private Vector3 startParentScale;
    protected Vector3 awakePos;
    private Transform startParent;
    protected Tweener tweenScale;
    protected Tween tweenJump;
    protected Tween tweenMove;
    protected Tween tweenPunch;
    protected Tween delayTween;
    private bool firstTouch = true;
    private int rdTimeLoop;
    private Vector3 startContentScale;
    private float _distance;

    public bool IsFood { get => isFood; }
    public bool IsShapeGlocery { get => isShapeGlocery; }
    public bool IsWolfoo { get => isWolfoo; }
    public bool IsBook { get => isBook; }
    public bool IsPencil { get => isPencil; }
    public bool IsInBag { get => isInBag; }
    public bool IsTrash { get => isTrash; }
    public bool IsCarryItem { get => isCarryItem; }
    public bool IsStandTable { get => isStandTable; }
    public bool IsComparePos { get => isComparePos; }

    private void Awake()
    {
        startScale = transform.localScale;
        startParentScale = transform.parent.localScale;
        awakePos = transform.localPosition;
        startParent = transform.parent;
    }
    protected virtual void Start()
    {
        id = GameManager.instance.countId++;
        image = imageItem == null ? GetComponent<Image>() : imageItem;
        ease = Ease.OutBounce;
        power = 0.1f;

        if(isDance)
        {
            rdTimeLoop = UnityEngine.Random.Range(2, 4);
            Dancing();
        }
    }
    private void OnDestroy()
    {
    }
    void Dancing()
    {
        tweenPunch = transform.DOPunchScale(new Vector3(0.05f, -0.05f, 0), 1.5f, 2).OnComplete(() =>
        {
            delayTween = DOVirtual.DelayedCall(rdTimeLoop, () =>
            {
                Dancing();
            });
        });
    }
    public void DisableDance()
    {
        if (tweenPunch != null)
        {
            tweenPunch?.Kill();
            transform.localScale = startScale;
        }
        if(delayTween != null)
        {
            delayTween?.Kill();
        }
    }
    public Image GetImage()
    {
        return image;
    }

    public void GetMainPanel(GameObject content_, GameObject ground_)
    {
        content = content_;
        ground = ground_;
    }

    void OnEnable()
    {
        canClick = true;
        EventManager.OnEndDragBackItem += GetEndDragBackItem;
        EventManager.GetMainPanel += GetMainPanel;
        EventDispatcher.Instance.RegisterListener<EventKey.OnBeginDragBackItem>(GetBeginEndDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnDragBackItem>(GetDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnEndDragBackItem>(GetEndDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnClickBackItem>(GetClickBackItem);
    }
     void OnDisable()
    {
        tweenMove?.Kill();
        tweenPunch?.Kill();
        tweenJump?.Kill();

        EventManager.GetMainPanel -= GetMainPanel;
        EventManager.OnEndDragBackItem -= GetEndDragBackItem;
        EventDispatcher.Instance.RemoveListener<EventKey.OnBeginDragBackItem>(GetBeginEndDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnDragBackItem>(GetDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnEndDragBackItem>(GetEndDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnClickBackItem>(GetClickBackItem);
    }

    protected virtual void GetClickBackItem(EventKey.OnClickBackItem item)
    {
        
    }

    protected virtual void GetBeginEndDragItem(EventKey.OnBeginDragBackItem item)
    {
    }

    protected virtual void GetDragItem(EventKey.OnDragBackItem item)
    {
    }

    protected virtual void GetEndDragItem(EventKey.OnEndDragBackItem item)
    {
   //     if (item.backItem == null) return;
        if (item.backItem == this) return;
        if (!isComparePos) return;
        GetSiblingIndex(item.backItem);
    }

    public virtual void GetEndDragBackItem(BackItem item, int id_)
    {
     //   if (!item.isDragging) return;
        if (id == id_) return;
        if (!isComparePos) return;

        GetSiblingIndex(item);
     //   MoveToGround();
    }
    public virtual void GetSiblingIndex(BackItem backItem_)
    {
        if(backItem_ == null) return;

        _distance = Vector2.Distance(backItem_.transform.position, transform.position);
        if (_distance > 2) return;
        if (backItem_.transform.position.y > transform.position.y)
        {
            if(canMoveToGround)
            {
                backItem_.DisableDrag();
                backItem_.transform.SetParent(transform.parent);
                backItem_.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
                backItem_.MoveToGround();
            }
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDrag || !canDrag) return;

        canMoveToGround = true;
        isMovingToDesk = false;
        DisableDrag();

        transform.SetParent(content.transform);
        transform.SetAsLastSibling();
        GameManager.instance.GetCurrentPosition(transform);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isDrag || !canDrag) return;
        DisableDrag();

        isDragging = true;
        isDrag = true;
        GameManager.instance.GetCurrentPosition(transform);
        EventManager.OnDragBackItem?.Invoke(transform);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!isDrag || !canDrag) return;
        isDragging = false;

        GameManager.instance.CheckPos(transform);
        transform.SetParent(content.transform);
        MoveToGround();
        EventManager.OnEndDragBackItem?.Invoke(this, id);
    }
    public void MoveToGround()
    {
        if(!canMoveToGround) return;

        // new
      //  transform.SetParent(content.transform);

        if (transform.position.y > ground.transform.position.y)
        {
            if (tweenMove != null) tweenMove?.Kill();
            tweenMove = transform.DOMoveY(ground.transform.position.y, 1)
           // .SetSpeedBased(true)
            .SetEase(ease, 0.5f, 0.5f)
            .OnComplete(() =>
            {
                SoundManager.instance.PlayOtherSfx(SfxOtherType.DownToGround);
            });
        }
        else
        {
            if (tweenJump != null) tweenJump?.Kill();
            tweenJump = transform.DOLocalJump(transform.localPosition, 10, 1, 0.5f).SetEase(Ease.OutBounce);
        }
    }
    public void MoveToStartPos()
    {
        canMoveToGround = false;
        DisableDrag();
        transform.SetParent(startParent);
        transform.localScale = startScale;
        tweenMove = transform.DOLocalMove(awakePos, 0.5f).OnComplete(() =>
        {
            tweenMove = transform.DOPunchPosition(Vector3.up * 3, 0.5f, 1);
        });
    }
    public void JumpOutSide(Vector3 endPos)
    {
        DisableDrag();
        gameObject.SetActive(true);

        transform.SetParent(content.transform);
        tweenScale = transform.DOScale( (startScale.x * startParentScale), 0.5f);
        tweenJump = transform.DOJump(endPos, 1, 1, 0.5f);
    }
    public void MoveToDesk(Vector3 _endPos, Transform _endParent)
    {
        if (isMovingToDesk) return;
        isMovingToDesk = true;
        canMoveToGround = false;
        DisableDrag();
        transform.SetParent(_endParent);
        tweenMove = transform.DOJump(_endPos, 0.5f, 1, 0.5f)
        .SetEase(ease)
        .OnComplete(() =>
        {
            
            SoundManager.instance.PlayOtherSfx(SfxOtherType.DownToGround);
        });
    }

    public void DisableDrag()
    {
        if(tweenJump != null)
            tweenJump?.Kill();
        if(tweenMove != null)
            tweenMove?.Kill();
    }
    public void DisableScale()
    {
        if(tweenScale != null)
        {
            tweenScale?.Kill();
            transform.localScale = startScale;
        }

    }
    public void SetNotMoveToGround()
    {
        canMoveToGround = false;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        EventManager.OnClickBackItem?.Invoke(id, this);

        if (!firstTouch) return;
        if (isDance)
        {
            DisableScale();
            firstTouch = false;
            rdTimeLoop *= 2;
        }
    }
    public void OnPunchScale()
    {
        if (!isClick) return;
        if (tweenPunch != null)
        {
            tweenPunch?.Kill();
            transform.localScale = startScale;
        }

        if (delayTween != null)
        {
            delayTween?.Kill();
        }

        tweenPunch = transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 2);
    }
}
