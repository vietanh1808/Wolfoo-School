using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShapeItem : MonoBehaviour
{
    [SerializeField] Button lockBtn;
    private int id;
    Button button;

    public void AssignItem(int _id, Sprite sprite)
    {
        id = _id;
        button = GetComponent<Button>();
        button.image.sprite = sprite;
        button.image.SetNativeSize();

        button.onClick.AddListener(OnClickItem);
        OnAnim();
        lockBtn.onClick.AddListener(OnUnlockItem);

        EventManager.OnWatchAds += GetWatchAds;
    }
    private void OnUnlockItem()
    {
        EventManager.InitAdsPanelWithNoCoin?.Invoke(id, button.image.sprite);
        EventManager.OpenPanel?.Invoke(PanelType.Ads);
    }
    private void OnDestroy()
    {
        EventManager.OnWatchAds -= GetWatchAds;
    }
    private void GetWatchAds(int idx, PriceItem priceItem)
    {
        if (id != idx) return;

        DataMainManager.instance.LocalDataStorage.unlockShapeTopics[id] = true;
        DataMainManager.instance.SaveItem(DataMainManager.StorageKey.Data);
        Unlock();
    }
    public void Unlock()
    {
    //    canDrag = true;
        button.image.color = Color.white;
        button.image.DOFade(1, 0);
        lockBtn.gameObject.SetActive(false);
    }
    public void Lock()
    {
     //   canDrag = false;
        button.image.color = Color.black;
        button.image.DOFade(0.6f, 0);
        lockBtn.gameObject.SetActive(true);
    }

    private void OnAnim()
    {
        transform.DOPunchScale(new Vector3(-0.05f, 0.05f, 0), 1f, 1).SetLoops(-1);
    }

    private void OnClickItem()
    {
        EventManager.OnClickEmptyShape?.Invoke(id, this);
    }
}
