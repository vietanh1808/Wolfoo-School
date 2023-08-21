using DG.Tweening;
using SCN;
using SCN.Tutorial;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayMode : Panel
{
    [SerializeField] int curIdxSession;
    [SerializeField] Clay clay;
    [SerializeField] Peeler peeler;
    [SerializeField] ShapeBoardFlower shapeBoard;
    [SerializeField] Transform shapeItemZone;
    [SerializeField] Tray tray;
    [SerializeField] Pistil pistilTest;
    [SerializeField] LineKnife knife;
    [SerializeField] MagnifyingGlass magnifyingGlass;
    [SerializeField] ItemBoard petalBoard;
    [SerializeField] Transform flowerZone;
    [SerializeField] MagicStick magicStick;
    [SerializeField] ItemBoard trunkBoard;
    [SerializeField] ConfirmBtn confirmBtn;
    [SerializeField] ItemBoard leafBoard;
    [SerializeField] VerticalScroll potScroll;
    [SerializeField] Pot pot;
    [SerializeField] MagicFan fan;
    [SerializeField] ButtonBase backBtn; 

    private Tween delayTween2;
    private Tween delayTween;
    private List<ShapeFlowerItem> curShapeItems;
    private List<Transform> curStampedItems = new List<Transform>();
    private ClayModeDataSO data;
    private int curBoxIdx;
    private Pistil pistil;
    private FlowerData curFlowerItem;
    private Flower flower;
    private Tweener scaleItemTween;
    private int countLeafSuccess;
    private int countPetalSuccess;
    private Tweener rotateTween;
    private int countThrowing;

    private void Awake()
    {
        
    }
    private void Start()
    {
        InitEvent();

        GameManager.instance.GetDataSO(DataSOType.ClayMode);
        data = GameManager.instance.ClayDataSO;

        clay.MoveOut(Direction.Right, 0);
        peeler.MoveOut(Direction.Right, 0);
        shapeBoard.MoveOut(Direction.Up, 0);
        tray.MoveOut(Direction.Left, 0);
        magicStick.MoveOut(Direction.Left, 0);
        knife.MoveOut(Direction.Right, 0);
        petalBoard.MoveOut(Direction.Right, 0);
        trunkBoard.MoveOut(Direction.Right, 0);
        leafBoard.MoveOut(Direction.Right, 0);
        potScroll.MoveOut(Direction.Right, 0);
        fan.MoveOut(Direction.Right, 0);

        delayTween = DOVirtual.DelayedCall(0.5f, () =>
        {
            OnNextSession(); // Begin Session 0
        });
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener<EventKey.OnBeginDragItem>(GetBeginDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnDragItem>(GetDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnEndDragItem>(GetEndDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnClickItem>(GetClickItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnCompleteAll>(GetCompleteAll);

        backBtn.onClick.RemoveListener(OnBack);

        if (delayTween != null) delayTween?.Kill();
        if (delayTween2 != null) delayTween2?.Kill();
        if (scaleItemTween != null) scaleItemTween?.Kill();
        if (rotateTween != null) rotateTween?.Kill();

        TutorialManager.Instance.Stop();
    }

    private void InitEvent()
    {
        EventDispatcher.Instance.RegisterListener<EventKey.OnBeginDragItem>(GetBeginDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnDragItem>(GetDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnEndDragItem>(GetEndDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnClickItem>(GetClickItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnCompleteAll>(GetCompleteAll);

        backBtn.onClick.AddListener(OnBack);
    }

    private void OnBack()
    {
        EventManager.OnBackPanel?.Invoke(this, PanelType.Room3, true);
    }

    private void GetCompleteAll(EventKey.OnCompleteAll item)
    {
        if(item.pistil != null)
        {
            GUIManager.instance.PlayLighting(magnifyingGlass.transform);
            knife.MoveOut(Direction.Right, 0.5f, () =>
            {
                magnifyingGlass.OnClose(() =>
                {
                    OnNextSession(); // Begin Session 3
                }, null);
            });
        }
    }

    private void GetClickItem(EventKey.OnClickItem item)
    {
        if(item.flowerBox != null)
        {
            OnClickFlowerBox(item);
        }
        if(item.flowerItem != null)
        {
            OnClickFlowerItem(item);
        }
        if(item.confirmBtn != null)
        {
            TutorialManager.Instance.Stop();
            OnClickBtn(item);
        }
        if(item.fan != null)
        {
            OnClickFan(item);
        }
        if(item.gui != null)
        {
            EventDispatcher.Instance.Dispatch(new EventKey.OnModeComplete { flowerPot = pot });
            delayTween = DOVirtual.DelayedCall(0.25f, () =>
            {
                EventManager.OnBackPanel.Invoke(this, PanelType.Room3, true);
            });
        }
    }

    private void OnClickFan(EventKey.OnClickItem item)
    {
        fan.OnThrowing(flower.transform.position, () =>
        {
            countThrowing++;
            if (countThrowing >= 3) return;

            var cloneFlower = Instantiate(flower, pot.FlowerCloneZone);
            transform.localPosition = Vector2.zero;
            cloneFlower.transform.SetSiblingIndex(0);
            cloneFlower.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            scaleItemTween = cloneFlower.transform.DOScale(cloneFlower.transform.localScale - Vector3.one * 0.1f, 0.3f);
            rotateTween = cloneFlower.transform.DORotate(Vector3.forward * (countThrowing == 1 ? 30 : -30), 0.5f)
            .SetEase(Ease.OutBack, 3)
            .OnComplete(() =>
            {
                if (countThrowing == 2)
                {
                    fan.MoveOut(Direction.Right);
                    GUIManager.instance.PlayLighting(flower.transform);
                    delayTween = DOVirtual.DelayedCall(1, () =>
                    {
                        OnNextSession(); // Begin Session 9
                    });
                }
                else
                {
                    fan.AssignClick();
                }
            });
        });
    }

    private void OnClickBtn(EventKey.OnClickItem item)
    {
        if(curIdxSession - 1 == 5)
        {
            trunkBoard.MoveOut(Direction.Right, 0.5f, () =>
            {
                OnNextSession(); // Begin Session 6
            });
        }
        if(curIdxSession - 1 == 7)
        {
            potScroll.MoveOut(Direction.Right, 0.5f, () =>
            {
                GUIManager.instance.PlayLighting(flower.transform);
                delayTween = DOVirtual.DelayedCall(1, () =>
                {
                    OnNextSession(); // Begin Session 8
                });
            });
        }
    }

    private void OnClickFlowerBox(EventKey.OnClickItem item)
    {
        curBoxIdx = item.id;

        TutorialManager.Instance.Stop();

        shapeBoard.MoveOut(Direction.Up, 0.5f, () =>
        {
            GUIManager.instance.whitPanel.gameObject.SetActive(false);
        });
        item.flowerBox.transform.SetParent(transform);
        item.flowerBox.AssignEndPos(UISetupManager.Instance.endMoveShapeFlowerZone.position, transform);
        item.flowerBox.MoveToEndPos(() =>
        {
            curShapeItems = item.flowerBox.OnGetItem(() =>
            {
                for (int i = 0; i < curShapeItems.Count; i++)
                {
                    int idx = i;
                    curShapeItems[idx].JumpOutBox(
                        UISetupManager.Instance.endMoveShapeItemZones[idx].position,
                        shapeItemZone);
                }

                item.flowerBox.OnCloseLid(() =>
                {
                    item.flowerBox.MoveOut(Direction.Left, 0.5f, () =>
                    {
                        foreach (var shape in curShapeItems)
                        {
                            shape.AssignClick();
                            shape.InitStartInfo();
                        }
                        TutorialManager.Instance.StartPointer(curShapeItems[2].transform.position);
                    });
                });
            });
        });
    }

    private void OnClickFlowerItem(EventKey.OnClickItem item)
    {
        TutorialManager.Instance.Stop();

        GUIManager.instance.PLayStar(item.flowerItem.transform);
        // On Begin Stamp
        var stampedClay = Instantiate(data.stampedClayPbs[curBoxIdx], clay.StampedZone);
        stampedClay.gameObject.SetActive(false);
        for (int i = 0; i < curShapeItems.Count; i++)
        {
            int idx = i;
            curShapeItems[idx].JumpToEndPos(stampedClay.transform.GetChild(idx).position,
                () =>
                {

                }, 5);
        }

        // On Stamping
        delayTween = DOVirtual.DelayedCall(1.5f, () =>
        {
            // On Stamped
        stampedClay.gameObject.SetActive(true);
            foreach (var shape in curShapeItems)
            {
                shape.MoveToEndPos(shape.transform.position + Vector3.up * 2, () =>
                {
                    shape.MoveOut(Direction.Left);
                });
            }

            delayTween2 = DOVirtual.DelayedCall(0.5f, () =>
            {
                clay.ChangeState(2);
            });

            // On Shape Item Move Out
            delayTween = DOVirtual.DelayedCall(1.5f, () =>
            {
                var length = stampedClay.transform.childCount;
                for (int i = 0; i < length; i++)
                {
                    var stampedItem = stampedClay.transform.GetChild(0);
                    stampedItem.SetParent(transform);
                    curStampedItems.Add(stampedItem);
                }

                // On StampedClay Move Out
                delayTween = DOVirtual.DelayedCall(1, () =>
                {
                    stampedClay.MoveOut(Direction.Down, 1, () =>
                    {
                        tray.MoveInBack(0.5f, () =>
                        {
                            foreach (var stampedItem in curStampedItems)
                            {
                                if (stampedItem.GetComponent<Pistil>() == null)
                                {
                                    stampedItem.SetParent(tray.ItemZone);
                                    var rdPos = new Vector3(
                                        UnityEngine.Random.Range(-2, 2),
                                        UnityEngine.Random.Range(-1, 1),
                                        0);
                                    stampedItem.DOJump(tray.ItemZone.position + rdPos, 6, 1, 0.5f)
                                    .SetDelay(0.2f)
                                    .OnComplete(() =>
                                    {
                                        tray.Dance(0.1f, () =>
                                        {
                                            tray.MoveOut(Direction.Left, 0.5f, () =>
                                            {
                                                pistil.AssignEndPos(UISetupManager.Instance.center.position, transform);
                                                pistil.MoveToEndPos(() =>
                                                {
                                                    OnNextSession(); // Begin Session 2
                                                });
                                            });
                                        });
                                    });
                                }
                                else
                                {
                                    pistil = stampedItem.GetComponent<Pistil>();
                                }
                            }
                        });
                    });
                });
            });
        });
    }

    private void GetBeginDragItem(EventKey.OnBeginDragItem item)
    {
        if (item.trunk != null)
        {
        //    trunkBoard.ShowNextItem(item.trunk.transform.parent.GetSiblingIndex());
        }
    }
    private void GetDragItem(EventKey.OnDragItem item)
    {
        if (item.peeler != null)
        {
            item.peeler.OnCompare(() =>
            {
                clay.OnChange(1, () =>
                {
                    GUIManager.instance.PlayLighting(clay.transform);
                    delayTween = DOVirtual.DelayedCall(1, () =>
                    {
                        OnNextSession(); // Begin Session 1
                    });
                }, null);
            });
        }
    }
    private void GetEndDragItem(EventKey.OnEndDragItem item)
    {
        if (item.petal != null)
        {
            item.petal.OnCompare(() =>
            {
                item.petal.InitStartScale(flower.PetalItems[item.petal.Id].transform.localScale);
                item.petal.MoveToEndPos2(() =>
                {
                    item.petal.transform.SetParent(flower.PetalZone);
                    item.petal.transform.localScale = flower.PetalItems[item.petal.Id].transform.localScale;
                    item.petal.transform.SetSiblingIndex(item.petal.Id);
                    GUIManager.instance.PLayStar2(item.petal.transform);

                    countPetalSuccess++;
                    if(countPetalSuccess == flower.PetalItems.Length)
                    {
                        TutorialManager.Instance.Stop();
                        petalBoard.MoveOut(Direction.Right, 0.5f);
                        GUIManager.instance.PlayLighting(flower.transform);
                        delayTween = DOVirtual.DelayedCall(1, () =>
                        {
                            OnNextSession(); // BeginSession 4
                        });
                    }
                });
            },
            () =>
            {
                item.petal.JumpToStartPos(() =>
                {
                    item.petal.AssignItem(
                        flower.PetalItems[item.petal.Id].transform,
                        flower.PetalZone,
                        3,
                        item.petal.transform);
                });
            });
        }
        if (item.trunk != null)
        {
            item.trunk.OnCompare(() =>
            {
                trunkBoard.ShowNextItem(item.trunk.transform.parent.GetSiblingIndex());
                if (!confirmBtn.IsShowed) confirmBtn.Show();
                flower.Trunk.gameObject.SetActive(false);
                item.trunk.OnPlugin(() =>
                {
                    flower.Trunk.gameObject.SetActive(true);
                    item.trunk.OnHide();
                    flower.AssignTrunk2(data.flowerItems[curBoxIdx].trunkSprites[item.trunk.Id]);
                });
            },
            () =>
            {
                trunkBoard.ShowNextItem(item.trunk.transform.parent.GetSiblingIndex());
                item.trunk.OnMoveDown(null);
            });
        }
        if (item.leaf != null)
        {
            item.leaf.OnCompare(() =>
            {
                item.leaf.MoveToEndPos2(() =>
                {
                    item.leaf.transform.SetParent(flower.LeafZone);
                    item.leaf.transform.localScale = flower.LeafItems[item.leaf.Id].transform.localScale;
                    item.leaf.transform.SetSiblingIndex(item.leaf.Id);
                    GUIManager.instance.PLayStar2(item.leaf.transform);

                    countLeafSuccess++;
                    if (countLeafSuccess == flower.LeafItems.Length)
                    {
                        TutorialManager.Instance.Stop();
                        leafBoard.MoveOut(Direction.Right, 0.5f);
                        GUIManager.instance.PlayLighting(flower.transform);
                        delayTween = DOVirtual.DelayedCall(1, () =>
                        {
                            OnNextSession(); // BeginSession 7
                        });
                    }
                });
            },
            () =>
            {
                item.leaf.JumpToStartPos(() =>
                {
                    leafBoard.CurLeafItems[item.leaf.Id].AssignItem(
                        flower.LeafItems[item.leaf.Id].transform,
                        flower.LeafZone,
                        3,
                        leafBoard.CurLeafItems[item.leaf.Id].transform);
                });
            });
        }
        if (item.potItem != null)
        {
            item.potItem.OnCompare(() =>
            {
                TutorialManager.Instance.Stop();
                flower.StopDancing();
                pot.OnChangeSkin(
                    data.potSprites[item.potItem.Order],
                    data.potMaskSprites[item.potItem.Order],
                    data.localFlowerPos[item.potItem.Order],
                    data.localMaskPos[item.potItem.Order]);

                confirmBtn.Show();
            });
        }
    }

    private void OnNextSession()
    {
        switch (curIdxSession)
        {
            case 0:
                clay.MoveInBack(0);
                peeler.MoveInBack(0.5f, () =>
                {
                    peeler.AssignItem(clay.transform, transform, 5, peeler.CompareTrans);
                    peeler.ScaleTut(true);
                });
                break;
            case 1:
                clay.MoveInBack(0);
                clay.ChangeState(1);
                peeler.MoveOut(Direction.Right, 0.5f, () =>
                {
                    peeler.StopScaleTut();
                    shapeBoard.MoveInBack(0.5f, () =>
                    {
                        TutorialManager.Instance.StartPointer(shapeBoard.transform.position);
                    });
                });
                break;
            case 2:
                // Next Session Cut Pistil
                if (pistil != null)
                {
                    pistilTest.AssignSprite(pistil.ItemImg.sprite);
                    pistil.gameObject.SetActive(false);
                }
                knife.OnChangeDirection(LineKnife.Direction.Horizontal);

                magnifyingGlass.OnOpen(() =>
                {
                    knife.MoveInBack(0.5f, () =>
                    {
                        pistilTest.AssignLine(knife, null);
                    });
                });
                break;
            case 3:
                if (pistil != null) pistil.gameObject.SetActive(false);
                curFlowerItem = data.flowerItems[curBoxIdx];
                flower = Instantiate(curFlowerItem.flowerPb, flowerZone);
                flower.transform.position = pistil != null ? pistil.transform.position : pistilTest.transform.position;
                flower.AssignEndPos(UISetupManager.Instance.addPetalZone.position, transform);
                flower.AssignPetal(curFlowerItem.petalSprites);
                flower.AssignTrunk(curFlowerItem.trunkSprites[0]);
                flower.AssignLeaf(curFlowerItem.leafSprites);
                petalBoard.AssignPetalBoard(curFlowerItem.petalSprites);

                delayTween = DOVirtual.DelayedCall(1, () =>
                {
                    flower.MoveToEndPos();
                    petalBoard.MoveInBack(0.5f, () =>
                    {
                        for (int i = 0; i < curFlowerItem.petalSprites.Length; i++)
                        {
                            petalBoard.CurItems[i].InitStartInfo();
                            //  petalBoard.CurItems[i].AssignItem(i);
                            petalBoard.CurItems[i].AssignItem(
                                flower.PetalItems[i].transform,
                                flower.PetalZone,
                                3,
                                petalBoard.CurItems[i].transform);
                        }

                        TutorialManager.Instance.StartPointer(petalBoard.transform.position, pistil.transform.position);
                    });
                });
                break;
            case 4:
                if (pistil != null) pistil.gameObject.SetActive(false);

                curFlowerItem = data.flowerItems[curBoxIdx];
                if(flower == null)
                {
                    flower = Instantiate(curFlowerItem.flowerPb, flowerZone);
                    flower.transform.position = pistil != null ? pistil.transform.position : pistilTest.transform.position;
                    petalBoard.AssignPetalBoard(curFlowerItem.petalSprites);
                    flower.AssignTrunk(curFlowerItem.trunkSprites[0]);
                    flower.AssignLeaf(curFlowerItem.leafSprites);
                }

                flower.AssignPetal(petalBoard.CurItems);
                flower.transform.localScale += Vector3.one * 0.5f;
                flower.AssignEndPos(UISetupManager.Instance.center.position, transform);
                flower.MoveToEndPos(() =>
                {
                    magicStick.OnMoveToFlower(() =>
                    {
                        flower.DrawColor(curFlowerItem.colorPistilSprite, curFlowerItem.colorPetalSprites);
                    }, 
                    () =>
                    {
                        scaleItemTween = flower.transform.DOScale(flower.transform.localScale - Vector3.one * 0.5f, 0.5f);
                        OnNextSession(); // Begin Session 5
                    });
                }, false);
                break;
            case 5:
                curFlowerItem = data.flowerItems[curBoxIdx];
                if (flower == null)
                {
                    flower = Instantiate(curFlowerItem.flowerPb, flowerZone);
                    flower.transform.position = pistil != null ? pistil.transform.position : pistilTest.transform.position;
                    flower.AssignTrunk(curFlowerItem.trunkSprites[0]);
                    flower.AssignLeaf(curFlowerItem.leafSprites);
                }
                flower.AssignEndPos(UISetupManager.Instance.addTrunkZone.position, transform);
                flower.MoveToEndPos(() =>
                {
                    trunkBoard.AssignTrunkBoard(curFlowerItem.trunkSprites);
                    trunkBoard.MoveInBack(0.5f, () =>
                    {
                        foreach (var trunk in trunkBoard.CurTrunks)
                        {
                            trunk.AssignItem(flower.TrunkZone, flower.TrunkZone, 3, trunk.transform);
                        }
                        TutorialManager.Instance.StartPointer(trunkBoard.transform.position, pistil.transform.position);
                    });
                }, false);
                break;
            case 6:
                curFlowerItem = data.flowerItems[curBoxIdx];
                if (flower == null)
                {
                    flower = Instantiate(curFlowerItem.flowerPb, flowerZone);
                    flower.transform.position = pistil != null ? pistil.transform.position : pistilTest.transform.position;
                    flower.AssignLeaf(curFlowerItem.leafSprites);
                }
                flower.AssignEndPos(UISetupManager.Instance.addTrunkZone.position, transform);
                flower.MoveToEndPos(() =>
                {
                    leafBoard.AssignLeafBoard(curFlowerItem.leafSprites);
                    leafBoard.MoveInBack(0.5f, () =>
                    {
                        for (int i = 0; i < curFlowerItem.leafSprites.Length; i++)
                        {
                            leafBoard.CurLeafItems[i].InitStartInfo();
                            leafBoard.CurLeafItems[i].AssignItem(
                                flower.LeafItems[i].transform, 
                                flower.LeafZone, 
                                3,
                                leafBoard.CurLeafItems[i].transform);
                        }
                        TutorialManager.Instance.StartPointer(leafBoard.transform.position, flower.LeafZone.position);
                    });
                }, false);
                break;
            case 7:
                curFlowerItem = data.flowerItems[curBoxIdx];
                potScroll.Setup(data.potSprites.Length, this);
                if (flower == null)
                {
                    flower = Instantiate(curFlowerItem.flowerPb, flowerZone);
                    flower.transform.position = pistil != null ? pistil.transform.position : pistilTest.transform.position;
                }
                scaleItemTween = flower.transform.DOScale(Vector3.one * 0.5f, 0.5f);
                flower.AssignEndPos(pot.FlowerZone.position, pot.FlowerZone);
                flower.MoveInBack(0.5f, () =>
                {
                    flower.transform.SetParent(pot.FlowerZone);
                    flower.transform.localPosition = Vector3.zero;
                    flower.InitLocalPos();
                    flower.OnDancing();
                });

                delayTween = DOVirtual.DelayedCall(0.5f, () =>
                {
                    foreach (var item in potScroll.MaskTrans.GetComponentsInChildren<PotItem>())
                    {
                        item.AssignItem(pot.transform, item.transform, 3);
                    }
                    potScroll.MoveInBack(2, () =>
                    {
                        TutorialManager.Instance.StartPointer(potScroll.transform.position, pot.transform.position);
                    });
                });
                break;
            case 8:
                curFlowerItem = data.flowerItems[curBoxIdx];
                if (flower == null)
                {
                    pot.OnChangeSkin(data.potSprites[0], data.potMaskSprites[0], data.localFlowerPos[0], data.localMaskPos[0]);
                    flower = Instantiate(curFlowerItem.flowerPb, flowerZone);
                    flower.transform.position = pistil != null ? pistil.transform.position : pistilTest.transform.position;
                    flower.transform.localScale = Vector3.one * 0.5f;
                    flower.transform.SetParent(pot.FlowerZone);
                    flower.transform.localPosition = Vector3.zero;
                }

                fan.MoveInBack(0.5f, () =>
                {
                    fan.AssignClick();
                });
                break;
            case 9:
                GUIManager.instance.OnCongratulate(pot.transform, UISetupManager.Instance.potLastZone.position);
                break;
        }
        curIdxSession++;
    }
}
