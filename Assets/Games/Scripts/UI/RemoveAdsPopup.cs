using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Firebase.Analytics;
using DG.Tweening;
using SCN;
using SCN.Ads;

public class RemoveAdsPopup : MonoBehaviour
{

    [SerializeField] private ButtonBase close;
    [SerializeField] private string idRemoveAds = "wolfoo.ice.cream.icecreammaker.noads";
    [SerializeField] private ButtonBase buttonRestore;
    [SerializeField] private ButtonBase buttonBuy;
    [SerializeField] private Text costBuyTxt;
    private void Start()
    {
        close.onClick.AddListener(OnClose);
        buttonBuy.onClick.AddListener(() =>
        {
            IAPManager.Instance.BuyProductID(idRemoveAds, "Remove Ads", "2.99", "USD");
        });
        buttonRestore.onClick.AddListener(IAPManager.Instance.RestorePurchases);
        IAPManager.Instance.OnBuyDone.AddListener(OnRemoveAds);
        AssignSaleOff();
    }
    private void OnClose()
    {
        transform.parent.gameObject.SetActive(false);
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }
    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == idRemoveAds)
        {
            OnRemoveAds();
        }
        Debug.Log($"Purchase complete - Product: '{product.definition.id}'");
    }
    private void OnRemoveAds()
    {
        AdsManager.Instance.SetRemovedAds();

        //Bắn event xử lý ở đây
        EventManager.OnBuyRemoveAds?.Invoke();
        DOVirtual.DelayedCall(0.25f, ()=> {
            transform.parent.gameObject.SetActive(false);
        });
        Debug.Log("Remove Ads");
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        FirebaseAnalytics.LogEvent("RemovedAds", "RemovedAds", 1);
    }
    private void AssignSaleOff()
    {
        Product product = IAPManager.Instance.GetProduct(idRemoveAds);
        if (product != null)
        {
            costBuyTxt.text = product.metadata.localizedPriceString;
        }
        

    }
}
