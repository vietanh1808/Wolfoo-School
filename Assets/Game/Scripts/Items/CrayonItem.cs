using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CrayonItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Button pen;
    [SerializeField] Color color;

    int id;
    float yPos, xPos;
    Tween moveTween;

    public Color Color { get => color; set => color = value; }

    public void AssignItem(int _id)
    {
        id = _id;
        xPos = transform.localPosition.x;
        yPos = transform.localPosition.y;
    }

    public void ChooseColor()
    {
        moveTween?.Kill();
        moveTween = transform.DOLocalMove(new Vector2(xPos + 150, yPos), 0.5f);
    }
    public void HideColor()
    {
        moveTween?.Kill();
        moveTween = transform.DOLocalMove(new Vector2(xPos, yPos), 0.5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      //  EventManager.ChangeCrayon(this);
    }
}
