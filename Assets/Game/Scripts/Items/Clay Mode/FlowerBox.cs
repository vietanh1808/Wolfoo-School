using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowerBox : ItemMove
{
    [SerializeField] Transform itemZone;
    [SerializeField] Lid lid;
    [SerializeField] Button button;
    [SerializeField] Transform openLidZone;
    [SerializeField] ShapeFlowerItem shapeFlowerPb;

    private List<ShapeFlowerItem> shapeItems = new List<ShapeFlowerItem>();
    private Tweener rotateItemTween;
    private Tweener moveItemTween;
    private Tween delayItemTween;
    private Tweener scaleItemTween;

    private void Start()
    {
        if (button != null) button.onClick.AddListener(GetClickItem);
    }
    private void OnDestroy()
    {
        if (rotateItemTween != null) rotateItemTween?.Kill();
        if (moveItemTween != null) moveItemTween?.Kill();
        if (delayItemTween != null) delayItemTween?.Kill();
        if (scaleItemTween != null) scaleItemTween?.Kill();
    }

    private void GetClickItem()
    {
        button.interactable = false;
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        GUIManager.instance.whitPanel.gameObject.SetActive(true);
        rotateItemTween = transform.DOPunchRotation(Vector3.forward * 10, 1, 6);
        scaleItemTween = transform.DOPunchScale(Vector3.one * 0.1f, 1)
            .OnComplete(() =>
            {
                EventDispatcher.Instance.Dispatch(new EventKey.OnClickItem { flowerBox = this, id = id });
            });
    }

    public void AssignItem(Sprite traySprite, Sprite lidSprite, Sprite[] itemSprites)
    {
        itemImg.sprite = traySprite;
        itemImg.SetNativeSize();
        lid.AssignItem(lidSprite);

        for (int i = 0; i < itemSprites.Length; i++)
        {
            var shapeItem = Instantiate(shapeFlowerPb, itemZone.GetChild(i));
            shapeItem.AssignItem(i, itemSprites[i]);
            shapeItem.transform.localPosition = Vector3.zero;
            shapeItems.Add(shapeItem);
        }
    }

    public void OnCloseLid(System.Action OnComplete = null)
    {
        var time = 1;
        rotateItemTween = lid.transform.DORotate(Vector3.zero, time);
        moveItemTween = lid.transform.DOLocalMove(Vector3.up * 90, time)
            .SetEase(Ease.Flash)
            .OnComplete(() =>
            {
                OnComplete?.Invoke();
            });
    }

    public List<ShapeFlowerItem> OnGetItem(System.Action OnComplete = null)
    {
        var time = 1;
        rotateItemTween = lid.transform.DORotate(Vector3.forward * 25, time);
        moveItemTween = lid.transform.DOMove(openLidZone.position, time)
            .SetEase(Ease.Flash)
            .OnComplete(() =>
            {
                OnComplete?.Invoke();
            });

        return shapeItems;
    }


}
