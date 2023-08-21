using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedTable : MonoBehaviour
{
    [SerializeField] List<Transform> listTrans;
    [SerializeField] Button playgameBtn;
    [SerializeField] ParticleSystem rainbowFx;
    [SerializeField] Panel parentPanel;
    [SerializeField] PanelType nextMode;

    int nextPosIdx = 0;
    bool hasNewGift;

    private void Start()
    {
        playgameBtn.onClick.AddListener(OnPlaygame);
        playgameBtn.transform.DOPunchScale(Vector3.one * 0.2f, 1f, 2).SetLoops(-1, LoopType.Restart);
        EventManager.OnEndgame += GetEndgame;
    }
    private void OnEnable()
    {
        if(hasNewGift)
        {
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Wow);
            rainbowFx.Play();
            hasNewGift = false;
        }
    }
    private void OnDestroy()
    {
        EventManager.OnEndgame -= GetEndgame;
    }

    private void GetEndgame(GameObject gameObject_, PanelType panelType, bool isDestroy, GameObject item_)
    {
        if (item_ == null) return;
        if (!gameObject_.name.Contains("FruitDecorMode")) return;

        hasNewGift = true;
        if (listTrans[nextPosIdx].childCount > 0)
        {
            listTrans[nextPosIdx].GetChild(0).gameObject.SetActive(false);
        }
        var item = Instantiate(item_, listTrans[nextPosIdx]);
        item.transform.SetAsFirstSibling();
        //for (int i = 0; i < item.transform.GetChild(0).childCount; i++)
        //{
        //    var itemChild = item.transform.GetChild(0).GetChild(i).GetComponent<DecorDragItem>();
        //    itemChild.enabled = false;
        //    Destroy(itemChild);
        //}
        foreach (var itemChild in item.transform.GetComponentsInChildren<DecorDragItem>())
        {

            itemChild.enabled = false;
         //   Destroy(itemChild);
        }
        item.transform.localScale = Vector3.one * 0.45f;
        item.transform.localPosition = Vector3.zero;
        rainbowFx.transform.position = listTrans[nextPosIdx].position;

        nextPosIdx++;
        nextPosIdx = nextPosIdx >= listTrans.Count ? 0 : nextPosIdx;
    }

    private void OnPlaygame()
    {
        EventManager.OnPlaygame?.Invoke(parentPanel, nextMode);
    }
}
