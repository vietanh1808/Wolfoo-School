using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN.Common;
using System;
using UnityEngine.UI;
using DG.Tweening;
using SCN.Tutorial;

public class SessionPractise : Panel
{
    [SerializeField] List<ResultItem> resultItems;
    [SerializeField] List<CacullationItem> caculationItems;
    [SerializeField] Button confirmButton;
    [SerializeField] List<Color> colors;
    [SerializeField] Transform spawnResultTrans;
    [SerializeField] ParticleSystem starFx;

    int range;
    private CaculateType caculateType;
    int count = 0;

    List<int> results = new List<int>();
    List<int> firstInts = new List<int>();
    List<int> secondInts = new List<int>();
    List<bool> curEmptyIdxs = new List<bool>();
    List<Vector3> endResultPos = new List<Vector3>();
    List<ResultItem> curResultItemsUsed = new List<ResultItem>();
    private Tweener punchScale;
    private Vector3 startScale;
    private int countCorrect;
    private Tween delayTween;

    private void Awake()
    {
            FirebaseManager.instance.LogBeginMode(gameObject.name);
        startScale = confirmButton.transform.localScale;
        confirmButton.transform.localScale = Vector3.zero;
        foreach (var item in resultItems)
        {
            endResultPos.Add(item.transform.position);
            item.transform.position = new Vector3(item.transform.position.x, spawnResultTrans.position.y, 0);
        }
    }
    void Start()
    {
        EventManager.OnEndDragResult += GetEndDragResult;
        EventManager.OnResultMoveOut += GetResultMoveOut;
        EventManager.OnCorrect += GetCorrectResult;
        confirmButton.onClick.AddListener(OnCheckResult);

        InitData();
        delayTween = DOVirtual.DelayedCall(2, () =>
        {
            confirmButton.transform.DOScale(startScale, 0.1f).OnComplete(() =>
            {
                confirmButton.transform.DOPunchScale(new Vector3(-0.3f, 0.3f, 0), 1f, 2);
            });
            starFx.Play();

            GetTut();
        });
    }

    private void OnDestroy()
    {
        EventManager.OnEndDragResult -= GetEndDragResult;
        EventManager.OnResultMoveOut -= GetResultMoveOut;
        EventManager.OnCorrect -= GetCorrectResult;
        TutorialManager.Instance.Stop();
    }

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        firstInts.Clear();
        secondInts.Clear();
        results.Clear();
        count = 0;
    }

    void GetTut()
    {
        //    TutorialManager.Instance.NoReactTime = 2;
        //    TutorialManager.Instance.MoveTime = 0;

   //     TutorialPanel.instance.Speed = 50;
        for (int i = 0; i < curEmptyIdxs.Count; i++)
        {
            if (curEmptyIdxs[i])
            {
                TutorialPanel.instance.StartPointer(GetResultTrans().position,
                    caculationItems[i].ResultText.transform.position);
              //  TutorialManager.Instance.StartPointer(GetResultTrans().position, caculationItems[i].ResultText.transform.position, true);
                return;
            }
        }
        TutorialPanel.instance.StartPointer(confirmButton.transform.position);
     //   TutorialManager.Instance.StartPointer(confirmButton.transform.position, false);
    }
    Transform GetResultTrans()
    {
        for (int i = 0; i < resultItems.Count; i++)
        {
            if (resultItems[i].IdxEmpty == -1)
            {
                return resultItems[i].transform;
            }
        }
        return null;
    }

    void InitData()
    {
        range = GameManager.instance.curRange;
        caculateType = GameManager.instance.curCaculate;

        for (int i = 0; i < caculationItems.Count; i++)
        {
            curEmptyIdxs.Add(true);
        }

        InitCaculation();
        InitResult();
    }

    private void GetCorrectResult()
    {
        countCorrect++;
        OnSoundPLay();
        if(countCorrect == 3)
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                EventManager.OnEndSession?.Invoke();
            });
        }

    }
    void OnSoundPLay()
    {
        if (delayTween != null) delayTween?.Kill();

        delayTween = DOVirtual.DelayedCall(0.1f, () =>
        {
            if (countCorrect == 0)
            {
                SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Sad);
            }
            else if (countCorrect == caculationItems.Count)
            {
                SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Perfect);
            }
            else
            {
                SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Hoow);
            }
        });
    }

    private void OnCheckResult()
    {
        confirmButton.interactable = false;
        countCorrect = 0;
        for (int i = 0; i < caculationItems.Count; i++)
        {
            caculationItems[i].OnCheck(curResultItemsUsed[i]);
        }
        if (punchScale != null && punchScale.IsActive()) return;
        punchScale = confirmButton.transform.DOPunchScale(new Vector2(-0.1f, 0.1f), 0.5f, 2);

    }

    private void GetResultMoveOut(int idxEmpty)
    {
        if (idxEmpty == -1) return;
        curEmptyIdxs[idxEmpty] = true;
        caculationItems[idxEmpty].AssignItem(-1);
    }

    private void InitResult()
    {
        var listInt = new List<int>();
        for (int i = 1; i <= range; i++)
        {
            listInt.Add(i);
        }
        var rdNoRdResult = new RandomNoRepeat<int>(listInt);
        int rdResult = rdNoRdResult.Random();
        for (int i = 0; i < resultItems.Count - results.Count + 1; i++)
        {
            while (results.Contains(rdResult))
            {
                rdResult = rdNoRdResult.Random();
            }
            results.Add(rdResult);
        }

        var listInt2 = new List<int>();
        for (int i = 0; i < resultItems.Count; i++)
        {
            listInt2.Add(i);
        }
        var rdNoRpColor = new RandomNoRepeat<Color>(colors);
        var rdNoRp = new RandomNoRepeat<int>(listInt2);
        for (int i = 0; i < resultItems.Count; i++)
        {
            int rdIdx = rdNoRp.Random();
            resultItems[rdIdx].AssignItem(rdIdx, results[rdIdx], rdNoRpColor.Random());
            resultItems[i].SetStateDrag(false);
        }

        for (int i = 0; i < resultItems.Count; i++)
        {
            int newIDx = i;
            resultItems[newIDx].SetStateDrag(false);
            resultItems[newIDx].transform.DOMoveY(endResultPos[newIDx].y, 7)
            .SetEase(Ease.OutBounce)
            .SetSpeedBased(true)
            .OnComplete(() =>
            {
                resultItems[newIDx].transform.localPosition =
                                new Vector3(resultItems[newIDx].transform.localPosition.x,
                                resultItems[newIDx].transform.localPosition.y,
                                0);
                SoundManager.instance.PlayOtherSfx(SfxOtherType.Popup);
                resultItems[newIDx].AssignPos();
                resultItems[newIDx].transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 2).OnComplete(() =>
                {
                    //  resultItems[0].SetStateDrag(true);
                    resultItems[newIDx].SetStateDrag(true);
                });
            });
        }
    }
    void InitCaculation()
    {
        count++;
        if (count > caculationItems.Count) return;

        curResultItemsUsed.Add(null);
        var firstInt = UnityEngine.Random.Range(1, range);
        var secondInt = UnityEngine.Random.Range(1, range );

        switch (caculateType)
        {
            case CaculateType.Minus:
                while (Mathf.Abs(firstInt - secondInt) > range)
                {
                    firstInt = UnityEngine.Random.Range(1, range);
                    secondInt = UnityEngine.Random.Range(1, range);
                }
                results.Add(Mathf.Abs(firstInt - secondInt));
                break;
            case CaculateType.Plus:
                while ((firstInt + secondInt) > range)
                {
                    firstInt = UnityEngine.Random.Range(1, range);
                    secondInt = UnityEngine.Random.Range(1, range);
                }
                results.Add(firstInt + secondInt);
                break;
            case CaculateType.Divide:
                while ((firstInt % secondInt != 0) || (firstInt % secondInt > range))
                {
                    firstInt = UnityEngine.Random.Range(1, range);
                    secondInt = UnityEngine.Random.Range(1, range);
                }
                results.Add(firstInt / secondInt);
                break;
            case CaculateType.Multiple:
                while ((firstInt * secondInt) > range)
                {
                    firstInt = UnityEngine.Random.Range(1, range);
                    secondInt = UnityEngine.Random.Range(1, range);
                }
                results.Add(firstInt * secondInt);
                break;
        }

        firstInts.Add(firstInt);
        secondInts.Add(secondInt);
        caculationItems[count-1].AssignItem(count-1, firstInt, secondInt, caculateType);

        InitCaculation();
    }

    private void GetEndDragResult(int id, int result, ResultItem item)
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
        confirmButton.interactable = true;
        TutorialManager.Instance.Stop();
        for (int i = 0; i < caculationItems.Count; i++)
        {
            if (!curEmptyIdxs[i]) continue;
            if(Vector2.Distance(item.ResultText.transform.position, 
                caculationItems[i].ResultText.transform.position) <= 2)
            {
                curEmptyIdxs[i] = false;
                caculationItems[i].AssignItem(item.Result);
                curResultItemsUsed[i] = item;
                item.OnSuccess(i, caculationItems[i].ResultText.transform.position, caculationItems[i].transform);
                GetTut();
                return;
            }
        }

        SoundManager.instance.PlayOtherSfx(SfxOtherType.Incorrect);
        item.OnFail();
        GetTut();
    }
}
