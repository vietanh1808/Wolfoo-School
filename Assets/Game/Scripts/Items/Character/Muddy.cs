using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Muddy : BackItem
{
    [SerializeField] ParticleSystem lightingFx;
    private float distance;
    private Tween delayTween;
    private bool isComplete;

    protected override void Start()
    {
        base.Start();
    }
    private void OnEnable()
    {
        EventManager.OnDragMop += GetDragMop;
        EventManager.OnEndDragMop += GetEndDragMop;
    }
    private void OnDisable()
    {
        EventManager.OnDragMop -= GetDragMop;
        EventManager.OnEndDragMop -= GetEndDragMop;
    }

    private void GetEndDragMop()
    {
        if (delayTween != null)
            delayTween?.Kill();
    }

    private void GetDragMop(Vector3 pos)
    {
        distance = Vector2.Distance(pos, transform.position);
        if (distance <= 5)
        {
            if (delayTween != null && delayTween.IsActive()) return;
            delayTween = DOVirtual.DelayedCall(1f, () =>
            {
                OnComplete();
            });
        }
        else
        {
            if(delayTween != null)
                delayTween?.Kill();
        }
    }
    void OnComplete()
    {
        if (isComplete) return;

        isComplete = true;
        image.DOFade(0, 0.5f).OnComplete(() =>
        {
            if (lightingFx == null) return;
            lightingFx.Play();
        });
    }
}
