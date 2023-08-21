using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class BackgroundMoveItem : MonoBehaviour
{
    [SerializeField] Image backGroundImg;
    [SerializeField] Image backPlantImg;
    [SerializeField] Image backSkyImg;
    [SerializeField] Image backMountainImg;
    [SerializeField] Transform startTrans;
    [SerializeField] Transform endTrans;
    [SerializeField] Transform objstacleZone;
    [SerializeField] Transform desinationTrans;

    float totalTime = 10;
    private bool isMoved;
    private Tween tweenMove1, tweenMove2, tweenMove3, tweenMove4;

    public Transform DesinationTrans { get => desinationTrans; }
    public bool IsMoved { get => isMoved; }
    public Transform ObjstacleZone { get => objstacleZone; }
    public Image BackMountainImg { get => backMountainImg; }

    // private Sequence tweenMove;


    private void Awake()
    {
        backGroundImg.transform.position = new Vector3(
            startTrans.position.x, 
            backGroundImg.transform.position.y, 
            transform.position.z);
        backMountainImg.transform.position = new Vector3(
            startTrans.position.x, 
            backMountainImg.transform.position.y,
            transform.position.z);
        backSkyImg.transform.position = new Vector3(
            startTrans.position.x, 
            backSkyImg.transform.position.y,
            transform.position.z);
        backPlantImg.transform.position = new Vector3(
            startTrans.position.x, 
            backPlantImg.transform.position.y,
            transform.position.z);
    }

    public float GetTotalDistance()
    {
        return Vector2.Distance(startTrans.position, endTrans.position);
    }
    public float GetCurDistance()
    {
        return Vector2.Distance(backGroundImg.transform.position, endTrans.position);
    }

    public void MapMove()
    {
        if (isMoved) return;
        isMoved = true;

        tweenMove1 = backSkyImg.transform.DOMoveX(endTrans.position.x, totalTime)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear);
        tweenMove2 = backMountainImg.transform.DOMoveX(endTrans.position.x - 10, totalTime - 2)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear);
        tweenMove3 = backPlantImg.transform.DOMoveX(endTrans.position.x, totalTime)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear);
        tweenMove4 = backGroundImg.transform.DOMoveX(endTrans.position.x, totalTime)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    tweenMove3.Kill();
                    EventManager.OnCompletedRace?.Invoke();
                });
    }

    public void MapPause()
    {
        if (!isMoved) return;
        isMoved = false;

        //if (tweenMove1 == null || !tweenMove1.IsActive()) return;
        //tweenMove1.Kill();
        //if (tweenMove2 == null || !tweenMove2.IsActive()) return;
        //tweenMove2.Kill();
        //if (tweenMove3 == null || !tweenMove3.IsActive()) return;
        //tweenMove3.Kill();
        //if (tweenMove4 == null || !tweenMove4.IsActive()) return;
        //tweenMove4.Kill();

        tweenMove1?.Kill();
        tweenMove2?.Kill();
        tweenMove3?.Kill();
        tweenMove4?.Kill();
    }
}
