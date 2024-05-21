using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SCN.Ads;

public class IAPPanel : Panel
{
    [SerializeField] Button tryFreeBtn;
    [SerializeField] Button buyBtn;
    [SerializeField] Button backBtn;

    private void Awake()
    {
        EventManager.OnShowPanel += InitScreen;
    }
    private void OnDestroy()
    {
        EventManager.OnShowPanel += InitScreen;
    }

    private void InitScreen()
    {

        if (DataMainManager.instance.LocalDataStorage.isRemoveAds)
        {
            buyBtn.interactable = false;
        }
        if (DataMainManager.instance.LocalDataStorage.isTryFree)
        {
            tryFreeBtn.interactable = false;
        }
    }

    private void Start()
    {
        tryFreeBtn.onClick.AddListener(OnTryFreeClick);
        buyBtn.onClick.AddListener(OnBuyClick);
        backBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnTryFreeClick()
    {
        DataMainManager.instance.LocalDataStorage.isTryFree = true;
    }

    private void OnBuyClick()
    {
        DataMainManager.instance.LocalDataStorage.isRemoveAds = true;
        AdsManager.Instance.SetRemovedAds();
        buyBtn.interactable = false;
        buyBtn.transform.DOPunchScale(Vector3.one * .1f, 0.5f, 5);
    }
}
