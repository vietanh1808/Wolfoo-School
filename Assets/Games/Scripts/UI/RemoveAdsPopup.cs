using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using SCN;
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
        });
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
        //Bắn event xử lý ở đây
        EventManager.OnBuyRemoveAds?.Invoke();
        DOVirtual.DelayedCall(0.25f, ()=> {
            transform.parent.gameObject.SetActive(false);
        });
        Debug.Log("Remove Ads");
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
    }
    private void AssignSaleOff()
    {

    }
}
