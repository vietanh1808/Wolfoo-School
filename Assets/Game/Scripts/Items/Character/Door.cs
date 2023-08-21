using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using SCN;

public class Door : BackItem
{
    [SerializeField] List<Sprite> sprites;
    [SerializeField] float scaleLess = 0f;
    [SerializeField] Vector3 posLess;
    [SerializeField] Button playgameBtn;
    [SerializeField] PanelType panelType;
    [SerializeField] Panel parentPanel;

    GameObject curRoom;
    private Tween fadeTween;
    private Tweener colorTween;
    private Tweener colorTween2;
    private Tweener punchTween;
    private Type curType = Type.Close;
    bool isFirstTouch = true;

    public Type CurType { get => curType; }

    public enum Type
    {
        Close,
        Open
    }

    protected override void Start()
    {
        base.Start();
        skinType = SkinBackItemType.Door;

        if (playgameBtn != null)
        {
            playgameBtn.onClick.AddListener(OnPLaygame);
            playgameBtn.gameObject.SetActive(false);
            playgameBtn.transform.DOPunchPosition(Vector3.up * 15, 1, 3).SetLoops(-1, LoopType.Restart);
        }

        OnClick();
    }

    void FadeTween()
    {
        fadeTween = DOVirtual.DelayedCall(3, () =>
        {
            //colorTween = image.DOColor(Color.black, 1.5f).SetLoops(-1, LoopType.Yoyo);
            colorTween = image.DOColor(new Color(0.8f, 0.8f, 0.8f , 1), 0.5f)
            .SetLoops(2, LoopType.Restart)
            .OnComplete(() =>
            {
                colorTween2 = image.DOColor(Color.white, 0.5f).OnComplete(() =>
                {
                    FadeTween();
                });
            });
        });
    }

    private void OnPLaygame()
    {
        OnClick();
        parentPanel.Hide();
        EventManager.OpenPanel?.Invoke(panelType);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnClick();
    }
    void OnClick()
    {
        if(isFirstTouch)
        {
            isFirstTouch = false;
        }
        else
        {
            DisableDance();
        }

        //if(colorTween != null)
        //{
        //    image.color = Color.white;
        //    colorTween?.Kill();
        //}
        //if (colorTween2 != null)
        //{
        //    image.color = Color.white;
        //    colorTween2?.Kill();
        //}
        fadeTween?.Kill();

        //image.DOFade(1, 0);
        //image.color = Color.white;

        if (curType == Type.Open) curType = Type.Close;
        else curType = Type.Open;

        image.sprite = sprites[(int)curType];
        image.SetNativeSize();

        EventDispatcher.Instance.Dispatch(new EventKey.OnClickBackItem { door = this });
    //    EventManager.OnOpenDoor?.Invoke(curType == Type.Open);

        if (curType == Type.Open)
        {
            transform.localScale -= Vector3.one * scaleLess;
            transform.position += posLess;

            if (panelType != PanelType.None)
            {
                //fadeTween = DOVirtual.DelayedCall(1, () =>
                //{
                //    OnPLaygame();
                //});
            }
            if (playgameBtn != null)
                playgameBtn.gameObject.SetActive(true);
        }
        else
        {
            if (fadeTween != null) fadeTween?.Kill();

            transform.localScale = startScale;
            transform.position -= posLess;

            if (playgameBtn != null)
                playgameBtn.gameObject.SetActive(false);
        }
    }
}