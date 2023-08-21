using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class ReadyPanel : MonoBehaviour
{
    [SerializeField] Text countText;
    [SerializeField] Button readyPanel;
    [SerializeField] int countTime = 3;

    bool clicked;
    Tween loopTween;

    private void Start()
    {
        readyPanel.onClick.AddListener(OnClickPanel);
    }

    private void OnClickPanel()
    {

    }

    public void OpenTutorial()
    {
        transform.SetAsLastSibling();
        readyPanel.gameObject.SetActive(true);
    }
    public void OnCountTime(System.Action OnComplete = null)
    {
        countText.text = countTime.ToString();
        OnCount();
        countTime--;
        loopTween = DOVirtual.DelayedCall(1.1f, () =>
        {
            if (countTime < 0)
            {
                gameObject.SetActive(false);
                loopTween.Kill();
                OnComplete?.Invoke();
                return;
            } else if(countTime == 0)
            {
                countText.text = "start";
            } else
            {
                countText.text = countTime.ToString();
            }
            countTime--;
            OnCount();
        }).SetLoops(-1);
    }

    void OnCount()
    {
        countText.transform.DOScale(2, 0)
        .OnComplete(() =>
        {
            countText.transform.DOScale(1, 0.5f)
            .OnComplete(() =>
            {
                countText.transform.DOScale(0, 0.5f)
                .OnComplete(() =>
                {
                    return;
                });
            });
        });
    }
}
