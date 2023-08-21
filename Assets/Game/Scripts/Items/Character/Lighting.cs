using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Lighting : BackItem
{
    [SerializeField] List<Sprite> sprites;
    int isTurnOn = 0;
    private Tweener rotaeTween;
    private Vector3 startPos;

    enum State
    {
        On,
        Off
    }
    protected override void Start()
    {
        base.Start();
        startPos = transform.localPosition;
        GetClick();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        isTurnOn = 1 - isTurnOn;
        GetClick();
    }

    void GetClick()
    {
        if(rotaeTween != null)
        {
            rotaeTween?.Kill();
            image.transform.localPosition = startPos;
        }

        image.sprite = sprites[isTurnOn];
        image.SetNativeSize();

        rotaeTween = image.transform.DOPunchPosition(Vector3.down * 40, 1, 2);
    }
}
