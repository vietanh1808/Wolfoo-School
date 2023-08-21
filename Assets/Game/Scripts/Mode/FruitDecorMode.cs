using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using SCN.Tutorial;

public class FruitDecorMode : Panel
{
    [SerializeField] RectTransform scrollItemPb;
    [SerializeField] List<Button> typePickingBtns;
    [SerializeField] Image solesImg;
    [SerializeField] Image mainDecorImg;
    [SerializeField] ScrollRect trayItemPb;
    [SerializeField] Transform startTrayTrans;
    [SerializeField] Transform endTrayTrans;
    [SerializeField] Transform startPickBtnTrans;
    [SerializeField] Button confirmBtn;
    [SerializeField] Button backBtn;
    [SerializeField] ParticleSystem rainbowFx;

    ItemsDataSO data;
    List<ScrollRect> trayItems = new List<ScrollRect>();
    private int curTrayIdx = 0;
    bool isStart = true;
    List<Vector2> listEndPos = new List<Vector2>();
    private Tweener loopTween;
    private Tweener scaleTween;
    Vector3 startBtnScale;
    private Tweener scaleConfirmTween;
    private bool isBtnSpawn;
    private Vector3 startConfirmScale;

    private void Awake()
    {
        startBtnScale = typePickingBtns[0].transform.localScale;
        for (int i = 0; i < typePickingBtns.Count; i++)
        {
            listEndPos.Add(typePickingBtns[i].transform.position);
            typePickingBtns[i].transform.position = 
                new Vector3(typePickingBtns[i].transform.position.x, startPickBtnTrans.position.y, 0);
        }
    }

    private void Start()
    {
        FirebaseManager.instance.LogBeginMode(gameObject.name);
        EventManager.OnEndDrag += GetEndDragItem;
        EventManager.OnBeginDragDecor += GetBeginDragDecor;

        confirmBtn.onClick.AddListener(OnEndgame);
        backBtn.onClick.AddListener(OnBack);

        for (int i = 0; i < typePickingBtns.Count; i++)
        {
            typePickingBtns[i].onClick.AddListener(OnClickButton);
        }
        GameManager.instance.GetDataSO(DataSOType.Items);
        data = GameManager.instance.ItemDataSO;
        InitData();
        OpenFruitDecor();
        TutorialManager.Instance.NoReactTime = 1;
        TutorialManager.Instance.StartPointer(endTrayTrans.position, mainDecorImg.transform.position);

        startConfirmScale = confirmBtn.transform.localScale;
        confirmBtn.transform.localScale = Vector3.zero;
    }

    private void OnBack()
    {
        EventManager.OnBackPanel?.Invoke(this, PanelType.Room2, true);
    }

    private void OnDestroy()
    {
        EventManager.OnEndDrag -= GetEndDragItem;
        EventManager.OnBeginDragDecor -= GetBeginDragDecor;
    }

    private void OnEndgame()
    {
        confirmBtn.interactable = false;

        //    SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
        EventManager.OnEndSession?.Invoke();

        var blackPanel = Instantiate(GUIManager.instance.BlackPanel, transform);
        blackPanel.gameObject.SetActive(true);
        blackPanel.transform.SetAsLastSibling();
        rainbowFx.transform.SetAsLastSibling();
        solesImg.transform.SetAsLastSibling();

        solesImg.transform.localPosition =
                new Vector3(solesImg.transform.localPosition.x,
                solesImg.transform.localPosition.y,
                0);
        mainDecorImg.transform.localPosition =
                new Vector3(mainDecorImg.transform.localPosition.x,
                mainDecorImg.transform.localPosition.y,
                0);

        for (int i = 0; i < mainDecorImg.transform.childCount; i++)
        {
            mainDecorImg.transform.GetChild(i).localPosition =
                new Vector3(mainDecorImg.transform.GetChild(i).localPosition.x,
                mainDecorImg.transform.GetChild(i).localPosition.y,
                0);
        }

        scaleConfirmTween?.Kill();
        rainbowFx.transform.position = mainDecorImg.transform.position;
        rainbowFx.Play();
        solesImg.enabled = false;

        solesImg.transform.DOJump(Vector3.down * 2, 1f, 1, 2).SetEase(Ease.Linear).OnComplete(() =>
        {
            EventManager.OnEndgame?.Invoke(gameObject, PanelType.Room2, true, solesImg.gameObject);
        });
    }

    private void GetBeginDragDecor(int idxTray, DecorDragItem item)
    {
        trayItems[curTrayIdx].gameObject.SetActive(false);
        trayItems[idxTray].gameObject.SetActive(true);
        curTrayIdx = idxTray;
        item.transform.SetAsLastSibling();
    }

    private void GetEndDragItem(int idx, DecorDragItem item, float distance)
    {
        // Gender Main Item
        if (curTrayIdx == 0)
        {
            if (distance >= 3)
            {
                item.OnIncorrect();
                return;
            }
            mainDecorImg.sprite = data.fruitSprites[idx];
            mainDecorImg.SetNativeSize();
            mainDecorImg.gameObject.SetActive(true);
            item.OnCorrect(mainDecorImg.transform, () =>
            {
                OnClickBottle(Const.EYES_DECOR);
                item.gameObject.SetActive(false);
                for (int i = 0; i < typePickingBtns.Count; i++)
                {
                    typePickingBtns[i].transform.DOMoveY(listEndPos[i].y, 8)
                    .SetSpeedBased(true)
                    .SetEase(Ease.OutBounce);
                }
            });

            if(!isBtnSpawn)
            {
                isBtnSpawn = true;
                confirmBtn.transform.DOScale(startConfirmScale, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    scaleConfirmTween = confirmBtn.transform.DOPunchScale(Vector3.one * 0.1f, 1, 2).SetLoops(-1);
                });
            }
        }
        else
        {
            if (distance < 3.5f)
            {
                item.OnCorrect(mainDecorImg.transform);
            }
            else
            {
                item.OnIncorrect();
            }
        }
    }

    void InitData()
    {
        for (int i = 0; i < 4; i++)
        {
            var tray = Instantiate(trayItemPb, transform);
            // tray.transform.position = startTrayTrans.position;
            tray.gameObject.SetActive(false);
            trayItems.Add(tray);
            switch (i)
            {
                case Const.FRUIT_DECOR:
                    for (int j = 0; j < data.fruitSprites.Count; j++)
                    {
                        var scrollItem = Instantiate(scrollItemPb, tray.content);
                        var item = scrollItem.transform.GetChild(0).GetComponent<DecorDragItem>();
                        item.AssignItem(j, data.fruitSprites[j], tray.transform);
                        item.AssignItem(i, item.transform.localScale,
                            mainDecorImg.transform.position);
                        if (DataMainManager.instance.LocalDataStorage.unlockFruitDecorTopics[j])
                        {
                            item.Unlock();
                        }
                        else
                        {
                            item.Lock();
                        }
                    }
                    break;
                case Const.EYES_DECOR:
                    for (int j = 0; j < data.eyeSprites.Count; j++)
                    {
                        var scrollItem = Instantiate(scrollItemPb, tray.content);
                        var item = scrollItem.transform.GetChild(0).GetComponent<DecorDragItem>();
                        item.AssignItem(j, data.eyeSprites[j], tray.transform);
                        item.AssignItem(i, item.transform.localScale,
                            mainDecorImg.transform.position);
                        item.Unlock();

                        if (j < data.eyeAnimations.Count)
                            item.AssignItem(data.eyeAnimations[j]);
                    }
                    break;
                case Const.MOUTH_DECOR:
                    for (int j = 0; j < data.mouthSprites.Count; j++)
                    {
                        var scrollItem = Instantiate(scrollItemPb, tray.content);
                        var item = scrollItem.transform.GetChild(0).GetComponent<DecorDragItem>();
                        item.AssignItem(j, data.mouthSprites[j], tray.transform);
                        item.AssignItem(i, item.transform.localScale,
                            mainDecorImg.transform.position);
                        item.Unlock();
                    }
                    break;
                case Const.HAND_DECOR:
                    for (int j = 0; j < data.handSprites.Count; j++)
                    {
                        var scrollItem = Instantiate(scrollItemPb, tray.content);
                        var item = scrollItem.transform.GetChild(0).GetComponent<DecorDragItem>();
                        item.AssignItem(j, data.handSprites[j], tray.transform);
                        item.AssignItem(i, item.transform.localScale,
                            mainDecorImg.transform.position);
                        item.Unlock();
                        tray.content.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.LowerCenter;
                        // is Left Hand
                        if(j % 2 == 0)
                        {
                            item.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                            item.transform.localPosition += Vector3.left * 200;
                            item.transform.localPosition += Vector3.up * 200;
                        }
                        // is Right Hand
                        else
                        {
                            item.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                            item.transform.localPosition += Vector3.right * 200;
                            item.transform.localPosition += Vector3.up * 200;
                        }
                    }
                    break;
            }
        }
    }
    void OnClickButton()
    {
        var numberString = EventSystem.current.currentSelectedGameObject.name;
        var number = int.Parse(numberString);

        OnClickBottle(number);
    }
    void OpenFruitDecor()
    {
        trayItems[Const.FRUIT_DECOR].gameObject.SetActive(true);
        trayItems[Const.FRUIT_DECOR].transform.position = startTrayTrans.position;
        trayItems[Const.FRUIT_DECOR].transform.DOMoveY(endTrayTrans.position.y, 0.5f);
    }
    void OnClickBottle(int number)
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        if (number == curTrayIdx) return;

        trayItems[curTrayIdx].gameObject.SetActive(false);
        if(number != Const.FRUIT_DECOR && number >= 0)
        {
            if (loopTween != null)
            {
                loopTween.Kill();
            }
            if (scaleTween != null)
            {
                scaleTween.Kill();
            }

            if (curTrayIdx > 0)
                typePickingBtns[curTrayIdx - 1].transform.localScale = startBtnScale;

            scaleTween = typePickingBtns[number - 1].transform.DOPunchScale(Vector3.one * 0.1f, 0.5f, 2)
            .OnComplete(() =>
            {
                loopTween = typePickingBtns[number - 1].transform
                .DOScale(typePickingBtns[number - 1].transform.localScale + Vector3.one * 0.1f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo);
            });
        }

        switch (number)
        {
            case Const.EYES_DECOR:
                trayItems[Const.EYES_DECOR].gameObject.SetActive(true);
                if (isStart)
                {
                    TutorialManager.Instance.StartPointer(listEndPos[Const.HAND_DECOR-1]);
                    trayItems[Const.EYES_DECOR].transform.position = startTrayTrans.position;
                    trayItems[Const.EYES_DECOR].transform.DOMoveY(endTrayTrans.position.y, 0.5f);
                    isStart = false;
                }
                else
                {
                    TutorialManager.Instance.Stop();
                }
                break;
            case Const.HAND_DECOR:
                trayItems[Const.HAND_DECOR].gameObject.SetActive(true);
                break;
            case Const.MOUTH_DECOR:
                trayItems[Const.MOUTH_DECOR].gameObject.SetActive(true);
                break;
            default:
                break;
        }
        curTrayIdx = number;
    }

   
}
