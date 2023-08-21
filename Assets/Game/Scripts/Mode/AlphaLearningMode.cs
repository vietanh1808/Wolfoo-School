using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using SCN.UIExtend;

public class AlphaLearningMode : Panel
{
    [SerializeField] ScratchCardManager cardManagerPb;
    [SerializeField] Rake rakePb;
    [SerializeField] Image picture;
    [SerializeField] HorizontalScrollInfinity horizontalScrollInfinity;
    [SerializeField] ParticleSystem starFx;
    [SerializeField] Button backBtn;
    [SerializeField] SpriteRenderer alphaSpritePb;

    Rake rake;
    AlphaLearningDataSO data;
    private Tween delayTWeen;
    private Tweener punchTween;
    ScratchCardManager cardManager;
    List<ScratchCardManager> cardManagers = new List<ScratchCardManager>();
    SpriteRenderer curAlpha;
    private Vector3 startScale;

    private void Start()
    {
            FirebaseManager.instance.LogBeginMode(gameObject.name);
        backBtn.onClick.AddListener(OnBack);

        rake = Instantiate(rakePb, GUIManager.instance.SpawnModeAlphaZone);
        CreateCardManager();

        curAlpha = Instantiate(alphaSpritePb, GUIManager.instance.SpawnModeAlphaZone);
        curAlpha.transform.SetParent(GUIManager.instance.SpawnModeAlphaZone);
        startScale = curAlpha.transform.localScale;

        GameManager.instance.GetDataSO(DataSOType.AlphaLearning);
        data = GameManager.instance.AlphaLearningDataSO;

        delayTWeen = DOVirtual.DelayedCall(0.2f, () =>
        {
            horizontalScrollInfinity.Setup(52, this);
        });

        EventManager.OnClickAlpha += GetClickAlpha;
        EventManager.OnBeginDragDrake += GetBeginDragDrake;
        EventManager.OnEndDragDrake += GetEndDragDrake;
        //  CalculateCamSize();
    }

    private void OnDestroy()
    {
        EventManager.OnBeginDragDrake -= GetBeginDragDrake;
        EventManager.OnEndDragDrake -= GetEndDragDrake;
        EventManager.OnClickAlpha -= GetClickAlpha;

        if (delayTWeen != null) delayTWeen?.Kill();
        for (int i = 0; i < GUIManager.instance.SpawnModeAlphaZone.childCount; i++)
        {
            Destroy(GUIManager.instance.SpawnModeAlphaZone.GetChild(i).gameObject);
        }
    }
    void CalculateCamSize()
    {
        var width = Screen.width;
        var height = Screen.height;

        var camSize = Camera.main.orthographicSize;

        camSize = (1920 / 2 * height) / width;
        Camera.main.orthographicSize = camSize / 100;
    }
    private void OnBack()
    {
        EventManager.OnBackPanel?.Invoke(this, PanelType.Room2, true);
    }

    private void GetClickAlpha(int idx, bool isLower)
    {
        SoundManager.instance.SpeakAlpha(idx);
        starFx.Play();

        if (punchTween != null)
        {
            curAlpha.transform.localScale = startScale;
            punchTween?.Kill();
        }

        curAlpha.sprite = !isLower ? data.uppercaseSprites[idx] : data.lowercaseSprites[idx];
        curAlpha.transform.localScale = isLower ?  Vector3.one * 0.7f : Vector3.one;
        startScale = curAlpha.transform.localScale;

        punchTween = curAlpha.transform.DOPunchScale(new Vector3(0.1f, -0.1f, 0), 0.5f, 2, 1);

        for (int i = 0; i < cardManagers.Count; i++)
        {
            cardManagers[i].gameObject.SetActive(false);
        }
        CreateCardManager();
    }

    private void GetEndDragDrake()
    {
        cardManager.Card.Mode = ScratchCard.ScratchMode.Erase;
    }

    void CreateCardManager()
    {
        cardManager = Instantiate(cardManagerPb, GUIManager.instance.SpawnModeAlphaZone);
        cardManager.Card.Mode = ScratchCard.ScratchMode.Restore;
        cardManagers.Add(cardManager);

        DOVirtual.DelayedCall(0.1f, () =>
        {
            cardManager.Card.FillInstantly();
        });
    }

    private void GetBeginDragDrake()
    {
        //  if (tween != null && tween.IsActive()) return;
        CreateCardManager();

        cardManager.SpriteCard.GetComponent<SpriteRenderer>().sortingOrder = 12;
    }
}
