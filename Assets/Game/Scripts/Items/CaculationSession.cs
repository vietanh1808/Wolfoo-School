using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CaculationSession : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] List<Button> caculateBtns;
    [SerializeField] List<BallAnimation> ballAnimations;
    [SerializeField] Text rangeText;

    float curIdx;
    float curValue;
    private int range = 10;
    private int count = 0;

    private void Awake()
    {
        //foreach (var item in ballAnimations)
        //{
        //    Debug.Log(item.name);
        //    item.gameObject.SetActive(false);
        //}
        for (int i = 0; i < ballAnimations.Count; i++)
        {
            ballAnimations[i].gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        caculateBtns[0].onClick.AddListener(() => OnClickCaculate(0));
        caculateBtns[1].onClick.AddListener(() => OnClickCaculate(1));
        caculateBtns[2].onClick.AddListener(() => OnClickCaculate(2));
        caculateBtns[3].onClick.AddListener(() => OnClickCaculate(3));
        slider.onValueChanged.AddListener(GetValueSlider);

        ShowBall(() =>
        {
             slider.value = 0;
            GetValueSlider(0);
        });
    }
    void ShowBall(System.Action OnComplete )
    {
        count++;
        if (count > ballAnimations.Count)
        {
            OnComplete.Invoke();
            return;
        }

        DOVirtual.DelayedCall(0.25f, () =>
        {
            ballAnimations[count-1].gameObject.SetActive(true);
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Popup);
            ShowBall(OnComplete);
        });
    }

    private void OnClickCaculate(int idx)
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
        var listBall = new List<BallAnimation>();
        for (int i = 0; i < caculateBtns.Count; i++)
        {
            caculateBtns[i].interactable = false;
            if (idx == i) continue;
            listBall.Add(ballAnimations[i]);
        }
        EventManager.OnMoveBall?.Invoke(listBall, ballAnimations[idx]);

        switch (idx)
        {
            case 0:
                GameManager.instance.curCaculate = CaculateType.Minus;
                break;
            case 1:
                GameManager.instance.curCaculate = CaculateType.Plus;
                break;
            case 2:
                GameManager.instance.curCaculate = CaculateType.Divide;
                break;
            case 3:
                GameManager.instance.curCaculate = CaculateType.Multiple;
                break;  
            default:
                break;
        }
        GameManager.instance.curRange = range;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            EventManager.OnEndSession?.Invoke();
        });
    }

    private void GetValueSlider(float value)
    {
        curValue = value;
        if (slider.value > 0.5f)
        {
            if (curValue != 1)
            {
                curValue = 1;
               // slider.value = curValue;
                rangeText.text = "1 - 20";
                range = 20;
                caculateBtns[(int)CaculateType.Multiple].transform.parent.gameObject.SetActive(false);
                caculateBtns[(int)CaculateType.Divide].transform.parent.gameObject.SetActive(false);
                return;
            }
        }
        else if (slider.value <= 0.5f)
        {
            if (curValue != 0.5f)
            {
                curValue = 0.5f;
                //    slider.value = curValue;
                rangeText.text = "1 - 10";
                range = 10;
                caculateBtns[(int)CaculateType.Multiple].transform.parent.gameObject.SetActive(true);
                caculateBtns[(int)CaculateType.Divide].transform.parent.gameObject.SetActive(true);
                return;
            }
        }
    }
}
public enum CaculateType
{
    Minus,
    Plus,
    Divide,
    Multiple,
}
