using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SCN;
using Coffee.UIExtensions;
using SCN.Ads;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private SlotPiece[] slotPieces;
    [SerializeField] private Transform[] transStarts;
    [SerializeField] private Transform dragTrans;
    [SerializeField] private ItemPiece itemPiece;
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private Transform contenPieces;
    [SerializeField] private Image imageHint;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Image hintTrans;
    [SerializeField] private ButtonBase btnCloseLevel;
    private List<ItemPiece> itemPieces = new List<ItemPiece>();
    private List<Piece> pieces;
    private Tween tweenDelay;
    [ContextMenu("ValidateLevel")]
    private void ValidateLevel()
    {
        slotPieces = transform.GetComponentsInChildren<SlotPiece>();
        //pieces = transform.GetComponentsInChildren<Piece>();
        //for (int i = 0; i < pieces.Length; i++)
        //{
        //    pieces[i].OnValidateLevel(slotPieces[i]);
        //}
    }
    private void Start()
    {
        EventDispatcher.Instance.RegisterListener<EventKey.OnDragPiece>(OnDragPiece);
        EventDispatcher.Instance.RegisterListener<EventKey.CheckProgress>(CheckProgress);
        EventDispatcher.Instance.RegisterListener<EventKey.DoneDragItemPiece>(DoneDragItemPiece);
        EventDispatcher.Instance.RegisterListener<EventKey.ReturnItemPiece>(ReturnItemPiece);
        btnCloseLevel.onClick.AddListener(OnCloseLevel);
        //EventDispatcher.Instance.RegisterListener<EventKey.OnHintBottom>(OnHintBottom);
        //EventDispatcher.Instance.RegisterListener<EventKey.OnHintImage>(OnHintImage);
        //EventDispatcher.Instance.RegisterListener<EventKey.OnHintPiece>(OnHintPiece);
        //EventDispatcher.Instance.RegisterListener<EventKey.OnHint>(OnHint);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener<EventKey.OnDragPiece>(OnDragPiece);
        EventDispatcher.Instance.RemoveListener<EventKey.CheckProgress>(CheckProgress);
        EventDispatcher.Instance.RemoveListener<EventKey.DoneDragItemPiece>(DoneDragItemPiece);
        EventDispatcher.Instance.RemoveListener<EventKey.ReturnItemPiece>(ReturnItemPiece);
        //EventDispatcher.Instance.RemoveListener<EventKey.OnHintBottom>(OnHintBottom);
        //EventDispatcher.Instance.RemoveListener<EventKey.OnHintImage>(OnHintImage);
        //EventDispatcher.Instance.RemoveListener<EventKey.OnHintPiece>(OnHintPiece);
        //EventDispatcher.Instance.RemoveListener<EventKey.OnHint>(OnHint);
    }
    private void OnCloseLevel()
    {
        tweenDelay?.Kill();
        if (AdsManager.Instance.HasInters)
        {
            AdsManager.Instance.ShowInterstitial((a) =>
            {
                EventDispatcher.Instance.Dispatch(new EventKey.OnCloseLevelJigsaw());
             //   FirebaseManager.Instance.WatchInter("Jigsaw");
                gameObject.SetActive(false);
            });
        }
        else
        {
            EventDispatcher.Instance.Dispatch(new EventKey.OnCloseLevelJigsaw());
            gameObject.SetActive(false);
        }
    }
    public void OnLevelStart(Sprite img)
    {
        imageHint.sprite = img;
        //for (int i = 0; i < pieces.Length; i++)
        //{
        //    var rd = Random.Range(0, 2);
        //    var item = pieces[i];
        //    var randomX = Random.Range(-30f, 30f);
        //    var randomY = Random.Range(-300, 300);
        //    item.transform.position = transStarts[rd].position + new Vector3(randomX, randomY);
        //    item.IsInstalled = false;
        //    item.transform.SetParent(dragTrans);
        //}
        pieces = new List<Piece>();
        var slotsTemp = slotPieces;
        Shuffle(slotsTemp);
        for (int i = 0; i < slotsTemp.Length; i++)
        {
            var item = Instantiate(itemPiece, slotsTemp[i].transform);
            item.InitPiece(slotsTemp[i].transform, imageHint,i);
            item.transform.SetParent(contenPieces);
            item.transform.localScale = Vector3.one * .6f;
            itemPieces.Add(item);
        }
      
    }
    //private void OnHint()
    //{
    //    bool canHint = false;
    //    var itemHint = itemPieces[0];
    //    for (int i = 0; i < itemPieces.Count; i++)
    //    {
    //        if (!itemPieces[i].IsSpawned)
    //        {
    //            canHint = true;
    //            itemHint = itemPieces[i];
    //            break;
    //        }
    //    }
    //    if (canHint)
    //    {
    //        if(PlayerData.CountHint > 0)
    //        {
    //            PlayerData.CountHint -= 1;
    //            itemHint.OnHint();
    //            EventDispatcher.Instance.Dispatch(new EventKey.UpdateUIIngame());
    //        }
    //        else
    //        {
    //            if (AdsManager.Instance.HasRewardVideo)
    //            {
    //                AdsManager.Instance.ShowRewardVideo(() =>
    //                {
    //                    FirebaseManager.Instance.TrackingReward();
    //                    PlayerData.CountHint = 3;
    //                    itemHint.OnHint();
    //                    EventDispatcher.Instance.Dispatch(new EventKey.UpdateUIIngame());
    //                });
    //            }
    //        }
            
    //    }
    //}
    private void Shuffle(SlotPiece[] ts)
    {
        var count = ts.Length;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
    private void OnDragPiece(EventKey.OnDragPiece data)
    {
        var currentPiece = Instantiate(piecePrefab, dragTrans);
        currentPiece.transform.rotation = data.quaternion;
        currentPiece.gameObject.SetActive(false);
        currentPiece.transform.SetAsLastSibling();
        currentPiece.InitPiece(slotPieces[data.idPiece].transform, imageHint, data.idPiece, scrollRect.transform);
        currentPiece.transform.position = Input.mousePosition;
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.localScale = Vector3.one * 1.15f;
        scrollRect.enabled = false;
        pieces.Add(currentPiece);
    }
    private void ReturnItemPiece(EventKey.ReturnItemPiece data)
    {
        scrollRect.enabled = true;
    }
    private void DoneDragItemPiece()
    {
        scrollRect.enabled = true;
    }

    private void CheckProgress()
    {
        hintTrans.enabled = false;
        bool isWin = true;
        for (int i = 0; i < slotPieces.Length; i++)
        {
            if(slotPieces[i].IsFull == false)
            {
                isWin = false;
                return;
            }
        }
        if (isWin) OnWin();
    }
    private void OnWin()
    {
        imageHint.gameObject.SetActive(true);
        imageHint.color = Color.white;
        imageHint.GetComponent<UIShiny>().enabled = true;
        tweenDelay= DOVirtual.DelayedCall(2.5f,()=> {
            if (AdsManager.Instance.HasInters)
            {
                AdsManager.Instance.ShowInterstitial((a) =>
                {
                    EventDispatcher.Instance.Dispatch(new EventKey.OnCloseLevelJigsaw());
                 //   FirebaseManager.Instance.WatchInter("Jigsaw");
                    gameObject.SetActive(false);
                });
            }
            else
            {
                EventDispatcher.Instance.Dispatch(new EventKey.OnCloseLevelJigsaw());
                gameObject.SetActive(false);
            }
        });
    }
    //private void OnHintImage(EventKey.OnHintImage data)
    //{
    //    imageHint.gameObject.SetActive(data.isHint);
    //}
    //private void OnHintPiece(EventKey.OnHintPiece data)
    //{
    //    hintTrans.enabled = true;
    //    slotPieces[data.idPiece].transform.SetParent(hintTrans.transform);
    //    var currentPiece = Instantiate(piecePrefab, dragTrans);
    //    currentPiece.transform.rotation = data.quaternion;
    //    currentPiece.gameObject.SetActive(false);
    //    currentPiece.transform.SetAsLastSibling();
    //    currentPiece.InitPiece(slotPieces[data.idPiece].transform, imageHint, data.idPiece, scrollRect.transform);
    //    currentPiece.transform.position = scrollRect.transform.position - new Vector3(150f, 0, 0);
    //    currentPiece.gameObject.SetActive(true);
    //    currentPiece.transform.SetParent(hintTrans.transform);
    //    scrollRect.enabled = false;
    //    pieces.Add(currentPiece);
    //    currentPiece.OnHintPiece();
    //    DOVirtual.DelayedCall(1f, () =>
    //    {
    //        slotPieces[data.idPiece].transform.SetParent(transform.GetChild(1).transform);
    //    });
    //}
    //private void OnHintBottom(EventKey.OnHintBottom data)
    //{
    //    var colorBottom = slotPieces[0].GetComponent<Image>().color;
    //    for (int i = 0; i < slotPieces.Length; i++)
    //    {
    //        colorBottom.a = data.isHint ? colorBottom.a = 1: colorBottom.a = 0;
    //        slotPieces[i].GetComponent<Image>().color = colorBottom;
    //    }
    //}
}
