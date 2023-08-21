using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SCN.ActionLib;
using UnityEngine.UI;
using DG.Tweening;
using System;
using SCN.Tutorial;

public class SessionCutItem : Panel
{
    [SerializeField] Image dotImg;
    [SerializeField] Image fillImg;
    [SerializeField] Transform paperZone;
    [SerializeField] Image mainPaper;
    [SerializeField] PaperItem paperItemPb;
    [SerializeField] ParticleSystem smokeFx;
    [SerializeField] ParticleSystem starFx;
    [SerializeField] Button blockBtn;
    [SerializeField] Image remainImg;
    [SerializeField] Button sliceBtn;
    [SerializeField] Transform outsideTrans;
    [SerializeField] GameObject highlightPanel;

    DragFollowPath dragFollow;
    ShapeModeDataSO data;
    private Tweener punchTween;
    private Tweener punchTween1;
    private Tweener punchTween2;
    int curIdxPaper = 0;
    int curIdxShape;
    private bool isCompleteCut;
    private Tweener loopTween;
    private bool isClickPanel;
    private bool isEndSession;

    private void Awake()
    {
        dotImg.fillAmount = 0; 
        fillImg.fillAmount = 0;
        sliceBtn.image.DOFade(0, 0);
        remainImg.DOFade(0, 0);
        blockBtn.transform.localScale = Vector3.zero;
        blockBtn.interactable = false;
        sliceBtn.interactable = false;
    }

    private void Start()
    {
            FirebaseManager.instance.LogBeginMode(gameObject.name);
        EventManager.OnClickPaper += GetClickPaper;
        EventManager.OnClickTutorial += GetClickTutorial;
        sliceBtn.onClick.AddListener(OnOpenBlockShape);
        blockBtn.onClick.AddListener(OnEndSession);

        GameManager.instance.GetDataSO(DataSOType.Shape);
        data = GameManager.instance.ShapeDataSO;

        curIdxShape = GameManager.instance.curShapeIdx;
        for (int i = 0; i < data.paperColorSprites.Count; i++)
        {
            var item = Instantiate(paperItemPb, paperZone);
            item.AssignItem(i, data.paperColorSprites[i]);
            if (i == 0)
            {
                curIdxPaper = i;
                item.OnAnim();
                mainPaper.sprite = data.paperColorSprites[i];
                mainPaper.SetNativeSize();
            }
        }
        dotImg.sprite = data.emptySliceSprites[GameManager.instance.curShapeIdx];
        dotImg.SetNativeSize();
        fillImg.sprite = data.emptySliceSprites[GameManager.instance.curShapeIdx];
        fillImg.SetNativeSize();

        dragFollow = Instantiate(data.shapeCuts[GameManager.instance.curShapeIdx], mainPaper.transform.parent);
        dragFollow.OnProgress += OnProgress;
        dragFollow.DragObject.gameObject.SetActive(false);

        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
        if (isClickPanel) return;
        GetTut();
    }
    /// <summary>
    /// Get Tutorial
    /// </summary>
    void GetTut()
    {
        dotImg.DOFillAmount(1, 1.5f).OnComplete(() =>
        {
            if (isClickPanel)
            {
                TutorialPanel.instance.DisableTut();
                return;
            }
            highlightPanel.transform.DOScale(18, 1).OnComplete(() =>
            {
                // Tutorial
                if (isClickPanel)
                {
                    TutorialPanel.instance.DisableTut();
                    return;
                }
                TutorialPanel.instance.StartPointer(dragFollow.transform.GetChild(0));
            });
        });
    }

    private void OnDisable()
    {
        EventManager.OnClickPaper -= GetClickPaper;
        dragFollow.OnProgress -= OnProgress;
        EventManager.OnClickTutorial -= GetClickTutorial;
    }

    private void GetClickTutorial()
    {
        isClickPanel = true;
        highlightPanel.transform.DOScale(100, 0.5f).OnComplete(() =>
        {
            highlightPanel.gameObject.SetActive(false);
            dragFollow.DragObject.gameObject.SetActive(true);
            dragFollow.DragObject.transform
            .DOMove(dragFollow.transform.GetChild(0).GetChild(0).position, 1)
            .OnComplete(() =>
            {
                dragFollow.Init();
            });
        });
    }

    private void OnEndSession()
    {
        if (isEndSession) return;

        isEndSession = true;
        blockBtn.interactable = false;
        loopTween.Kill();
        blockBtn.transform.SetParent(transform.parent);

        EventManager.OnEndSession?.Invoke();

        SoundManager.instance.PlayOtherSfx(SfxOtherType.BongBong);
        blockBtn.transform.SetParent(GUIManager.instance.curShapeMode.transform);
        blockBtn.transform.SetAsLastSibling();
        blockBtn.transform.DOScale(0.3f, 2f);
        blockBtn.transform.DOJump(Vector3.zero, 1, 1, 2f)
        .OnComplete(() =>
        {
            EventManager.OnCompleteMove?.Invoke(blockBtn.gameObject);
        });
    }

    private void OnOpenBlockShape()
    {
        loopTween.Kill();
        sliceBtn.interactable = false;

        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);

        starFx.Play();
        sliceBtn.transform.DOScale(0, 1);
        blockBtn.transform.DOScale(1, 1).OnComplete(() =>
        {
            loopTween = blockBtn.transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 1)
            .SetLoops(-1, LoopType.Restart);
            blockBtn.interactable = true;
        });
    }

    private void GetClickPaper(int idx, PaperItem paper)
    {
        if (idx == curIdxPaper) return;

        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        curIdxPaper = idx;
        GameManager.instance.curIdxColor = idx;
        paper.OnAnim();
        smokeFx.Play();

        mainPaper.sprite = data.paperColorSprites[idx];
        mainPaper.SetNativeSize();
    }

    void OnProgress(float value)
    {
        fillImg.fillAmount = value + 0.01f;

        if(!SoundManager.instance.Sfx.isPlaying)
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Cutting);

        if(value > 0.9f)
        {
            if (isCompleteCut) return;
            isCompleteCut = true;
            EventManager.OnCompleteCutBox?.Invoke();
            OnCompleteCutBox();
            return;
        }

        if (punchTween2 != null && punchTween2.IsActive()) return;
        punchTween = mainPaper.transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 1);
        punchTween1 = dotImg.transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 1);
        punchTween2 = fillImg.transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 1);
    }

    private void OnCompleteCutBox()
    {
       // paperZone.DOMoveX(-outsideTrans.position.x, 1f);
        dragFollow.gameObject.SetActive(false);

        sliceBtn.image.sprite = data.shapeColors[curIdxShape].sliceSprites[curIdxPaper];
        sliceBtn.image.SetNativeSize();
        remainImg.sprite = data.shapeColors[curIdxShape].remainSliceSprites[curIdxPaper];
        remainImg.SetNativeSize();
        blockBtn.image.sprite = data.shapeColors[curIdxShape].blockSprites[curIdxPaper];
        blockBtn.image.SetNativeSize();

        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
        float time = 1;
        sliceBtn.image.DOFade(1, time);
        remainImg.DOFade(1, time);
        dotImg.DOFade(0, time);
        fillImg.DOFade(0, time);
        mainPaper.DOFade(0, time).OnComplete(() =>
        {
            remainImg.transform.DOMove(Vector3.up * 2, 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                remainImg.transform.DOMoveX(outsideTrans.position.x, 1)
                .OnComplete(() =>
                {
                    TutorialManager.Instance.StartPointer(sliceBtn.transform.position);
                    loopTween = sliceBtn.transform.DOPunchScale(new Vector3(-0.1f, 0.1f, 0), 0.5f, 1)
                    .SetLoops(-1, LoopType.Restart);
                    sliceBtn.interactable = true;
                });
            });
        });
    }
}
