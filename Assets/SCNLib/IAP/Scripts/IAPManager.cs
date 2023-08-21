using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Purchasing.Security;
using System.Collections;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance;

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public UnityEvent OnBuyDone = null, OnBuyFail = null;

    public bool IsInitDone => IsInitialized();
    private bool isBuying = false;

    GameObject removeAdsPopup, spawnedPopup;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public IEnumerator IEStart()
    {
        yield return new WaitForEndOfFrame();
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());



        var catalog = ProductCatalog.LoadDefaultCatalog();

        foreach (var product in catalog.allValidProducts)
        {
            if (product.allStoreIDs.Count > 0)
            {
                var ids = new IDs();
                foreach (var storeID in product.allStoreIDs)
                {
                    ids.Add(storeID.id, storeID.store);
                }
                builder.AddProduct(product.id, product.type, ids);
            }
            else
            {
                builder.AddProduct(product.id, product.type);
            }

        }


        // Continue adding the non-consumable product.

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 


        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    public Product GetProduct(string productID)
    {
        Product product = m_StoreController.products.WithID(productID);
        return product;
    }

    public bool CheckBought(string ID)
    {
        Product product = m_StoreController.products.WithID(ID);
        if (product != null && product.hasReceipt)
        {
            return true;
        }
        return false;
    }
    string productId;
    string productName;
    string price;
    string currency;
    public void BuyProductID(string productId, string product_name, string price, string currency)
    {
        if (isBuying) return;
        if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(product_name))
            Debug.LogError("buy productId : " + productId + " - product_name : " + product_name);
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            isBuying = true;
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null)
                this.productId = product.definition.id;
            this.productName = product_name;
            this.price = price;
            this.currency = currency;
            Debug.LogError("product : " + (product != null));
            if (product != null)
                Debug.LogError("product availableToPurchase : " + product.availableToPurchase);
            if (product != null && product.availableToPurchase)
            {
                //Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                //NoticeManager.Instance.LogNotice(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product, productId);
            }
            // Otherwise ...
            else
            {
                isBuying = false;
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                //NoticeManager.Instance.LogNotice("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            isBuying = false;
            Debug.Log("BuyProductID FAIL. Not initialized.");
            //NoticeManager.Instance.LogNotice("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            //NoticeManager.Instance.LogNotice("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var catalog = ProductCatalog.LoadDefaultCatalog();

        foreach (var product in catalog.allValidProducts)
        {

            if (String.Equals(args.purchasedProduct.definition.id, product.id, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                //#if  UNITY_EDITOR
                if (OnBuyDone != null)
                {
                    OnBuyDone.Invoke();
                    OnBuyDone.RemoveAllListeners();
                }
                //#endif
                return PurchaseProcessingResult.Complete;
            }
        }
        isBuying = false;
        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Pending;
    }

    bool checkProductList(PurchaseEventArgs args, List<string> ProductList)
    {
        foreach (string s in ProductList)
        {
            if (String.Equals(args.purchasedProduct.definition.id, s, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                //NoticeManager.Instance.LogNotice(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                OnBuyFail.RemoveAllListeners();
                if (OnBuyDone != null)
                {
                    OnBuyDone.Invoke();
                    OnBuyDone.RemoveAllListeners();
                }
                return true;
            }
        }
        return false;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        isBuying = false;
        OnBuyDone.RemoveAllListeners();
        if (OnBuyFail != null)
        {
            OnBuyFail.Invoke();
            OnBuyFail.RemoveAllListeners();
        }

        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

    }
    public void OpenPanel()
    {
        if (removeAdsPopup == null)
        {
            removeAdsPopup = Resources.Load<GameObject>("RemoveAdsCanvas");
        }

        if (spawnedPopup == null)
        {
            spawnedPopup = Instantiate(removeAdsPopup, transform);
        }
        else
        {
            spawnedPopup.gameObject.SetActive(true);
        }

    }
}