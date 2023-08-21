using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class AlphaTable : BackItem
{
    [SerializeField] Button playgameBtn;
    [SerializeField] PanelType nextMode;
    [SerializeField] Panel parentPanel;

    private Tweener scaleBtnTween;

    protected override void Start()
    {
        playgameBtn.onClick.AddListener(OnPlaygame);
        playgameBtn.transform.DOPunchScale(Vector3.one * 0.2f, 1f, 2).SetLoops(-1, LoopType.Restart);
    }
 
    private void OnDestroy()
    {
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
      //  OnPunchScale();

        if (scaleBtnTween != null) scaleBtnTween?.Kill();
        scaleBtnTween = playgameBtn.transform.DOScale(playgameBtn.transform.localScale + Vector3.one * 0.3f, 2);
    }

    private void OnPlaygame()
    {
        EventManager.OnPlaygame?.Invoke(parentPanel, nextMode);
    //    EventManager.OpenPanel(PanelType.AlphaLearningMode);
    }
}
