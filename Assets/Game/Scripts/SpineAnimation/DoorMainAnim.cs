using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DoorMainAnim : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] DoorAnimation doorAnim;
    [SerializeField] Button playGameBtn;
    [SerializeField] PanelType panelType;
    [SerializeField] Panel parentPanel;
    [SerializeField] bool isCommingSoon;

    bool isClosed = true;
    GameObject curMode;
    private Tween delayDoorTween;
    private Tweener colorTween;
    private Tweener colorTween2;
    private Tweener punchTween;
    private Vector3 startScale;

    private void Start()
    {
        playGameBtn.onClick.AddListener(PLayGame);
        playGameBtn.gameObject.SetActive(false);

        OnCliCkDoor();

        if (playGameBtn != null)
        {
            playGameBtn.transform.DOPunchPosition(Vector3.up * 15, 1, 3).SetLoops(-1, LoopType.Restart);
        }
    }

    private void PLayGame()
    {
        OnCliCkDoor();
        if(isCommingSoon)
        {
            EventManager.OpenPanel?.Invoke(PanelType.CommingSoon);
        }
        else
        {
            EventManager.OnPlaygame?.Invoke(parentPanel, panelType);
        }
        startScale = transform.localScale;
        Dancing();
    }


    void Dancing()
    {
        delayDoorTween = DOVirtual.DelayedCall(3, () =>
        {
            punchTween = transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 1).OnComplete(() =>
            {
                Dancing();
            });
        });
    }

    private void OnCliCkDoor()
    {
        isClosed = !isClosed;
        if(isClosed)
        {
            doorAnim.CloseAnim();
            if (delayDoorTween != null) delayDoorTween?.Kill();
            playGameBtn.gameObject.SetActive(false);
        }
        else 
        {
            doorAnim.OpenAnim(() =>
            {
                playGameBtn.gameObject.SetActive(true);
            });

        }

        if (punchTween != null)
        {
            punchTween?.Kill();
            transform.localScale = startScale;
        }
        if (delayDoorTween != null)
        {
            delayDoorTween?.Kill();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCliCkDoor();
    }

    public enum AnimState
    {
        None,
        Idle,
        Play,
        Open,
        Close,
    }
}
