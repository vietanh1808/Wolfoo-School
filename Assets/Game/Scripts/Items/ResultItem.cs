using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ResultItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Text resultText;

    private int id;
    private int result;
    private Vector3 startPos;
    private bool canDrag;
    private Tweener moveTween;
    private int idxEmpty = -1;
    private Transform startParent;

    public Text ResultText { get => resultText; }
    public int Result { get => result; }
    public int IdxEmpty { get => idxEmpty; }

    public void AssignItem(int _id, int _result, Color color)
    {
        id = _id;
        result = _result;
        resultText.text = result + "";
        resultText.color = color;
        canDrag = true;
        startPos = transform.position;
        idxEmpty = -1;
        startParent = transform.parent;
    }
    public void AssignPos()
    {
        startPos = transform.position;
    }
    public void SetStateDrag(bool state)
    {
        canDrag = state;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (moveTween != null) moveTween?.Kill();
        transform.SetParent(startParent);
        EventManager.OnResultMoveOut?.Invoke(idxEmpty);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        GameManager.instance.GetCurrentPosition(transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        EventManager.OnEndDragResult?.Invoke(id, result, this);
    }

    public void OnSuccess(int _idxEmpty, Vector3 endPos, Transform endParent)
    {
        idxEmpty = _idxEmpty;
        transform.SetParent(endParent);
        transform.SetSiblingIndex(3);
        transform.position = endPos;
    }

    public void OnCompleted()
    {
        canDrag = false;
    }

    public void OnFail(System.Action OnStart = null)
    {
        EventManager.OnResultMoveOut?.Invoke(idxEmpty);
        //  EventManager.OnResultMoveOut?.Invoke(idxEmpty);
        canDrag = false;
        moveTween =  transform.DOMove(startPos, 1).OnComplete(() =>
        {
            idxEmpty = -1;
            canDrag = true;
        });
    }
}
