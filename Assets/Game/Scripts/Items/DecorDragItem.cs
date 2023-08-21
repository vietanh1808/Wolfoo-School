using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DecorDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image image;
    [SerializeField] Button lockBtn;

    int id;
    bool canDrag = true;
    Vector3 startPos;
    Vector3 startLocalPos;
    Transform dragParent;
    Transform startParent;
    private Vector3 endPos;
    private bool hasAnim;
    private GameObject eyeAnim;
    private int trayIdx;
    bool isOutScroll;
    private Tweener tweenRotate;
    Vector3 startScale;
    private bool isCompleted;

    public Image Image { get => image; }

    public void AssignItem(int _id, Sprite _sprite, Transform _dragParent)
    {
        id = _id;
        image.sprite = _sprite;
        image.SetNativeSize();

        startParent = transform.parent;
        dragParent = _dragParent;
        startPos = transform.position;
        startLocalPos = transform.localPosition;
        startScale = transform.localScale;

        var curParent = transform.parent.GetComponent<RectTransform>();
        curParent.sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width, 100);

        lockBtn.onClick.AddListener(OnUnlockItem);
    }
    private void Start()
    {
        EventManager.OnEndSession += GetEndSession;
        EventManager.OnWatchAds += GetWatchAds;
    }
    private void OnDisable()
    {

        EventManager.OnWatchAds -= GetWatchAds;
        EventManager.OnEndSession -= GetEndSession;
    }
    private void OnUnlockItem()
    {
        EventManager.InitAdsPanelWithNoCoin?.Invoke(id, image.sprite);
        EventManager.OpenPanel?.Invoke(PanelType.Ads);
    }
    private void GetWatchAds(int idx, PriceItem priceItem)
    {
        if (id != idx) return;

        DataMainManager.instance.LocalDataStorage.unlockFruitDecorTopics[id] = true;
        DataMainManager.instance.SaveItem(DataMainManager.StorageKey.Data);
        Unlock();
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

    private void GetEndSession()
    {
        isCompleted = true;
    }

    public void AssignItem(int _trayIdx, Vector3 _scale, Vector3 _endPos)
    {
        trayIdx = _trayIdx;
        transform.localScale = _scale;
        endPos = _endPos;
    }
    public void AssignItem(Vector3 _endPos)
    {
        endPos = _endPos;
    }
    public void AssignItem(GameObject _eyeAnimPb)
    {
        hasAnim = true;
        eyeAnim = Instantiate(_eyeAnimPb, transform);
        eyeAnim.transform.localPosition = Vector3.zero;
        eyeAnim.transform.localScale = Vector3.one;
       // eyeAnim.transform.position = Vector3.down * image.rectTransform.rect.height / 2 / 100;
        eyeAnim.gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (isCompleted) return;

        EventManager.OnBeginDragDecor?.Invoke(trayIdx, this);
        transform.SetParent(dragParent);
    
        if(!isOutScroll)
            startPos = transform.position;
        
        transform.localScale += Vector3.one * 0.3f;
        startParent.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (isCompleted) return;

        GameManager.instance.GetCurrentPosition(transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (isCompleted) return;
        transform.localScale = startScale;
        EventManager.OnEndDrag?.Invoke(id, this, Vector2.Distance(transform.position, endPos));
    }
    public void OnCorrect(Transform endParent, System.Action OnComplete = null)
    {
        isOutScroll = true;
        transform.SetParent(endParent);
        OnComplete?.Invoke();

        if(!hasAnim)
        {
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
            if (tweenRotate == null || !tweenRotate.IsActive())
                tweenRotate = transform.DOPunchRotation(Vector3.forward * 10, 0.5f, 2);
        }
        else
        {
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Wow, CharacterAnimation.SexType.Boy);
            eyeAnim.SetActive(true);
            image.gameObject.SetActive(false);
        }
    }
    public void OnIncorrect()
    {
        canDrag = false;
        isOutScroll = false;
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Incorrect);
        transform.DOMove(startPos, 1).OnComplete(() =>
        {
            if(hasAnim)
            {
                eyeAnim.SetActive(false);
                image.gameObject.SetActive(true);
            }
            transform.SetParent(startParent);
            startParent.gameObject.SetActive(true);
            transform.localPosition = startLocalPos;
            canDrag = true;
        });
    }
}
