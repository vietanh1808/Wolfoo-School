using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using SCN.Ads;

public class ShapeMode : Panel
{
    [SerializeField] List<GameObject> sessionPbs;
    [SerializeField] Button backBtn;

    private int countSession = -1;
    private GameObject curSession;
    float height;
    bool isLose;

    private void Start()
    {
            FirebaseManager.instance.LogBeginMode(gameObject.name);
        EventManager.OnEndSession += GetEndSession;
        EventManager.OnLose += GetLose;
        backBtn.onClick.AddListener(OnBack);

        GetEndSession();
        height = GetComponent<RectTransform>().rect.height;

        GUIManager.instance.curShapeMode = this;
    }

    private void OnBack()
    {
        EventManager.OnBackPanel?.Invoke(this, PanelType.Room1, true);
    }

    private void OnDestroy()
    {
        EventManager.OnLose -= GetLose;
        EventManager.OnEndSession -= GetEndSession;
    }

    private void GetLose()
    {
        isLose = true;
    }

    private void GetEndSession()
    {
        countSession++;
        if(countSession >= sessionPbs.Count)
        {
            if(isLose)
            {
                GUIManager.instance.GetLoseGame(gameObject, PanelType.Room1);
                return;
            }
            EventManager.OnEndgame?.Invoke(gameObject, PanelType.Room1, true, null);
        }
        else
        {
            var session = Instantiate(sessionPbs[countSession], transform);
            if (countSession == 2)
            {
                    //OnNextModeGalaxy(session);
                if (AdsManager.Instance.HasInters)
                {
                    AdsManager.Instance.ShowInterstitial((a) =>
                    {
                        OnNextModeGalaxy(session);
                    });
                }
                else
                {
                        OnNextModeGalaxy(session);
                }
            }
            else
            {
                curSession = session;
            }

            backBtn.transform.SetAsLastSibling();
        }
    }

    void OnNextModeGalaxy(GameObject session)
    {
        session.transform.localPosition = Vector3.down * height;
        curSession.transform.DOLocalMoveY(height, 2).SetEase(Ease.Linear).OnComplete(() =>
        {
            curSession.gameObject.SetActive(false);
        });
        session.transform.DOMove(Vector3.zero, 2).SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                curSession = session;
            });
        });
    }
}
