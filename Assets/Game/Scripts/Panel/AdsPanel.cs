using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using SCN;

public class AdsPanel : Panel
{
    [SerializeField] Button backBtn;
    [SerializeField] Button confirmBtn;
    [SerializeField] Button buyWithCoin;
    [SerializeField] Button cancelBtn;
    [SerializeField] Button confirmWithNoCoinBtn;
    [SerializeField] Text title;
    [SerializeField] Image picture;
    [SerializeField] Image background;
    [SerializeField] Text coinText;
    [SerializeField] Text textItem;

    private int curInstanceId;
    int curIdx;
    private PriceItem priceItem;
    bool isWatchAds;
    Vector3 startScale;
    bool isStart = true;
    

    protected override void Start()
    {
        InitEvent();
        if (isStart)
        {
            base.Hide();
            isStart = false;
        }
    }
    private void OnEnable()
    {
        coinText.text = priceItem.price + "";
        transform.SetAsLastSibling();

        if (isStart) return;
        GUIManager.instance.SetLayerHUD(100);
    }
    private void OnDisable()
    {
        GUIManager.instance.SetLayerHUD(1);
    }

    void InitEvent()
    {
        backBtn.onClick.AddListener(OnCancelClick);
        confirmBtn.onClick.AddListener(OnConfirmClick);
        confirmWithNoCoinBtn.onClick.AddListener(OnConfirmClick);
        buyWithCoin.onClick.AddListener(OnBuyWithCoin);
        cancelBtn.onClick.AddListener(() => base.Hide());
        EventManager.InitAdsPanel += GetInitAds;
        EventManager.InitAdsPanelWithNoCoin += GetInitAdsWithNoCoin;
        EventDispatcher.Instance.RegisterListener<EventKey.InitAdsPanel>(GetInitAds);
    }
    private void OnDestroy()
    {
        EventManager.InitAdsPanel -= GetInitAds;
        EventManager.InitAdsPanelWithNoCoin -= GetInitAdsWithNoCoin;
        EventDispatcher.Instance.RemoveListener<EventKey.InitAdsPanel>(GetInitAds);
    }

    private void GetInitAds(EventKey.InitAdsPanel item)
    {
        if(!item.textStr.Equals(""))
        {
            curInstanceId = item.instanceID;
            curIdx = item.idxItem;

            picture.gameObject.SetActive(false);
            textItem.text = item.textStr;
            textItem.gameObject.SetActive(true);

            cancelBtn.gameObject.SetActive(true);
            confirmWithNoCoinBtn.gameObject.SetActive(true);
            confirmBtn.gameObject.SetActive(false);
            buyWithCoin.gameObject.SetActive(false);
            title.text = "WATCH A VIDEO TO UNLOCK ITEM";
            return;
        }
        curInstanceId = item.instanceID;
        curIdx = item.idxItem;
        picture.sprite = item.sprite;
        GameManager.instance.ScaleImage(picture, 500, 390);
        picture.color = Color.white;
        picture.gameObject.SetActive(true);
        textItem.gameObject.SetActive(false);

        cancelBtn.gameObject.SetActive(true);
        confirmWithNoCoinBtn.gameObject.SetActive(true);
        confirmBtn.gameObject.SetActive(false);
        buyWithCoin.gameObject.SetActive(false);

        title.text = "WATCH A VIDEO TO UNLOCK ITEM";
    }

    private void GetInitAds(int id_, PriceItem priceItem_, Sprite sprite)
    {
        curIdx = id_;
        priceItem = priceItem_;
        picture.sprite = sprite;
        picture.SetNativeSize();
        picture.sprite = sprite;
        GameManager.instance.ScaleImage(picture, 500, 390);
        picture.color = Color.white;
      //  picture.DOFade(0.7f, 0);
        picture.gameObject.SetActive(true);
        textItem.gameObject.SetActive(false);

        cancelBtn.gameObject.SetActive(false);
        confirmWithNoCoinBtn.gameObject.SetActive(false); 
        confirmBtn.gameObject.SetActive(true);
        buyWithCoin.gameObject.SetActive(true);

        title.text = "WATCH A VIDEO TO UNLOCK OR BUY CHARACTER WITH COINS";
    }
    private void GetInitAdsWithNoCoin(int id_, Sprite sprite)
    {
        curIdx = id_;
        picture.sprite = sprite;
        picture.SetNativeSize();
        picture.sprite = sprite;
        GameManager.instance.ScaleImage(picture, 500, 390);
        picture.color = Color.white;
        picture.gameObject.SetActive(true);
        textItem.gameObject.SetActive(false);

        cancelBtn.gameObject.SetActive(true);
        confirmWithNoCoinBtn.gameObject.SetActive(true);
        confirmBtn.gameObject.SetActive(false);
        buyWithCoin.gameObject.SetActive(false);

        title.text = "WATCH A VIDEO TO UNLOCK ITEM";
    }

    void OnBuyWithCoin()
    {
        //   SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);

        // Set up coin
        var skinCoin = GameManager.instance.GetCoin(curIdx);
        DataMainManager.instance.UpdateCoin(skinCoin, false, () =>
        {
            base.Hide();
            EventManager.OnWatchAds?.Invoke(curIdx, priceItem);
        },
        () =>
        {
            OnFailBuyWithCoin();
        });
    }
    void OnFailBuyWithCoin()
    {
        EventManager.OpenPanel?.Invoke(PanelType.BuyFailPanel);
    }

    public void AssignItem(int _idx, Sprite sprite, string _title = "", bool isHideItem = true)
    {
        curIdx = _idx;
        title.text = _title == "" ? title.text : _title;
        picture.sprite = sprite;
        GameManager.instance.ScaleImage(picture, 500, 390);
        picture.color = isHideItem ? Color.black : Color.white;
        picture.gameObject.SetActive(true);
        background.rectTransform.sizeDelta = new Vector2(1378, 940);
        title.fontSize = 120;
    }

    public void AssignItem(int _idx, string _title = "")
    {
        title.text = _title == "" ? title.text : _title;
        curIdx = _idx;
        background.rectTransform.sizeDelta = new Vector2(1776, 711);
        title.fontSize = 130;
        picture.gameObject.SetActive(false);
    }

    private void OnCancelClick()
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        base.Hide();
    }

    private void OnConfirmClick()
    {
         FirebaseManager.instance.LogWatchAds("Bam_nut");
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        OnWatchAds();
    }
    void OnWatchAds()
    {
        base.Hide();

        FirebaseManager.instance.LogWatchAds("Reward");
        EventManager.OnWatchAds?.Invoke(curIdx, priceItem);
        EventDispatcher.Instance.Dispatch(new EventKey.OnWatchAds { instanceID = curInstanceId, idxItem = curIdx });
    }
}
