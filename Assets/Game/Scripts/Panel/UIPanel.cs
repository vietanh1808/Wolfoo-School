using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Ease ease = Ease.OutBack;
    private Vector3 startScale;
    bool isStarted = true;
    private Tweener scaleBackTween;

    private void Awake()
    {
        startScale = background.transform.localScale;
        background.transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        //if(isStarted)
        //{
        //    isStarted = false;
        //    return;
        //}
        scaleBackTween = background.transform.DOScale(startScale, 0.5f)
        .SetEase(ease)
        .OnComplete(() =>
        {
         //   EventManager.OnShowPanel?.Invoke();
        });
    }
    private void Start()
    {
     //   gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        background.transform.localScale = Vector3.zero;

        if (scaleBackTween != null) scaleBackTween?.Kill();
    }
}
