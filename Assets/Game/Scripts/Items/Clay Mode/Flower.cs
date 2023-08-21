using DG.Tweening;
using SCN;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flower : ItemMove
{
    [SerializeField] Transform trunkZone;
    [SerializeField] Transform petalZone;
    [SerializeField] Transform leafZone;
    [SerializeField] Transform pistilZone;
    [SerializeField] ParticleSystem trailFx;

    private Petal[] petalItems;
    private Leaf[] leafItems;
    private Trunk trunk;
    private Pistil pistil;
    private Petal[] petalItems2;
    private Vector3 startLocalPos;

    public Transform TrunkZone { get => trunkZone; }
    public Transform PetalZone { get => petalZone; }
    public Transform LeafZone { get => leafZone; }
    public Petal[] PetalItems { get => petalItems; }
    public Leaf[] LeafItems { get => leafItems; }
    public Trunk Trunk { get => trunk; }

    private void Awake()
    {
        petalItems = petalZone.GetComponentsInChildren<Petal>();
        leafItems = leafZone.GetComponentsInChildren<Leaf>();
        trunk = trunkZone.GetComponentInChildren<Trunk>();
        pistil = pistilZone.GetComponentInChildren<Pistil>();
    }

    private void Start()
    {
        trunk.GetComponent<Image>().enabled = false;
        trailFx.Stop();
    }
    private void OnEnable()
    {
        EventDispatcher.Instance.RegisterListener<EventKey.OnBeginDragItem>(GetBeginDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnDragItem>(GetDragItem);
        EventDispatcher.Instance.RegisterListener<EventKey.OnEndDragItem>(GetEndDragItem);
    }
    private void OnDisable()
    {
        EventDispatcher.Instance.RemoveListener<EventKey.OnBeginDragItem>(GetBeginDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnDragItem>(GetDragItem);
        EventDispatcher.Instance.RemoveListener<EventKey.OnEndDragItem>(GetEndDragItem);
    }
    public void AssignLeaf(Sprite[] itemSprites)
    {
        for (int i = 0; i < leafItems.Length; i++)
        {
            if (i < itemSprites.Length)
            {
                leafItems[i].AssignItem(i, itemSprites[i]);
                leafItems[i].StopTut();
            }
            else
            {
                petalItems[i].gameObject.SetActive(false);
            }
        }
    }
    public void InitLocalPos()
    {
        startLocalPos = transform.localPosition;
    }
    public void AssignTrunk(Sprite sprite)
    {
        trunk.AssignItem(0, sprite);
        trunk.gameObject.SetActive(false);
    }
    public void AssignTrunk2(Sprite sprite)
    {
        trunk.AssignItem(0, sprite);
    }
    public void AssignPetal(Sprite[] itemSprites)
    {
        for (int i = 0; i < petalItems.Length; i++)
        {
            if (i < itemSprites.Length)
            {
                petalItems[i].AssignItem(i, itemSprites[i]);
                petalItems[i].StopTut();
            }
            else
            {
                petalItems[i].gameObject.SetActive(false);
            }
        }
    }
    public void AssignPetal(Petal[] petals)
    {
        petalItems2 = petals;
    }
    public void DrawColor(Sprite pistilSprite, Sprite[] petalSprites)
    {
        pistil.AssignItem(pistilSprite);
        for (int i = 0; i < petalSprites.Length; i++)
        {
            if(petalItems != null)
                petalItems[i].AssignItem(i, petalSprites[i]);
            petalItems2[i].AssignItem(i, petalSprites[i]);
        }
    }

    private void GetBeginDragItem(EventKey.OnBeginDragItem item)
    {
        if(item.petal != null)
        {
            petalItems[item.petal.Id].GetTut();
        }
        if(item.trunk != null)
        {
            trunk.AssignTut( item.trunk.Sprite);
            trunk.GetTut();
        }
        if(item.leaf != null)
        {
            leafItems[item.leaf.Id].GetTut();
        }
    }

    private void GetDragItem(EventKey.OnDragItem item)
    {
    }

    private void GetEndDragItem(EventKey.OnEndDragItem item)
    {
        if (item.petal != null)
        {
            petalItems[item.petal.Id].StopTut();
        }
        if (item.trunk != null)
        {
            trunk.StopTut();
        }
        if (item.leaf != null)
        {
            leafItems[item.leaf.Id].StopTut();
        }
    }
    public void CheckIndexSibling(Petal _petal, int idx)
    {
        _petal.transform.SetParent(petalZone);
        _petal.transform.SetSiblingIndex(idx);
    }
    public void OnDancing()
    {
        trailFx.Play();
        moveTween = transform.DOPunchPosition(Vector3.up * 50, 1, 1)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }
    public void StopDancing()
    {
        if (moveTween != null) moveTween?.Kill();
        transform.localPosition = startLocalPos;
    }
}
