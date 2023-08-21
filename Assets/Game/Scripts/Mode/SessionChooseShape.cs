using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SCN.Common;
using System;

public class SessionChooseShape : Panel
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] ShapeItem shapeItemPb;

    ShapeModeDataSO data;
    int count = 0;

    private void Start()
    {
            FirebaseManager.instance.LogBeginMode(gameObject.name);
        EventManager.OnClickEmptyShape += GetClickItem;

        GameManager.instance.GetDataSO(DataSOType.Shape);
        data = GameManager.instance.ShapeDataSO;

        for (int i = 0; i < data.emptyBlockSprites.Count; i++)
        {
            var item = Instantiate(shapeItemPb, scrollRect.content);
            item.AssignItem(i, data.emptyBlockSprites[i]);
            item.gameObject.SetActive(false);
            if (DataMainManager.instance.LocalDataStorage.unlockShapeTopics[i])
            {
                item.Unlock();
            }
            else
            {
                item.Lock();
            }
        }
        GenderData();
    }
    private void OnDisable()
    {
        EventManager.OnClickEmptyShape -= GetClickItem;
    }

    private void GetClickItem(int idx, ShapeItem item)
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        GameManager.instance.curShapeIdx = idx;
        gameObject.SetActive(false);
        EventManager.OnEndSession?.Invoke();
    }

    private void GenderData()
    {
        if (count >= data.emptyBlockSprites.Count)
        {
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Wow);
            return;
        }

        SoundManager.instance.PlayOtherSfx(SfxOtherType.Popup);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            scrollRect.content.GetChild(count).gameObject.SetActive(true);
            count++;
            GenderData();
        });
    }
}
