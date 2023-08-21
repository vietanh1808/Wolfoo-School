using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using SCN.Common;

public class Refrigerator : BackItem
{
    [SerializeField] Transform endTrans;
    [SerializeField] Button doorBtn;
    [SerializeField] List<ItemInRefrigerator> itemsInside;
    private int maxItem;
    private RandomNoRepeat<GameObject> rdNoRp;
    private Tweener rotateTween;

    protected override void Start()
    {
        base.Start();
        doorBtn.onClick.AddListener(GetItemInside);

        maxItem = itemsInside.Count;
    }

    private void GetItemInside()
    {
        doorBtn.interactable = false;
        if(rotateTween == null || !rotateTween.IsActive())
        {
            rotateTween = transform.DOPunchRotation(Vector3.forward * 5, 0.5f, 2).OnComplete(() =>
            {
                doorBtn.interactable = true;
            });
        }

        int rd = UnityEngine.Random.Range(0, itemsInside.Count);
        var newObject = Instantiate(itemsInside[rd], itemsInside[0].transform.parent);
        newObject.AssignItem();
        newObject.transform.DOMoveY(endTrans.position.y, 1).OnComplete(() =>
        {
            newObject.transform.SetParent(endTrans.parent.parent);
        });

    }

    public override void GetEndDragBackItem(BackItem image_, int id_)
    {
        base.GetEndDragBackItem(image_, id_);
    }
}
