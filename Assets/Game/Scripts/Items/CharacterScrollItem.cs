using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterScrollItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] Button lockBtn;
    [SerializeField] Image image;
    [SerializeField] CharacterAnimation character;
    private int id;
    private PriceItem priceItem;
    private Vector3 startPos;
    private int startSiblingIdx;
    private float distance;
    bool canDrag;
    private Transform startParent;
    private Tweener punchScaleTween;
    private Vector3 startScale;

    private void Start()
    {
        lockBtn.onClick.AddListener(OnUnlockItem);
        startSiblingIdx = transform.GetSiblingIndex();
    }
    private void OnEnable()
    {
        EventManager.OnWatchAds += GetWatchAds;
    }
    private void OnDisable()
    {
        EventManager.OnWatchAds -= GetWatchAds;
    }
    private void GetWatchAds(int idx, PriceItem priceItem)
    {
        if (id != idx) return;

        DataMainManager.instance.LocalDataStorage.unlockCharacters[id] = true;
        DataMainManager.instance.SaveItem(DataMainManager.StorageKey.Data);
        Unlock();
    }

    public void AssignItem(int id_, Sprite sprite, PriceItem priceItem_)
    {
        Debug.Log("Assign ITem");
        id = id_;
        character.transform.localPosition = Vector3.zero;
        character.gameObject.SetActive(false);
        priceItem = priceItem_;

        canDrag = true;
        startParent = transform.parent;
        startPos = transform.position;
        startScale = transform.localScale;

        image.sprite = sprite;
        image.SetNativeSize();
    }
    public void AssignItem(Vector3 startPos_)
    {
        startPos = startPos_;
    }

    public void SetToScrollView()
    {
        Debug.Log("SetToScrollView");
        GetComponent<Wolfoo>().enabled = false;
        GetComponent<CharacterScrollItem>().enabled = true;

        transform.SetParent(startParent);
        transform.SetSiblingIndex(startSiblingIdx);

        image.gameObject.SetActive(true);
        character.gameObject.SetActive(false);

        canDrag = true;
    }

    private void OnUnlockItem()
    {
        EventManager.InitAdsPanel?.Invoke(id, priceItem, image.sprite);
        EventManager.OpenPanel?.Invoke(PanelType.Ads);
    }
    public void Unlock()
    {
        canDrag = true;
        image.color = Color.white;
        image.DOFade(1, 0);
        lockBtn.gameObject.SetActive(false);
    }
    public void Lock()
    {
        canDrag = false;
        image.color = Color.black;
        image.DOFade(0.6f, 0);
        lockBtn.gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        transform.SetParent(transform.parent.parent.parent);
        startPos = transform.position;
        image.gameObject.SetActive(false);
        character.gameObject.SetActive(true);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        GameManager.instance.GetCurrentPosition(transform);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        canDrag = false;
        distance = transform.position.y - GameManager.instance.ChooseCharacterZone.position.y;
        if(distance > 1)
        {
            GetComponent<Wolfoo>().enabled = true;
            GetComponent<CharacterScrollItem>().enabled = false;
        }
        else
        {
            if(GetComponent<Wolfoo>().enabled)
            {
                GetComponent<Wolfoo>().SetNotMoveToGround();
                GetComponent<Wolfoo>().DisableDrag();
            }
            transform.DOMove(startPos, 35)
            .SetSpeedBased(true)
            .OnComplete(() =>
            {
                canDrag = true;
                transform.SetParent(startParent);
                image.gameObject.SetActive(true);
                character.gameObject.SetActive(false);
            });
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(punchScaleTween != null)
        {
            punchScaleTween?.Kill();
            transform.localScale = startScale;
        }

        punchScaleTween = transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0), 0.5f, 1);
    }
}
