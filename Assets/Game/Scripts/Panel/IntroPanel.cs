using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SCN.IAP;

public class IntroPanel : Panel
{
    [SerializeField] Button playBtn;
    [SerializeField] Button iapBtn;
    [SerializeField] Button settingBtn;
    [SerializeField] Image logoImg;
    [SerializeField] List<CharacterAnimation> wolfooAnims;
    [SerializeField] Transform startLeftTrans;
    [SerializeField] Transform startRightTrans;
    [SerializeField] Transform startBtnTrans;
    [SerializeField] Transform endBtnTrans;
    [SerializeField] Transform doorZone;

    private Vector3 startScale;
    List<Vector3> listEndTrans = new List<Vector3>();
    private Tweener loopScaleTween;
    private Tweener moveTween;
    private Tweener scaleLogoTween;
    private Tweener rotateTween;
    private int countIdx;
    private Vector3 startWolfooScale;

    private void Awake()
    {
        for (int i = 0; i < wolfooAnims.Count; i++)
        {
            listEndTrans.Add(wolfooAnims[i].transform.position);

            wolfooAnims[i].transform.position = i < 2 ? 
                new Vector3(startLeftTrans.position.x, wolfooAnims[i].transform.position.y , 0) :
                    new Vector3(startRightTrans.position.x, wolfooAnims[i].transform.position.y, 0);
        }
        startScale = logoImg.transform.localScale;
        startWolfooScale = wolfooAnims[0].transform.localScale;
        logoImg.transform.localScale = Vector3.zero;
        playBtn.transform.position = new Vector3(playBtn.transform.position.x, startBtnTrans.position.y, 0);
    }
    private void OnEnable()
    {
        iapBtn.gameObject.SetActive(!AdsManager.Instance.IsRemovedAds);
    }

    private void Start()
    {
        playBtn.onClick.AddListener(OnPlaygame);
        iapBtn.onClick.AddListener(OnOpenIapPanel);
        settingBtn.onClick.AddListener(OnClickSetting);

        EventManager.OnBuyRemoveAds += GetBuyRemoveAds;

        //   iapBtn.gameObject.SetActive(!AdsManager.Instance.IsRemovedAds);
        AdsManager.Instance.HideBanner();

        //transform.DOScale(Vector3.one, 1).OnComplete(() =>
        //{
        countIdx = 0;
        // Wolfoo & Friend
        for (int i = 0; i < wolfooAnims.Count; i++)
        {
            int idx = i;
            wolfooAnims[idx].PlayMove();
            wolfooAnims[idx].transform.DOMoveX(listEndTrans[idx].x, 1f).OnComplete(() =>
            {
                wolfooAnims[idx].PlayWaveHand();
            });
        }

        // Button
        moveTween = playBtn.transform.DOMoveY(endBtnTrans.position.y, 1).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            loopScaleTween = playBtn.transform.DOPunchScale(Vector3.one * 0.1f, 1, 1).SetLoops(-1, LoopType.Yoyo);

            // Logo Image
            scaleLogoTween = logoImg.transform.DOScale(startScale, 1).SetEase(Ease.OutBack).OnComplete(() =>
            {
            });
        });
        //  });
    }


    private void OnDestroy()
    {
        EventManager.OnBuyRemoveAds -= GetBuyRemoveAds;

        rotateTween?.Kill();
        scaleLogoTween?.Kill();
        moveTween?.Kill();
        loopScaleTween?.Kill();
    }

    void GetBuyRemoveAds()
    {
      //  FirebaseManager.instance.LogBuyIAP("");
        iapBtn.gameObject.SetActive(false);
    }

    private void OnOpenIapPanel()
    {
        StartCoroutine(IAPManager.Instance.IEStart());
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        IAPPolicyDialog.Instance.OpenDialog(() =>
        {
            //GUIManager.instance.OpenPanel(PanelType.IAP);
            IAPManager.Instance.OpenPanel();
        });
    }
    void WolfooRunToSchool()
    {
        var idx = countIdx;
        wolfooAnims[idx].PlayMove();
        wolfooAnims[idx].transform.DOScale(wolfooAnims[countIdx].transform.localScale / 2, 1);
        wolfooAnims[idx].transform.DOMove(doorZone.transform.position, 1).OnComplete(() =>
        {
            wolfooAnims[idx].gameObject.SetActive(false);
        });

        DOVirtual.DelayedCall(1f / 3f, () =>
        {
            countIdx++;
            if (countIdx == wolfooAnims.Count)
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    OnGotoMain();
                });
            }
            else
            {
                WolfooRunToSchool();
            }
        });
    }
    private void OnClickSetting()
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);
        EventManager.OpenPanel?.Invoke(PanelType.Setting);
    }

    void OnGotoMain()
    {
        Destroy(gameObject);
        EventManager.OpenPanel?.Invoke(PanelType.Main);
    }

    private void OnPlaygame()
    {
        playBtn.interactable = false;
        loopScaleTween?.Kill();

        loopScaleTween = playBtn.transform.DOScale(Vector3.zero, 0.5f);
        logoImg.transform.DOPunchRotation(Vector3.forward * 10, 0.5f, 2);

        WolfooRunToSchool();
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);

        //GUIManager.instance.loadingPanel.Open(null, () =>
        //{
        //    GUIManager.instance.OpenPanel(PanelType.Home);
        //});
    }
}
