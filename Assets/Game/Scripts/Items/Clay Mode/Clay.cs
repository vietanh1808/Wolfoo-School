using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Clay : ItemMove
{
    [SerializeField] Image[] imageChanges;
    [SerializeField] Transform changedZone;
    [SerializeField] Transform stampedZone;

    private int curIdx;
    private bool canChange = true;
    private Tweener scaleItemTween;
    private Tweener scaleItemTween2;
    private Tween delaytemTween;
    private StampedClay stampedClay;

    public Transform StampedZone { get => stampedZone; }

    private void Start()
    {
        foreach (var item in imageChanges)
        {
            item.gameObject.SetActive(false);
        }
        imageChanges[0].gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        if (scaleItemTween != null) scaleItemTween?.Kill();
        if (scaleItemTween2 != null) scaleItemTween2?.Kill();
        if (delaytemTween != null) delaytemTween?.Kill();
    }
    /// <summary>
    /// 1: Peeled, 2: Stamped
    /// </summary>
    public void ChangeState(int stateIdx)
    {
        switch (stateIdx)
        {
            case 1:
                imageChanges[0].gameObject.SetActive(false);
                imageChanges[imageChanges.Length - 1].gameObject.SetActive(true);
                break;
            case 2:
                changedZone.gameObject.SetActive(false);
                break;
        }
    }

    public void OnChange(float time, System.Action OnCompleteAll, System.Action OnCompleteChange)
    {
        if (!canChange || curIdx > imageChanges.Length) return;
        canChange = false;

        curIdx++;
        if (curIdx == imageChanges.Length)
        {
            OnCompleteAll?.Invoke();
            return;
        }
        else
        {
            var scaleValue = Vector3.zero;
            switch (curIdx - 1)
            {
                case 0:
                    scaleValue = Vector3.one * 0.1f;
                    break;
                case 1:
                case 2:
                    scaleValue = Vector3.one * 0.2f;
                    break;
            }
            scaleItemTween = imageChanges[curIdx - 1].transform
                .DOScale(imageChanges[curIdx - 1].transform.localScale + scaleValue, time)
                .SetEase(Ease.Linear);
            scaleItemTween2 = imageChanges[curIdx].transform
                .DOScale(Vector3.one, time)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    canChange = true;
                    OnCompleteChange?.Invoke();
                });
            delaytemTween = DOVirtual.DelayedCall(time / 2, () =>
            {
                imageChanges[curIdx - 1].gameObject.SetActive(false);
                imageChanges[curIdx].gameObject.SetActive(true);
            });
        }
    }
}
