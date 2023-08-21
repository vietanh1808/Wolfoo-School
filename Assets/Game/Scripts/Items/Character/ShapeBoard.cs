using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ShapeBoard : BackItem
{
    [SerializeField] List<Image> items;
    [SerializeField] Button playGameBtn;
    [SerializeField] GameObject modePb;
    [SerializeField] ParticleSystem rainbowFx;
    [SerializeField] Panel parentPanel;
    [SerializeField] PanelType nextMode;
    [SerializeField] ParticleSystem smokeFx;

    GameObject curMode;
    bool isHasGift;
    private Vector3 startBtnScale;
    private List<Sprite> data;
    private List<Sprite> initSpriteData = new List<Sprite>();

    protected override void Start()
    {
        base.Start();
        playGameBtn.onClick.AddListener(Playgame);
  //      EventManager.OnEndgame += GetNextShape;
        EventManager.OnUpdateShapeBoard += GetNextShape;

        startBtnScale = playGameBtn.transform.localScale;
        playGameBtn.transform.DOPunchScale(Vector3.one * 0.2f, 1f, 2).SetLoops(-1, LoopType.Restart);
        DOVirtual.DelayedCall(0.1f, () =>
        {
            InitData();
        });
    }
    private void OnEnable()
    {
        if(isHasGift)
        {
            isHasGift = false;
            Debug.Log("Get NExt Shape");
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Wow);

            int rd = UnityEngine.Random.Range(0, data.Count);
            if (DataMainManager.instance.LocalDataStorage.unlockShapesBoard.Contains(false))
            {
                if (data.Count == 0) return;
                while (DataMainManager.instance.LocalDataStorage.unlockShapesBoard[rd])
                {
                    rd = UnityEngine.Random.Range(0, data.Count);
                }
            }

            DataMainManager.instance.UpdateNextBoardShape(rd);
            items[rd].sprite = data[rd];
            items[rd].SetNativeSize();

            rainbowFx.transform.position = items[rd].transform.position;
            rainbowFx.Play();

            delayTween = DOVirtual.DelayedCall(2, () =>
            {
                if (!DataMainManager.instance.LocalDataStorage.unlockShapesBoard.Contains(false))
                {
                    smokeFx.Play();
                    delayTween = DOVirtual.DelayedCall(smokeFx.main.duration - 0.5f, () =>
                    {
                        EventManager.OnUpdateGift?.Invoke();
                    });

                    DataMainManager.instance.ResetBoardShape();
                    for (int i = 0; i < DataMainManager.instance.LocalDataStorage.unlockShapesBoard.Count; i++)
                    {
                        items[i].sprite = initSpriteData[i];
                        items[i].SetNativeSize();
                    }
                }
            });
        }
    }

    private void OnDestroy()
    {
        EventManager.OnUpdateShapeBoard -= GetNextShape;
   //     EventManager.OnEndgame -= GetNextShape;
    }

    private void InitData()
    {
        GameManager.instance.GetDataSO(DataSOType.Items);
        data = GameManager.instance.ItemDataSO.shapeBoards;

        for (int i = 0; i < items.Count; i++)
        {
            initSpriteData.Add(items[i].sprite);
            if (DataMainManager.instance.LocalDataStorage.unlockShapesBoard[i])
            {
                items[i].sprite = data[i];
                items[i].SetNativeSize();
            }
        }
    }
    private void Playgame()
    {
        EventManager.OnPlaygame?.Invoke(parentPanel, nextMode);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        OnPunchScale();
    }

    void GetNextShape(int coin, System.Action OnComplete)
    {
        isHasGift = true;
    }
}
