using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PaperItem : MonoBehaviour
{
    [SerializeField] Button button;
    private int id;
    private Tweener loopTween;
    private Vector3 startScale;
    private Tweener rotateTween;
    bool isCompleted;

    public void AssignItem(int _id, Sprite sprite)
    {
        id = _id;
        button.image.sprite = sprite;
        button.image.SetNativeSize();
        button.onClick.AddListener(OnClickItem);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        startScale = transform.localScale;

        EventManager.OnClickPaper += OnClickOther;
        EventManager.OnCompleteCutBox += GetCompleteCutBox;
    }
    private void OnDisable()
    {
        EventManager.OnClickPaper -= OnClickOther;
        EventManager.OnCompleteCutBox -= GetCompleteCutBox;
    }

    private void GetCompleteCutBox()
    {
        isCompleted = true;
        loopTween?.Kill();
        rotateTween?.Kill();
    }

    void OnClickOther(int idx, PaperItem item)
    {
        if (id != idx)
        {
            loopTween?.Kill();
            rotateTween?.Kill();
            transform.localScale = startScale;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            return;
        }
    }

     public void OnAnim()
    {
        rotateTween = transform.DORotate(Vector3.forward * 60, 0.5f).OnComplete(() =>
        {
            loopTween = transform.DOPunchScale(Vector3.one * 0.05f, 0.5f, 1).SetLoops(-1, LoopType.Restart);
        });
    }

    private void OnClickItem()
    {
        if (isCompleted) return;
        EventManager.OnClickPaper?.Invoke(id, this);
    }
}
