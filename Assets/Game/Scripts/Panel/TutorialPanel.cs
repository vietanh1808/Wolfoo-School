using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] Image hand;
    [SerializeField] Button panel;

    public static TutorialPanel instance;
    
    private bool isClickPanel;
    private int count = 0;
    private int limitCount;
    bool isPlay;
    private Tween delayTween;
    private Tweener tween;
    private float speed = 5;
    private float noReactTime = 0;

    public float Speed { get => speed; set => speed = value; }
    public float NoReactTime { get => noReactTime; set => noReactTime = value; }

    private void Awake()
    {
        if (instance == null) instance = this;
       // panel.onClick.AddListener(OnPanelClick);
        hand.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        isClickPanel = false;
        count = limitCount = 0;
        speed = 5;
        noReactTime = 0;
    }

    private void OnMouseDown()
    {
        OnPanelClick();
    }
    public void DisableTut()
    {
        OnPanelClick();
    }
    public void StartPointer(Transform parentTrans)
    {
        if (delayTween != null) delayTween?.Kill();

        gameObject.SetActive(true);
        limitCount = parentTrans.childCount;
        isPlay = true;

        hand.transform.position = parentTrans.GetChild(count).position;
        hand.gameObject.SetActive(true);
        delayTween = DOVirtual.DelayedCall(noReactTime, () =>
        {
            HandMove(parentTrans);
        });
    }
    public void StartPointer(Vector3 startPos, Vector3 endPos)
    {
        gameObject.SetActive(true);
        isPlay = true;
        if (delayTween != null) delayTween?.Kill();

        delayTween =  DOVirtual.DelayedCall(noReactTime, () =>
        {
            HandMove(startPos, endPos);
        });
    }
    public void StartPointer(Vector3 pos)
    {
        gameObject.SetActive(true);
        isPlay = true;

        delayTween = DOVirtual.DelayedCall(noReactTime, () =>
        {
            HandMove(pos);
        });
    }

    void HandMove(Transform parentTrans)
    {
        if (!isPlay) return;
        if (tween != null) tween?.Kill();

        count++;
        if (count >= limitCount) count = 0;

        tween = hand.transform.DOMove(parentTrans.GetChild(count).position, speed)
        .SetSpeedBased(true)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            if (!isPlay) return;
            HandMove(parentTrans);
        });
    }
    void HandMove(Vector3 startPos, Vector3 endPos)
    {
        if (!isPlay) return;
        count++;
        if (count >= limitCount) count = 0;

        hand.transform.position = startPos;
        hand.gameObject.SetActive(true);

        tween = hand.transform.DOMove(endPos, speed)
        .SetSpeedBased(true)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            if (!isPlay) return;
            HandMove(startPos, endPos);
        });
    }
    void HandMove(Vector3 pos)
    {
        if (!isPlay) return;
        count++;
        if (count >= limitCount) count = 0;

        hand.transform.position = pos;
        hand.gameObject.SetActive(true);

        tween = hand.transform.DOMove(pos - Vector3.one * 0.1f, speed)
        .SetSpeedBased(true)
        .SetEase(Ease.Linear).SetLoops(-1);
    }

    void OnPanelClick()
    {
        gameObject.SetActive(false);
        if (isClickPanel) return;
        isClickPanel = true;
        isPlay = false;
        tween?.Kill();

        Debug.Log("Click Tut Panel");
        hand.gameObject.SetActive(false);
        EventManager.OnClickTutorial?.Invoke();
    }
}
