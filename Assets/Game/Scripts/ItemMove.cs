using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SCN;

public abstract class ItemMove : MonoBehaviour
{
    [SerializeField] protected Image itemImg;

    protected int id;

    private bool isMoveInback;
    protected Transform startParent;
    protected Transform endParent;
    protected Vector3 startPos;
    protected Vector3 endPos;
    protected Vector3 startScale;
    protected Quaternion startRotate;

    protected Tweener scaleTween;
    protected Tween jumpTween;
    protected Tweener punchScale;
    protected Tweener moveTween;

    public Image ItemImg { get => itemImg; }
    public bool IsMoveInback { get => isMoveInback; }

    private void Awake()
    {
        InitStartInfo();
    }
    private void OnDestroy()
    {
        if (moveTween != null) moveTween?.Kill();
        if (punchScale != null) punchScale?.Kill();
        if (jumpTween != null) jumpTween?.Kill();
        if (scaleTween != null) scaleTween?.Kill();
    }

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    public void AssignItem(int _id)
    {
        id = _id;
    }

    public void InitStartInfo()
    {
        //    if(image == null) image = GetComponent<Image>();
        startParent = transform.parent;
        startPos = new Vector3(transform.position.x, transform.position.y, 0);
        startScale = transform.localScale;
        startRotate = transform.rotation;
    }
    
    public void AssignEndPos(Vector3 _endPos, Transform _endParent)
    {
        endPos = _endPos;
        endParent = _endParent;
    }
    public void MoveToEndPos(System.Action OnComplete = null, bool isDance = true)
    {
     //   jumpTween = transform.DOJump(endPos, 3, 1, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
        moveTween = transform.DOMove(endPos, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            if(!isDance)
            {
                OnComplete?.Invoke();
                return;
            }
            punchScale = transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 3)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        });
    }
    /// <summary>
    /// Default time = 0.5f
    /// </summary>
    public void JumpToStartPos(System.Action OnComplete = null)
    {
        scaleTween = transform.DOScale(startScale, 0.5f);
        jumpTween = transform.DOJump(startPos, 2f, 1, 0.5f).SetEase(Ease.Flash).OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            transform.SetParent(startParent);
            OnComplete?.Invoke();
            //  EventDispatcher.Instance.Dispatch(new EventKey.OnEndMoveItem { endParent = endParent });
        });
    }
    public void MoveToStartPos(float time = 0.5f, Ease ease = Ease.OutBounce, System.Action OnComplete = null)
    {
        if (moveTween != null) moveTween?.Kill();
        moveTween = transform.DOMove(startPos, time).SetEase(ease).OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            OnComplete?.Invoke();
        });
    }
    public void JumpToEndPos( System.Action OnComplete = null)
    {
        if(jumpTween != null) jumpTween?.Kill(); 
        jumpTween = transform.DOJump(endPos, 3, 1, 0.5f)
        .SetEase(Ease.Flash)
        .OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 3)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
        });
    }
    public void Dance(float power = 0.05f, System.Action OnComplete = null)
    {
        if (punchScale != null)
        {
            punchScale?.Kill();
            transform.localScale = startScale;
        }
        punchScale = transform.DOPunchScale(new Vector3(-power, power, 0), 0.5f, 1)
        .OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }
    public void ScaleTut(float power = 0.1f, System.Action OnComplete = null)
    {
        scaleTween = transform.DOPunchScale(Vector3.one * power, 0.5f, 1).OnComplete(() =>
        {
            // spoonRice.AssignScale(Vector3.one);
            OnComplete?.Invoke();
        });
    }
  
    public void MoveInBack(float time = 0.5f, System.Action OnComplete = null)
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.BongBong);
        isMoveInback = true;
        if (time == 0)
        {
            transform.position = startPos;
            return;
        }

        moveTween = transform.DOMove(startPos, time).SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            OnComplete?.Invoke();
        });
    }
    public void MoveInBack(Transform endTrans, System.Action OnComplete = null)
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.BongBong);
        isMoveInback = true;
        moveTween = transform.DOMove(endTrans.position, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            OnComplete?.Invoke();
        });
    }
    public void MoveOut(Direction direction, float time = 0.5f, System.Action OnComplete = null)
    {
        isMoveInback = false;
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

        if (moveTween != null) moveTween?.Kill();
        moveTween = transform.DOMove(_endPos, time)
        .SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
            OnComplete?.Invoke();
        });
    }

}

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
}
