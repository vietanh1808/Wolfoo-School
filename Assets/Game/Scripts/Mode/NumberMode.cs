using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using SCN.Tutorial;

public class NumberMode : Panel
{
    [SerializeField] List<GameObject> sessions;
    [SerializeField] Transform endSessionTrans;
    [SerializeField] Transform endOtherBallTrans;
    [SerializeField] Transform endMainBallTrans;
    [SerializeField] Button backBtn;

    int countSession = 0;
    GameObject curSession;

    private void Start()
    {
            FirebaseManager.instance.LogBeginMode(gameObject.name);
        EventManager.OnEndSession += GetEndSession;
        EventManager.OnMoveBall += GetMoveBall;
        backBtn.onClick.AddListener(OnBack);

        curSession = sessions[countSession];
        curSession.gameObject.SetActive(true);
    }

    private void OnBack()
    {
        EventManager.OnBackPanel?.Invoke(this, PanelType.Room1, true);

    }

    private void GetMoveBall(List<BallAnimation> otherBalls, BallAnimation ball)
    {
        ball.transform.SetParent(transform);
        foreach (var item in otherBalls)
        {
            item.transform.SetParent(transform);
            item.PlayAnim();
            item.transform.DOMoveY(endOtherBallTrans.position.y, 5).SetSpeedBased(true);
        }
        ball.PlayAnim();
        ball.transform.DOScale(ball.transform.localScale - Vector3.one * 0.1f, 1);
        ball.transform.DOMove(endMainBallTrans.position, 1);
    }

    private void OnDisable()
    {
        EventManager.OnEndSession -= GetEndSession;
        EventManager.OnMoveBall -= GetMoveBall;
    }

    private void GetEndSession()
    {
        countSession++;
        if(countSession == 1)
        {
            curSession.transform.DOMoveY(endSessionTrans.position.y, 1)
            .OnStart(() =>
            {

                var session = sessions[countSession];
                session.SetActive(true);
                curSession = session;
                //session.transform.position = Vector3.down * endSessionTrans.position.y;
                //session.transform.DOMoveY(0, 1).OnComplete(() =>
                //{
                //    curSession = session;
                //});
            });
        }
        else if(countSession == 2)
        {
            OnEndgame();
        }
    }
    void OnEndgame()
    {
        EventManager.OnEndgame?.Invoke(gameObject, PanelType.Room1, true, null);
    }
}
