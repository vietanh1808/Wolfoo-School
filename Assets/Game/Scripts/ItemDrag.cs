using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected Image itemImg;
    [SerializeField] protected Transform compareTrans;

    protected int id;

    protected bool isCompareList;
    protected Transform endParent;
    private float distanceVerified = 2;
    private float distance;
    protected int idxVerified;
    private float curVerifyDistance;
    protected List<bool> listEmpty = new List<bool>();
    protected List<Transform> listEndTrans;

    protected bool canDrag;
    private bool isOutBack;
    protected bool isDragging;
    protected bool isHasLimit;

    protected Quaternion startRotate;
    protected Vector3 startPos;
    protected Vector3 startScale;
    private Vector3 endPos;
    private Transform leftLimit;
    private Transform rightLimit;
    private Transform upLimit;
    private Transform downLimit;
    protected Transform endTrans;
    protected Transform startParent;

    private Tween delayTween;
    protected Tweener moveTween;
    protected Tweener scaleTween;
    protected Tween jumpTween;
    private Tweener moveTutTween;
    protected Tweener punchScale;
    private bool isScaleTut;

    public int Id { get => id; }
    public int IdxVerified { get => idxVerified; }
    public bool IsOutBack { get => isOutBack; }
    public Transform CompareTrans { get => compareTrans;  }

    private void Awake()
    {
        InitStartInfo();
    }
    private void OnDestroy()
    {
        if (moveTween != null) moveTween?.Kill();
        if (delayTween != null) delayTween?.Kill();
        if (scaleTween != null) scaleTween?.Kill();
        if (jumpTween != null) jumpTween?.Kill();
        if (moveTutTween != null) moveTutTween?.Kill();
        if (punchScale != null) punchScale?.Kill();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        if (!canDrag) return;

        StopScaleTut();
        scaleTween = transform.DOScale(startScale + Vector3.one * 0.1f, 0.5f);
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        GameManager.instance.GetCurrentPosition(transform);
        if (isHasLimit)
        {
            if (rightLimit != null && transform.position.x > rightLimit.position.x)
            {
                transform.position = new Vector3(rightLimit.position.x, transform.position.y, 0);
                OnTouchLimit(TouchDirectionType.Right);
            }
            if (leftLimit != null && transform.position.x < leftLimit.position.x)
            {
                transform.position = new Vector3(leftLimit.position.x, transform.position.y, 0);
                OnTouchLimit(TouchDirectionType.Left);
            }
            if (upLimit != null && transform.position.y > upLimit.position.y)
            {
                transform.position = new Vector3(transform.position.x, upLimit.position.y, 0);
                OnTouchLimit(TouchDirectionType.Up);
            }
            if (downLimit != null && transform.position.y < downLimit.position.y)
            {
                transform.position = new Vector3(transform.position.x, downLimit.position.y, 0);
                OnTouchLimit(TouchDirectionType.Down);
            }
        }
        else
        {
            GameManager.instance.CheckPos(transform);
        }
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        if (!canDrag) return;

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

        if (scaleTween != null) scaleTween?.Kill();
        scaleTween = transform.DOScale(startScale, 0.5f)
        .OnComplete(() =>
        {
            if (isScaleTut)
                ScaleTut(true);
        });
    }

    /// <summary>
    /// 0: Right
    /// 1: Left
    /// 2: Up
    /// 3: Down
    /// </summary>
    /// <param name="v"></param>
    protected virtual void OnTouchLimit(TouchDirectionType v)
    {
    }

    #region ##### PUBLIC METHOD #####
    public void OnCompare(System.Action OnComplted = null, System.Action OnFail = null)
    {
        // On Compare With List End Pos
        if (isCompareList)
        {
            idxVerified = -1;
            curVerifyDistance = 1000f;
            for (int i = 0; i < listEndTrans.Count; i++)
            {
                if (!listEmpty[i]) continue;

                distance = Vector2.Distance(compareTrans.position, listEndTrans[i].position);
                if (distance <= distanceVerified)
                {
                    if (curVerifyDistance > distance)
                    {
                        idxVerified = i;
                        curVerifyDistance = distance;
                    }
                }
            }

            if (idxVerified != -1)
            {
                listEmpty[idxVerified] = false;
                endTrans = listEndTrans[idxVerified];
                if (endParent == null) endParent = endTrans;
                SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
                OnComplted?.Invoke();
            }
            else
            {
                OnFail?.Invoke();
                SoundManager.instance.PlayOtherSfx(SfxOtherType.Incorrect);
            }
        }
        // On Compare With End Pos
        else
        {
            distance = Vector2.Distance(compareTrans.position, endTrans.position);
            if (distance <= distanceVerified)
            {
                SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
                OnComplted?.Invoke();
            }
            else
            {
                SoundManager.instance.PlayOtherSfx(SfxOtherType.Incorrect);
                OnFail?.Invoke();
            }
        }
    }
    public void ScaleTut(bool isLoop, float delay = 2, float power = 0.1f, System.Action OnComplete = null)
    {
        isScaleTut = true;
        if (scaleTween != null) scaleTween?.Kill();
        if (punchScale != null) punchScale?.Kill();
        transform.localScale = startScale;

        punchScale = transform.DOPunchScale(Vector3.one * power, 0.5f, 6)
        .SetDelay(delay)
        .OnComplete(() =>
        {
            if(isLoop)
            {
                ScaleTut(isLoop, delay, power, OnComplete);
                return;
            }
            OnComplete?.Invoke();
        });
    }
    public void StopScaleTut()
    {
        isScaleTut = false;
        if(scaleTween != null) scaleTween.Kill();
        if (punchScale != null) punchScale?.Kill();
        transform.localScale = startScale;
    }
    #endregion

    #region ##### Assign #####
    public void InitStartInfo()
    {
        startParent = transform.parent;
        startPos = new Vector3(transform.position.x, transform.position.y, 0);
        startScale = transform.localScale;
        startRotate = transform.rotation;
    }

    public void AssignLimitZone(Transform _left, Transform _right, Transform _up, Transform _down)
    {
        isHasLimit = true;
        leftLimit = _left;
        rightLimit = _right;
        upLimit = _up;
        downLimit = _down;
    }

    public void AssignItem(
        Transform _endTrans,
        Transform _endParent,
        float _distanceVerified = 2,
        Transform _compareTrans = null)
    {
        isCompareList = false;
        endTrans = _endTrans;
        endParent = _endParent;
        distanceVerified = _distanceVerified;
        compareTrans = _compareTrans;
        canDrag = true;
    }
  
    public void AssignItem(
        List<Transform> _endTrans,
        Transform _endParent,
        float _distanceVerified = 2,
        Transform _compareTrans = null,
        List<bool> _emptyIdx = null)
    {
        isCompareList = true;
        listEndTrans = _endTrans;
        endParent = _endParent;
        distanceVerified = _distanceVerified;
        compareTrans = _compareTrans;
        canDrag = true;

        if (_emptyIdx == null)
        {
            if (listEndTrans.Count > 0)
                listEmpty.Clear();
            for (int i = 0; i < _endTrans.Count; i++)
            {
                listEmpty.Add(true);
            }
        }
        else
        {
            listEmpty = _emptyIdx;
        }
    }
    #endregion

    #region ##### MOVE METHOD ######
    public void JumpToEndPos(Vector3 _endPos, System.Action OnComplete = null, float power = 3)
    {
        canDrag = false;
        if (jumpTween != null) jumpTween?.Kill();
        jumpTween = transform.DOJump(_endPos, 3, 1, 0.5f)
            .SetEase(Ease.Flash)
            .OnComplete(() =>
            {
                if (punchScale != null)
                {
                    punchScale?.Kill();
                    transform.localScale = startScale;
                }
                punchScale = transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 3)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    OnComplete?.Invoke();
                });
            });
    }
    public void MoveToEndPos(Vector3 _endPos, System.Action OnComplete = null)
    {
        canDrag = false;
        jumpTween = transform.DOMove(_endPos, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
        {
            if (punchScale != null)
            {
                punchScale?.Kill();
                transform.localScale = startScale;
            }
            punchScale = transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 3)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
                OnComplete?.Invoke();
            });
        });
    }
    public void JumpToStartPos(System.Action OnComplete = null)
    {
        canDrag = false;
        scaleTween = transform.DOScale(startScale, 0.5f);
        jumpTween = transform.DOJump(startPos, 2f, 1, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
        {
            transform.SetParent(startParent);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            OnComplete?.Invoke();
        });
    }
    public void MoveInBack(float time = 0.5f, System.Action OnComplete = null)
    {
        canDrag = false;
        isOutBack = false;

        if (time == 0)
        {
            transform.position = startPos;
            return;
        }

        moveTween = transform.DOMove(startPos, time)
        .SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            OnComplete?.Invoke();
        });
    }
    public void MoveOut(Direction direction, float time = 0.5f, System.Action OnComplete = null)
    {
        canDrag = false;
        isOutBack = true;
        var _endPos = Vector2.zero;
        switch (direction)
        {
            case Direction.Left:
                _endPos = new Vector2(UISetupManager.Instance.outsideLeft.position.x, transform.position.y);
                break;
            case Direction.Right:
                _endPos = new Vector2(UISetupManager.Instance.outsideLeft.position.x * -1, transform.position.y);
                break;
            case Direction.Up:
                _endPos = new Vector2(transform.position.x, UISetupManager.Instance.outsideDown.position.y * -1);
                break;
            case Direction.Down:
                _endPos = new Vector2(transform.position.x, UISetupManager.Instance.outsideDown.position.y);
                break;
        }

        if(time == 0)
        {
            transform.position = _endPos;
            OnComplete?.Invoke();
            return;
        }

        if (moveTween != null) moveTween?.Kill();
        moveTween = transform.DOMove(_endPos, time)
        .SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            OnComplete?.Invoke();
        });
    }
    public void MoveToEndPos2(System.Action OnComplete)
    {
        canDrag = false;
        if (scaleTween != null)
        {
            scaleTween?.Kill();
            transform.localScale = startScale;
        }
        scaleTween = transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1);
        moveTween = transform.DOMove(endTrans.position, 0.5f)
        .OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }
    #endregion
}

public enum TouchDirectionType
{
    Left,
    Right,
    Up,
    Down
}