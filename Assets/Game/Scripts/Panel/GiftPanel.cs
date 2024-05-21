using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using SCN.Ads;

public class GiftPanel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] ParticleSystem rainbowFx;
    [SerializeField] GiftAnimation giftAnim;
    [SerializeField] Image picture;
    [SerializeField] ParticleSystem starExploreFx;
    [SerializeField] Image background;
    [SerializeField] ParticleSystem starUpFx1;
    [SerializeField] ParticleSystem starUpFx2;

    Vector3 startScale;
    private Tweener scaleTween;
    private int idxReward = 0;
    bool isAtHome;
    bool canClick = true;
    int countOpen = 0;
    private GameObject curItem;
    private bool isPutItem;

    public GiftAnimation GiftAnim { get => giftAnim; }

    private void Awake()
    {
        startScale = picture.transform.localScale;
        picture.transform.localScale = Vector3.zero;
        picture.transform.localPosition -= Vector3.down * 3;
    }

    private void Start()
    {
        //stickerData = Resources.Load<StickerDataSO>("Sticker Data");
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(canClick && isPutItem)
            {
                canClick = false;
                OnPutItem();
            }
        }
    }

    private void OnEnable()
    {
        countOpen = 0;
        canClick = true;
        picture.transform.localScale = Vector3.zero;
        giftAnim.ChangeSkin();
    }
    private void OnDisable()
    {
     //   picture.transform.localScale = Vector3.zero;
        giftAnim.PlayIdle();
        isPutItem = false;
    }
    void OnPutItem()
    {
        //  GUIManager.instance.OnPutItem();
        curItem.transform.DOScale(curItem.transform.localScale - Vector3.one * 0.2f, 1)
        .OnComplete(() =>
        {
            background.gameObject.SetActive(true);
            giftAnim.gameObject.SetActive(true);

            giftAnim.PlayPutItemIntoGift(curItem, () =>
            {
                Destroy(GUIManager.instance.CurMode);
                gameObject.SetActive(false);
                AdsManager.Instance.ShowBanner();
                if (AdsManager.Instance.HasInters)
                {
                    AdsManager.Instance.ShowInterstitial((value) =>
                    {
                        GUIManager.instance.OpenPanel(PanelType.Home);
                    });
                }
                else
                {
                    GUIManager.instance.OpenPanel(PanelType.Home);
                }
            });
        });
    }

    public void AssignToPutGift(GameObject _curItem)
    {

        isPutItem = true;
        _curItem.transform.position = new Vector3(
            _curItem.transform.position.x,
            _curItem.transform.position.y,
            transform.position.z);

        rainbowFx.gameObject.SetActive(true);
        background.gameObject.SetActive(false);
        giftAnim.gameObject.SetActive(false);
        giftAnim.PlayOpenIdle();

        _curItem.transform.SetParent(transform);
        curItem = _curItem;

        GUIManager.instance.OpenPanel(PanelType.Gift);
        starUpFx1.Play();
        starUpFx2.Play();
    }   

    void PictureDance()
    {
        float scale = 0.1f;
        scaleTween = picture.transform.DOScale(picture.transform.localScale - new Vector3(scale, -scale), 0.5f)
        .OnComplete(() =>
        {
            //picture.transform.DOScale(picture.transform.localScale - new Vector3(-scale, scale), 0.5f)
        }).SetLoops(-1, LoopType.Yoyo);
    }
    void GetGiftClick()
    {
        int time = 2;
        giftAnim.PlayOpenAnim(() =>
        {
            picture.transform.DOJump(picture.transform.position, 2, 1, time);
            picture.transform.DOScale(startScale, time).OnComplete(() =>
            {
                giftAnim.PlayIdle();
                PictureDance();
                canClick = true;
            });
            DOVirtual.DelayedCall(time / 2, () =>
            {
                starUpFx1.Play();
                starUpFx2.Play();
                rainbowFx.gameObject.SetActive(true);
            });
            DOVirtual.DelayedCall(time / 7, () =>
            {
                starExploreFx.gameObject.SetActive(true);
                starExploreFx.Play();
            });
        });
    }

    public void AssignItem(Sprite sprite, bool _hasReward = false, int _idx = -1)
    {
        if(_hasReward)
        {
            idxReward = _idx;
        }

        picture.sprite = sprite;
        picture.SetNativeSize();
        GUIManager.instance.ScaleImage(picture, 600, 600);
        giftAnim.PlayIdle();
        giftAnim.gameObject.SetActive(true);
    }
    /// <summary>
    /// Get random Picture in Local Storage
    /// </summary>
    public void AssignItem()
    {
       // int rdIdx = DataManager.instance.GetNextLockIdxItem();
        //if(stickerData == null)
        //    stickerData = Resources.Load<StickerDataSO>("Sticker Data");
        //int rdIdx = Random.Range(0, stickerData.pictures.Count);
        //GameManager.instance.curIdx = rdIdx;

        //var sprite = stickerData.pictures[rdIdx];
        //picture.sprite = sprite;
        //picture.SetNativeSize();
        //GUIManager.instance.ScaleImage(picture, 600, 600);
        //// DataManager.instance.UpdateData(rdIdx);
        //giftAnim.PlayIdle();
        //giftAnim.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClick || isPutItem) return;

        canClick = false;
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);

        if (countOpen == 0)
        {
            countOpen++;
            GetGiftClick();
        }
        else if (countOpen == 1)
        {
            scaleTween?.Kill();
            rainbowFx.gameObject.SetActive(false);
            gameObject.SetActive(false);
            EventManager.OnCompleteOpenGift?.Invoke(false);
        }
    }
}
