using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class PlayerPanel : Panel
{
    [SerializeField] Button changeBackItemBtn;
    [SerializeField] Button addCharacterBtn;
    [SerializeField] ScrollRect characterScrollView;
    [SerializeField] Text coinText;
    [SerializeField] ParticleSystem rainbowFx;
    [SerializeField] List<CharacterAnimation> characterAnimationPbs;
    [SerializeField] List<CharacterScrollItem> items;
    [SerializeField] List<Sprite> characterSprites;
    [SerializeField] List<int> prices;
    [SerializeField] List<Transform> moveScrollTrans;

    bool isUpdateCoin;
    int idxMoveScroll = 0;
    private Tweener moveScrollTween;
    private GameObject curBackContent;
    private Tweener moveBackContentTween;
    private Tweener moveContentTween;
    bool isOpenChooseCharacter;
    private float rangeMoveContent;
    private PanelType panelType;
    private bool isOpening;
    private Vector3 contentPos;
    bool isStart = true;
    private Tween delayTween;
    private int curCoinPlus;

    enum StateChooseCharacter
    {
        Close,
        Open,
    }

    public ScrollRect CharacterScrollView { get => characterScrollView; }
    private void Awake()
    {
    }

    protected override void Start()
    {
        EventManager.OnEndgame += GetEndGame;
        EventManager.OnInitCoin += GetInitCoin;
        EventManager.OnUpdateGift += GetUpdateGift;

        addCharacterBtn.onClick.AddListener(OnOpenCharacterPanel);
        changeBackItemBtn.onClick.AddListener(OnOpenPriceBackItem);

        rangeMoveContent = moveScrollTrans[1].position.y - moveScrollTrans[0].position.y;

        characterScrollView.transform.position = new Vector3(characterScrollView.transform.position.x, moveScrollTrans[idxMoveScroll].position.y, 0);
        characterScrollView.content.transform.position += Vector3.down * rangeMoveContent * 3;
        contentPos = characterScrollView.content.transform.position;

        DOVirtual.DelayedCall(0.3f, () =>
        {
            InitScroll();
            GameManager.instance.ChooseCharacterZone = moveScrollTrans[1];
            isStart = false;
        });

        PunchScale();
    }
    void PunchScale()
    {
        DOVirtual.DelayedCall(5, () =>
        {
            addCharacterBtn.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2)
            .OnComplete(() =>
            {
                PunchScale();
            });
        });
    }
    private void OnEnable()
    {
        EventManager.OnWatchAds += GetWatchAds;
        EventManager.OnEndDragBackItem += GetEndDragWolfoo;

        if (!isStart)
        {
            GenderData();
        }

        if (isUpdateCoin)
        {
            isUpdateCoin = false;
            rainbowFx.transform.position = coinText.transform.position;
            rainbowFx.Play();
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Wow);
        }
        else
        {
            if(isStart)
            {
                delayTween = DOVirtual.DelayedCall(1, () =>
                {
                    coinText.text = DataMainManager.instance.LocalDataStorage.totalCoin + "";
                });
            }
            else
            {
                if (delayTween != null) delayTween?.Kill();
                coinText.text = DataMainManager.instance.LocalDataStorage.totalCoin + "";
            }
        }
    }
    private void OnDisable()
    {
        EventManager.OnEndDragBackItem -= GetEndDragWolfoo;
        EventManager.OnWatchAds -= GetWatchAds;
    }

    private void OnDestroy()
    {
        EventManager.OnInitCoin -= GetInitCoin;
        EventManager.OnUpdateGift -= GetUpdateGift;
        EventManager.OnEndgame -= GetEndGame;

        if (delayTween != null) delayTween?.Kill();
    }

    private void GetUpdateGift()
    {
        Debug.Log("Action Update Coin");
        rainbowFx.transform.position = coinText.transform.position;
        rainbowFx.Play();
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Hooray);
        DataMainManager.instance.UpdateCoin(curCoinPlus, true,
        () =>
        {
            Debug.Log("Update Coin");
            isUpdateCoin = true;
        });
    }

    public void AssignPanel(PanelType panelType)
    {
        this.panelType = panelType;
    }

    private void GetEndDragWolfoo(BackItem item, int idx)
    {
        if (!isOpenChooseCharacter) return;
        if (!item.IsWolfoo) return;

        if (item.transform.position.y - moveScrollTrans[(int) StateChooseCharacter.Open].position.y <= 1)
        {
            item.DisableDrag();
            item.enabled = false;
            item.GetComponent<CharacterScrollItem>().SetToScrollView();
        }
    }
    void GetInitCoin(int coin)
    {
        coinText.text = coin + "";
    }
    private void GetWatchAds(int idx, PriceItem priceItem)
    {
        if(priceItem.skinType == SkinBackItemType.Character)
        {
            rainbowFx.transform.position = items[idx].transform.position;
            rainbowFx.Play();
        }
    }

    private void InitScroll()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].AssignItem(i, characterSprites[i], 
                new PriceItem() { skinType = SkinBackItemType.Character, price = prices[i] });
            if (DataMainManager.instance.LocalDataStorage.unlockCharacters[i])
            {
                items[i].Unlock();
            }
            else
            {
                items[i].Lock();
            }
        }
    }
    private void GenderData()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (DataMainManager.instance.LocalDataStorage.unlockCharacters[i])
            {
                items[i].Unlock();
            }
            else
            {
                items[i].Lock();
            }
        }
    }

    private void OnOpenSettingPanel()
    {
        EventManager.OpenPanel?.Invoke(PanelType.Setting);
    }

    private void OnOpenPriceBackItem()
    {

    }

    private void OnOpenCharacterPanel()
    {
        if (isOpening) return;
        isOpening = true;

        if (moveScrollTween != null) moveScrollTween?.Kill();
        if (moveContentTween != null)
        {
            moveContentTween?.Kill();
            characterScrollView.content.transform.position = contentPos;
        }
        if (moveBackContentTween != null) moveBackContentTween?.Kill();

        characterScrollView.transform.localPosition += new Vector3(0, 0, -characterScrollView.transform.localPosition.z);

        idxMoveScroll = 1 - idxMoveScroll;
        isOpenChooseCharacter = idxMoveScroll == 1;

        if(!isOpenChooseCharacter)
        {
            characterScrollView.content.transform.position += Vector3.down * rangeMoveContent * 3;
        }

        moveScrollTween = characterScrollView.transform
        .DOMoveY(moveScrollTrans[idxMoveScroll].position.y, 0.5f)
        .SetEase(Ease.Linear).OnComplete(() =>
        {
            if(isOpenChooseCharacter)
            {
                moveContentTween = characterScrollView.content.transform
                .DOMoveY(characterScrollView.content.position.y + rangeMoveContent * 3, 0.5f).OnComplete(() =>
                {
                    isOpening = false;
                })
                .SetEase(Ease.OutBack);
            }
        });

        curBackContent = GUIManager.instance.GetCurContent();
        moveBackContentTween = curBackContent.transform
        .DOMoveY(isOpenChooseCharacter ? curBackContent.transform.position.y + rangeMoveContent : curBackContent.transform.position.y - rangeMoveContent, 0.5f)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            isOpening = false;
        });

    }

    private void GetEndGame(GameObject arg1, PanelType panelType, bool arg2, GameObject arg3)
    {
        if (this.panelType != panelType) return;

        var coinPlus = 0;
        switch (GUIManager.instance.CurModeType)
        {
            case PanelType.NumberMode:
             //   isUpdateCoin = true;
                coinPlus = 8;
                curCoinPlus = coinPlus;

                Debug.Log("End Game");
                EventManager.OnUpdateShapeBoard?.Invoke(coinPlus, () =>
                {
                });
                return;
            case PanelType.ShapeMode:
             //   isUpdateCoin = true;
                coinPlus = 6;
                curCoinPlus = coinPlus;
                Debug.Log("End Game");
                EventManager.OnUpdateShapeGlocies?.Invoke(coinPlus, () =>
                {
                });

                return;
            case PanelType.GotoSchoolMode:
                coinPlus = 10;
                break;
            case PanelType.AlphaLearningMode:
               // coinPlus = 20;
                break;
            case PanelType.GalaxyMode:
                coinPlus = 10;
                break;
            case PanelType.FruitMode:
                coinPlus = 8;
                break;
        }

        DataMainManager.instance.UpdateCoin(coinPlus, true,
        () =>
        {
            isUpdateCoin = true;
        });
    }
}
