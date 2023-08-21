using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SCN.Common;
using DG.Tweening;

public class HomePanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] List<CharacterAnimation> characters;
    [SerializeField] List<GameObject> modePbs;
    [SerializeField] List<Sprite> iconSprites;
    [SerializeField] List<Sprite> tableSprites;
    [SerializeField] List<Transform> listTablePos;
    [SerializeField] Transform startTrans;
    [SerializeField] Transform endTrans;
    [SerializeField] Transform backMoveTrans;
    [SerializeField] float velocity;
    [SerializeField] Button giftBtn;
    [SerializeField] CharacterAnimation teacherCharacter;
    [SerializeField] List<Transform> listPos;
    [SerializeField] Button backBtn;
    [SerializeField] Button settingBtn;
    [SerializeField] Sprite giftSprite;
    [SerializeField] Button nextBtn;
    [SerializeField] Button prevBtn;
    [SerializeField] Button katBtn;

    float beginXPos;
    float curXPos;
    bool isSpawned;
    float offset;
    private bool canDrag;

    private void OnEnable()
    {
        isSpawned = false;
        AdsManager.Instance.ShowBanner();
        teacherCharacter.PlayWaveHand();
        EventManager.OnWatchAds += GetWatchAds;
    }
    private void Start()
    {
        nextBtn.onClick.AddListener(OnNext);
        prevBtn.onClick.AddListener(OnPrev);
        backBtn.onClick.AddListener(OnBack);
        settingBtn.onClick.AddListener(OnSetting);
        giftBtn.onClick.AddListener(OnOpenGift);
        katBtn.onClick.AddListener(OnOpenGift);

        InitScreen();
        OnBackMoveTutorial();
    }
    private void OnDisable()
    {
        EventManager.OnWatchAds -= GetWatchAds;
    }

    private void OnDestroy()
    {
    }

    void InitScreen()
    {
        // Init Mode Item
       

        giftBtn.transform.DOPunchScale(Vector3.one * 0.1f, 2, 2)
        .OnStart(() =>
        {
            giftBtn.transform.DOPunchRotation(Vector3.one * 2f, 2, 2);
        })
        .SetLoops(-1)
        .SetDelay(5);
    }

    void OnBackMoveTutorial()
    {
        canDrag = false;
        backMoveTrans.DOMoveX(startTrans.position.x, 3).OnComplete(() =>
        {
            backMoveTrans.DOMoveX(endTrans.position.x, 2).OnComplete(() =>
            {
                canDrag = true;
            });
        });
    }

    private void GetWatchAds(int obj, PriceItem priceItem)
    {
        //GUIManager.instance.giftPanel.AssignItem();
        //GUIManager.instance.OpenPanel(PanelType.Gift);
    }
    void CheckLimit(Transform targetTrans)
    {
        if (targetTrans.position.x <= startTrans.position.x)
        {
            nextBtn.gameObject.SetActive(false);
            targetTrans.position = new Vector3(startTrans.position.x, targetTrans.position.y, targetTrans.position.z);
            return;
        }
        if (targetTrans.position.x >= endTrans.position.x)
        {
            prevBtn.gameObject.SetActive(false);
            targetTrans.position = new Vector3(endTrans.position.x, targetTrans.position.y, targetTrans.position.z);
            return;
        }
        prevBtn.gameObject.SetActive(true);
        nextBtn.gameObject.SetActive(true);
    }
    void OnNext()
    {
        backMoveTrans.position += Vector3.left * 1f;
        CheckLimit(backMoveTrans);
    }
    void OnPrev()
    {
        backMoveTrans.position += Vector3.right * 1f;
        CheckLimit(backMoveTrans);
    }

    void OnBack()
    {
        gameObject.SetActive(false);
        if (AdsManager.Instance.HasInters)
        {
            AdsManager.Instance.ShowInterstitial(() =>
            {
                FirebaseManager.instance.LogWatchAds("Inters");
                GUIManager.instance.OpenPanel(PanelType.Intro);
            });
        }
        else
        {
            GUIManager.instance.OpenPanel(PanelType.Intro);
        }
    }
    void OnSetting()
    {
        GUIManager.instance.OpenPanel(PanelType.Setting);
    }

    private void GetClickModeItem(int idx)
    {
        if (isSpawned) return;
        isSpawned = true;
        AdsManager.Instance.HideBanner();
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        GUIManager.instance.PlayLoading(() =>
        {
            SoundManager.instance.PlayIngame();
            var mode = Instantiate(modePbs[idx == 0 ? Random.Range(0, 2) : idx + 1], GUIManager.instance.canvas.transform);
            FirebaseManager.instance.LogBeginMode(mode.name);
            gameObject.SetActive(false);
        });
    }

    private void OnOpenGift()
    {
        //SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        //GUIManager.instance.adsPanel.AssignItem(-1, 
        //    giftSprite, 
        //    "Do you want to open a Gift?", 
        //    false);
        //GUIManager.instance.OpenPanel(PanelType.Ads);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        //beginXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        var mousePos = Input.mousePosition;
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(mousePos);
        offset = curPosition.x - backMoveTrans.position.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        //var curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var mousePos = Input.mousePosition;
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(mousePos);
        backMoveTrans.position = new Vector3(curPosition.x - offset, backMoveTrans.position.y, backMoveTrans.position.z);


        //if (curXPos == beginXPos) return;
        ////  CheckLimit(backMoveTrans);
        //if (backMoveTrans.position.x > startTrans.position.x + 0.1f)
        //{
        //    return;
        //}
        //if (backMoveTrans.position.x < endTrans.position.x - 0.1f)
        //{
        //    return;
        //}
        //backMoveTrans.transform.position += Vector3.right * velocity * (curXPos - beginXPos);
        //beginXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        CheckLimit(backMoveTrans);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        //if (backMoveTrans.position.x > startTrans.position.x)
        //{
        //    backMoveTrans.transform.position = Vector2.right * startTrans.position.x;
        //    return;
        //}
        //if (backMoveTrans.position.x < endTrans.position.x)
        //{
        //    backMoveTrans.transform.position = Vector2.right * endTrans.position.x;
        //    return;
        //}
    }
}
