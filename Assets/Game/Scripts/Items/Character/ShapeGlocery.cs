using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ShapeGlocery : MonoBehaviour
{
    [SerializeField] List<BackShapeItem> items;
    [SerializeField] Button playgameBtn;
    [SerializeField] ParticleSystem rainbowFx;
    [SerializeField] GameObject modePb;
    [SerializeField] Panel parentPanel;
    [SerializeField] PanelType nextMode;
    [SerializeField] ParticleSystem smokeFx;

    private ShapeModeDataSO data;
    private Tweener scaleTween;
    private Vector3 btnStartScale;
    bool isHasGift;
    private int colorIdx;
    private int shapeIdx;
    GameObject curMode;
    private Tween delayTween;

    private void Start()
    {
        EventManager.OnCompleteCutBox += GetCompleteShapeCut;
        EventManager.OnUpdateShapeGlocies += GetNextShape;
        playgameBtn.onClick.AddListener(OnPlaygame);

        btnStartScale = playgameBtn.transform.localScale;
        GameManager.instance.GetDataSO(DataSOType.Shape);
        data = GameManager.instance.ShapeDataSO;

        playgameBtn.transform.DOPunchScale(Vector3.one * 0.2f, 1f, 2).SetLoops(-1, LoopType.Restart);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            InitData();
        });
    }

    private void OnDestroy()
    {
        EventManager.OnUpdateShapeGlocies -= GetNextShape;
        EventManager.OnCompleteCutBox -= GetCompleteShapeCut;

        if (delayTween != null) delayTween?.Kill();
    }
    private void OnEnable()
    {
        EventManager.OnClickBackItem += GetClickItem;
        if (isHasGift)
        {
            isHasGift = false;
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Wow);

            rainbowFx.Play();

            delayTween = DOVirtual.DelayedCall(2, () =>
            {
                foreach (var item in DataMainManager.instance.LocalDataStorage.shapeGlocerys)
                {
                    if (!item.unlockShapes) return;
                }

                smokeFx.Play();
                delayTween = DOVirtual.DelayedCall(smokeFx.main.duration - 0.5f, () =>
                {
                    EventManager.OnUpdateGift?.Invoke();
                });

                DataMainManager.instance.ResetShapeGloceris();
                for (int i = 0; i < DataMainManager.instance.LocalDataStorage.shapeGlocerys.Count; i++)
                {
                    items[i].AssignItem(data.emptyBlockSprites[i]);
                }
            });
        }
    }
    private void OnDisable()
    {
        EventManager.OnClickBackItem -= GetClickItem;
    }
    void GetNextShape(int coin, System.Action OnComplete)
    {
    //   isHasGift = true;
    }
    void InitData()
    {
        for (int i = 0; i < items.Count; i++)
        {
            var local = DataMainManager.instance.LocalDataStorage.shapeGlocerys[i];
            if (local.unlockShapes)
            {
                items[i].AssignItem(data.shapeColors[(int)local.idx.x].blockSprites[(int)local.idx.y]);
            }
            else
            {
                items[i].AssignItem(data.emptyBlockSprites[i]);
            }
        }
    }

    private void OnPlaygame()
    {
        EventManager.OnPlaygame?.Invoke(parentPanel, nextMode);
    }

    void GetCompleteShapeCut()
    {
        isHasGift = true;
        colorIdx = GameManager.instance.curIdxColor;
        shapeIdx = GameManager.instance.curShapeIdx;

        GameManager.instance.GetDataSO(DataSOType.Shape);
        data = GameManager.instance.ShapeDataSO;
        items[shapeIdx].AssignItem(data.shapeColors[shapeIdx].blockSprites[colorIdx]);
        rainbowFx.transform.position = items[shapeIdx].transform.position;
    }

    private void GetClickItem(int id, BackItem item)
    {
        if (!item.IsShapeGlocery) return;

        if(scaleTween != null)
        {
            playgameBtn.transform.localScale = btnStartScale;
            scaleTween?.Kill();
        }
        scaleTween = playgameBtn.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2);
    }
}
