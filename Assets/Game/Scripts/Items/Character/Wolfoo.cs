using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using SCN;

public class Wolfoo : BackItem
{
    [SerializeField] Image handItemImg;
    [SerializeField] ParticleSystem starFx;
    [SerializeField] CharacterAnimation wolfooAnim;
    [SerializeField] Transform mouthZone;

    private float distance_;
    GameObject curItem;
    Vector3 startScaleItem;
    bool isSitToChair;

    protected override void Start()
    {
        base.Start();
    //    transform.localScale = Vector3.one * 0.3f;
        isClick = true;
        isDrag = true;
        isWolfoo = true;
        handItemImg.gameObject.SetActive(false);

        if(content == null)
        {
            content = GUIManager.instance.GetCurContent();
            ground = GUIManager.instance.GetCurGround();
            transform.SetParent(content.transform);
       //     MoveToGround();
        }

    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        isSitToChair = false;
        wolfooAnim.PlaySit();
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (!isSitToChair)
            wolfooAnim.PlayIdle();
        transform.rotation = Quaternion.Euler(Vector3.zero);
        EventDispatcher.Instance.Dispatch(new EventKey.OnEndDragBackItem { wolfoo = this, backItem = this });
    }

    public void OnSitToChair( Vector3 endPos)
    {
        isSitToChair = true;
        SetNotMoveToGround();
        DisableDrag();

        wolfooAnim.PlaySit();
        transform.position = endPos;
    }
    protected override void GetEndDragItem(EventKey.OnEndDragBackItem item)
    {
        base.GetEndDragItem(item);
        if (item.backItem == this) return;
        if(item.book != null)
        {
            distance_ = Vector2.Distance(transform.position, item.book.transform.position);
            if (distance_ <= 3f && item.book.transform.position.y - wolfooAnim.transform.position.y > 1)
            {
                if (curItem != null)
                {
                    curItem.transform.localScale = startScaleItem;
                    curItem.SetActive(true);
                }

                startScaleItem = item.book.transform.localScale;
                curItem = item.book.gameObject;
                item.book.GetCarryBook();

                item.book.SetNotMoveToGround();
                item.book.DisableDrag();
                item.book.gameObject.SetActive(false);
                OnCarryBook(item.book);
            }
        }
    }
    public override void GetEndDragBackItem(BackItem item, int id_)
    {
        base.GetEndDragBackItem(item, id_);
        if (id == id_)
        {
         //   MoveToGround();
            return;
        }

      //  Debug.Log(item.transform.position.y - wolfooAnim.transform.position.y);
        if (item.IsFood)
        {
            //if (item.transform.position.y - wolfooAnim.transform.position.y < 1.5f) return;
            //if (Mathf.Abs(item.transform.position.x - wolfooAnim.transform.position.x) > 1f) return;

            distance_ = Vector2.Distance(mouthZone.position, item.transform.position);
            if (distance_ <= 1f)
            {
                item.SetNotMoveToGround();
                item.DisableDrag();
                OnFeed(item);
            }
        }

        else if (item.IsBook)
        {
        }
        else
        {
            distance_ = Vector2.Distance(transform.position, item.transform.position);
            if (distance_ <= 3f && item.transform.position.y - wolfooAnim.transform.position.y > 1)
            {
                if (!item.IsCarryItem)
                {
                    wolfooAnim.PlayDisagree();
                    return;
                }

                if (curItem != null)
                {
                    curItem.transform.localScale = startScaleItem;
                    curItem.SetActive(true);
                }

                startScaleItem = item.transform.localScale;
                curItem = item.gameObject;
                item.SetNotMoveToGround();
                item.DisableDrag();
                OnVerified(item);
            }
        }
    }

    private void OnCarryBook(BackItem item)
    {
        starFx.transform.position = handItemImg.transform.position;
        starFx.Play();
        handItemImg.sprite = item.GetImage().sprite;
        handItemImg.SetNativeSize();
        handItemImg.transform.localScale = Vector3.one * 2;
        handItemImg.gameObject.SetActive(true);
        wolfooAnim.PlayCarryAnim();
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Wow);
    }

    private void OnFeed(BackItem item)
    {
        wolfooAnim.PlayEat();
        item.transform.DOMove(mouthZone.position, 0.5f);
        item.transform.DOScale(item.transform.localScale / 2, 0.5f).OnComplete(() =>
        {
            item.gameObject.SetActive(false);
        });
    }

    void OnVerified(BackItem item)
    {
        starFx.transform.position = handItemImg.transform.position;
        starFx.Play();
        item.gameObject.SetActive(false);
        handItemImg.sprite = item.GetComponent<Image>().sprite;
        handItemImg.SetNativeSize();
        handItemImg.transform.localScale = Vector3.one * 2.5f;
        handItemImg.gameObject.SetActive(true);
        wolfooAnim.PlayCarryAnim();
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Cool);
    }

    public void OnSitDownHorse(Transform _endTrans)
    {
        canMoveToGround = false;
        DisableDrag();

        wolfooAnim.PlaySit();
        transform.SetParent(_endTrans);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = new Vector3(-70f, -80f, 0);
    }
}
