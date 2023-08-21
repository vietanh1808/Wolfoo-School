using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NoAdsPanel : Panel
{
    [SerializeField] Button closeBtn;

    Vector2 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.DOScale(startScale, 0.5f).SetEase(Ease.OutBack);
    }
    private void OnDisable()
    {
        transform.DOScale(Vector3.zero, 0.5f);
    }

    private void Start()
    {
        closeBtn.onClick.AddListener(() => { gameObject.SetActive(false); });
    }
}
