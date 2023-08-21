using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoard : ItemMove
{
    [SerializeField] Transform itemZone;
    [SerializeField] Trunk trunkPb;

    private Petal[] curItems;
    private List<Trunk> curTrunks = new List<Trunk>();
    private int curItemIdx;
    private int maxItem;
    private Leaf[] curLeafItems;

    public Petal[] CurItems { get => curItems; }
    public List<Trunk> CurTrunks { get => curTrunks; }
    public Leaf[] CurLeafItems { get => curLeafItems; }

    private void Start()
    {
    }

    public void AssignPetalBoard(Sprite[] itemSprites)
    {
        curItems = itemZone.GetComponentsInChildren<Petal>();
        for (int i = 0; i < curItems.Length; i++)
        {
            if(i < itemSprites.Length)
            {
                curItems[i].AssignItem(i, itemSprites[i]);
                curItems[i].gameObject.SetActive(true);
            }
            else
            {
                curItems[i].gameObject.SetActive(false);
            }
        }
    }
    public void AssignLeafBoard(Sprite[] itemSprites)
    {
        curLeafItems = itemZone.GetComponentsInChildren<Leaf>();
        for (int i = 0; i < curLeafItems.Length; i++)
        {
            if(i < itemSprites.Length)
            {
                curLeafItems[i].AssignItem(i, itemSprites[i]);
                curLeafItems[i].gameObject.SetActive(true);
            }
            else
            {
                curLeafItems[i].gameObject.SetActive(false);
            }
        }
    }
    public void AssignTrunkBoard(Sprite[] itemSprites)
    {
        curItemIdx = itemZone.childCount - 1;
        maxItem = itemSprites.Length;

        int curItemBoardIdx = 0;
        for (int i = 0; i < itemSprites.Length; i++)
        {
            var trunk = Instantiate(trunkPb, itemZone.GetChild(curItemBoardIdx));
            trunk.transform.localPosition = Vector3.zero;
            trunk.InitStartInfo();
            trunk.AssignItem(i, itemSprites[i]);
            if (i < itemZone.childCount)
            {
                trunk.OnShow(itemZone.GetChild(curItemBoardIdx), null);
            }
            else
            {
                trunk.OnHide();
            }
            curTrunks.Add(trunk);
            curItemBoardIdx = curItemBoardIdx + 1 >= itemZone.childCount ? 0 : curItemBoardIdx + 1;
        }
    }
    public void ShowNextItem(int siblingIdx)
    {
        curItemIdx++;
        if(curItemIdx >= maxItem)
        {
            curItemIdx = 0;
        }
        if(curTrunks[curItemIdx].IsShown)
        {
            ShowNextItem(siblingIdx);
        }
        curTrunks[curItemIdx].OnShow(itemZone.GetChild(siblingIdx), null);
    }
}
