using DG.Tweening;
using SCN;
using SCN.Common;
using SCN.UIExtend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotItem : ScrollItemBase
{
    [SerializeField] Image itemImg;
    [SerializeField] Button coinZoneBtn;

    private ClayModeDataSO data;
    private DataStorage localData;
    private bool isCompare;
    private Tween delayTween;
    private Vector3 startScale;
    private Tweener fadeTween;
    private Tweener colorTween;
    private Vector3 endPos;
    private Transform endTrans;
    private List<Transform> listEndTrans;
    private Transform compareTrans;
    private bool isCompareList;
    private List<bool> listEmpty;
    private int idxVerified;
    private float curVerifyDistance;
    private float distance;
    private float distanceVerified;

    public Sprite ItemSprite { get => itemImg.sprite; }

    private void Start()
    {
        coinZoneBtn.onClick.AddListener(OnBuyWithCoin);
    }
    private void OnDestroy()
    {
        if (delayTween != null) delayTween?.Kill();
        if (fadeTween != null) fadeTween?.Kill();
        if (colorTween != null) colorTween?.Kill();
    }
    private void OnEnable()
    {
        EventDispatcher.Instance.RegisterListener<EventKey.OnWatchAds>(GetConfirmChoose);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener<EventKey.OnWatchAds>(GetConfirmChoose);
    }

    private void GetConfirmChoose(EventKey.OnWatchAds item)
    {
        if (item.instanceID != GetInstanceID()) return;
        if (item.idxItem != order) return;

        OnUnlock();
    }

    void OnUnlock()
    {
        GUIManager.instance.rainbowFx.transform.position = transform.position;
        GUIManager.instance.rainbowFx.Play();
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
        delayTween = DOVirtual.DelayedCall(2, () =>
        {
            GUIManager.instance.rainbowFx.Stop();
        });

        coinZoneBtn.gameObject.SetActive(false);
        itemImg.raycastTarget = true;
   //     fadeTween = itemImg.DOFade(1, 1);
  //      colorTween = itemImg.DOColor(Color.white, 1);

        DataMainManager.instance.LocalDataStorage.unlockFlowerPots[order] = true;
        DataMainManager.instance.SaveItem(DataMainManager.StorageKey.Data);
    }

    private void OnBuyWithCoin()
    {
        EventDispatcher.Instance.Dispatch(
            new EventKey.InitAdsPanel
            {
                idxItem = order,
                instanceID = GetInstanceID(),
                sprite = data.potSprites[order]
            });
        GUIManager.instance.OpenPanel(PanelType.Ads);
    }
    public void AssignItem(Transform _endTrans, Transform _compareTrans, float _distanceVerify = 2)
    {
        endTrans = _endTrans;
        compareTrans = _compareTrans;
        isCompareList = false;
        distanceVerified = _distanceVerify;
    }
    public void AssignItem(List<Transform> _transforms, Transform _compareTrans, List<bool> _emptyIdx, float _distanceVerify = 2)
    {
        listEndTrans = _transforms;
        compareTrans = _compareTrans;
        isCompareList = true;
        listEmpty = _emptyIdx;
        distanceVerified = _distanceVerify;
    }
    public void OnCompare(System.Action OnComplted = null, System.Action OnFail = null)
    {
        // On Compare With List End Pos
        if (isCompareList)
        {
            idxVerified = -1;
            curVerifyDistance = 1000f;
            for (int i = 0; i < listEndTrans.Count; i++)
            {
                if (!listEmpty[i]) continue;

                distance = Vector2.Distance(compareTrans.position, listEndTrans[i].position);
                if (distance <= distanceVerified)
                {
                    if (curVerifyDistance > distance)
                    {
                        idxVerified = i;
                        curVerifyDistance = distance;
                    }
                }
            }

            if (idxVerified != -1)
            {
                listEmpty[idxVerified] = false;
                endTrans = listEndTrans[idxVerified];
            //    if (endParent == null) endParent = endTrans;
                SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
                OnComplted?.Invoke();
            }
            else
            {
                OnFail?.Invoke();
            }
        }
        // On Compare With End Pos
        else
        {
            distance = Vector2.Distance(compareTrans.position, endTrans.position);
            if (distance <= distanceVerified)
            {
                //    SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
                OnComplted?.Invoke();
            }
            else
            {
                OnFail?.Invoke();
            }
        }
    }

    protected override void Setup(int order)
    {
        GameManager.instance.GetDataSO(DataSOType.ClayMode);
        data = GameManager.instance.ClayDataSO;

        DataMainManager.instance.GetData();
        localData = DataMainManager.instance.LocalDataStorage;

        itemImg.sprite = data.potSprites[order];
        itemImg.SetNativeSize();

        startScale = transform.localScale;

        if(localData.unlockFlowerPots[order])
        {
            coinZoneBtn.gameObject.SetActive(false);
            itemImg.raycastTarget = true;
         //   itemImg.DOFade(1, 0);
            itemImg.color = Color.white;
        }
        else
        {
            coinZoneBtn.gameObject.SetActive(true);
            itemImg.raycastTarget = false;
         //   itemImg.DOFade(0.6f, 0);
        //    itemImg.color = Color.black;
            itemImg.color = Color.white;
        }

        Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerDown, OnPointerDown);
        Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerUp, OnPointerUp);
        Master.AddEventTriggerListener(EventTrigger, EventTriggerType.Drag, OnDrag);
    }

    protected override void OnDragOut()
    {
        base.OnDragOut();
    }
    protected override void OnStartDragOut()
    {
        base.OnStartDragOut();
        transform.localScale = startScale + Vector3.one * 0.5f;
        EventDispatcher.Instance.Dispatch(new EventKey.OnBeginDragItem { potItem = this });
    }

    private void OnDrag(BaseEventData arg0)
    {
        if (coinZoneBtn.gameObject.activeSelf) return;
    }

    private void OnPointerUp(BaseEventData arg0)
    {
        if (coinZoneBtn.gameObject.activeSelf) return;
        transform.localScale = startScale;
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragItem { potItem = this });
    }

    private void OnPointerDown(BaseEventData arg0)
    {
        if (coinZoneBtn.gameObject.activeSelf) return;
    }
}
