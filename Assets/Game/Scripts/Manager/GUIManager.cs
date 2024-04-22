using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SCN.Common;
using SCN.Tutorial;
using SCN;

public class GUIManager : MonoBehaviour
{
    [SerializeField] bool isTest;
    public static GUIManager instance;
    public ParticleSystem confettiFx1;
    public ParticleSystem confettiFx2;
    public ParticleSystem rainbowFx;
    public ParticleSystem starExplore;
    public ParticleSystem starExplore2;
    public ParticleSystem touchConfettiFx;
    public ParticleSystem lightingFx;

    [Header("----------------- PROPERTISE -----------------")]
    [SerializeField] Balloon balloonPb;
    [SerializeField] RectTransform balloonZone;
    [SerializeField] CharacterAnimation wolfooAnimation;
    [SerializeField] Image blackPanel;
    [SerializeField] Transform wolfooZone;
    [SerializeField] List<CharacterAnimation> characters;
    [SerializeField] List<Sprite> ballSprites;
    [SerializeField] Transform startWolfooZone;
    [SerializeField] Transform spawnModeAlphaZone;
    [SerializeField] Transform congratZone;
    public Image whitPanel;
    [SerializeField] Canvas canvasFx;

    [Header("----------------- PANEL -----------------")]
    public HUDSystem canvas;
    public AdsPanel adsPanel;
    public SettingPanel settingPanel;
    public LoadingPanel loadingPanel;
    public IntroPanel introPanel;
    public NoAdsPanel noAdsPanel;
    public GiftPanel giftPanel;
    public HomePanel homePanel;
    public IAPPanel iAPPanel;
    public GameObject homeUI;
    public Panel losePanel;
    public TutorialPanel tutorialPanel;
    public MainPanel mainPanel;
    public Room1 room1;
    public Room2 room2;
    public Room3 room3;

    [Header("----------------- MODE -----------------")]
    [SerializeField] AlphaLearningMode alphaLearningMode;
    [SerializeField] NumberMode numberMode;
    [SerializeField] GotoSchoolMode gotoSchoolMode;
    [SerializeField] FruitDecorMode fruitDecorMode;
    [SerializeField] ShapeMode shapeMode;
    [SerializeField] ClayMode clayMode;
    [SerializeField] GalaxyMode galaxyMode;

    int totalBalloon = 20;
    bool isOnBack;
    bool isEndgame;

    List<Balloon> balloons = new List<Balloon>();
    private float ratio;
    private Tweener punchTween;
    private GameObject curMode;
    private GameObject curItem;
    private RandomNoRepeat<Sprite> rdNoRp;
    private PanelType curModeType;
    public ShapeMode curShapeMode;
    private bool canClick;
    private Tween delayTween;
    private Vector3 startRainbowScale;
    private Transform curTrans;
    private Tweener moveTween;
    private GameObject curPanel;
    private GameObject curGround;
    private Panel curModePanel;
    private PanelType nextPanelType;
    private Canvas canvasHUD;

    public GameObject CurMode { get => curMode; }
    public Image BlackPanel { get => blackPanel; }
    public PanelType CurModeType { get => curModeType; }
    public Transform SpawnModeAlphaZone { get => spawnModeAlphaZone; }
    public GameObject CurPanel { get => curPanel; }
    public GameObject CurGround { get => curGround; }

    private void Awake()
    {
        if (instance == null) instance = this;
        startRainbowScale = rainbowFx.transform.localScale;
    }

    private void Start()
    {
        Application.targetFrameRate = 120;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        EventManager.OnEndgame += PlayEndgame;
        EventManager.OnBackPanel += Onback;
        EventManager.OnCompleteOpenGift += OpenPausePanel;
        EventManager.OpenPanel += OpenPanel;
        EventManager.OnPlaygame += GetPlaygame;

        InitBalloon();
        InitWolfooAndFriends();

        if(!isTest)
             HUDSystem.Instance.Show<IntroPanel>(null, UIPanels<HUDSystem>.ShowType.Duplicate);
        //    HUDSystem.Instance.Show<>

        HUDSystem.Instance.Show<AdsPanel>(null, UIPanels<HUDSystem>.ShowType.Duplicate);

    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (!canClick) return;
            if (delayTween != null) delayTween?.Kill();
            OnOffCongratulate();

            //GameManager.instance.GetCurrentPosition(touchConfettiFx.transform);
            //touchConfettiFx.transform.SetAsLastSibling();
            //touchConfettiFx.Play();
        }
    }

    private void OnDestroy()
    {
        EventManager.OnEndgame -= PlayEndgame;
        EventManager.OnBackPanel -= Onback;
        EventManager.OnCompleteOpenGift -= OpenPausePanel;
        EventManager.OpenPanel -= OpenPanel;
        EventManager.OnPlaygame -= GetPlaygame;

        if (moveTween != null) moveTween?.Kill();
        if (delayTween != null) delayTween?.Kill();
    }

    public void AssignPanel(GameObject _curPanel,GameObject _curGround)
    {
        curPanel = _curPanel;
        curGround = _curGround;
    }
    public void AssignModePanel(Panel _curModePanel, PanelType _nextPanelType)
    {
        curModePanel = _curModePanel;
        nextPanelType = _nextPanelType;
    }

    public void OnCongratulate(Transform _trans, Vector3 _endPos)
    {
        blackPanel.gameObject.SetActive(true);

        congratZone.position = _trans.position;

        rainbowFx.transform.position = _trans.position;
        rainbowFx.transform.SetParent(congratZone);
        rainbowFx.transform.localScale = Vector3.one * 700;
        rainbowFx.Play();
        _trans.SetParent(congratZone);
        curTrans = _trans;
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Wow);

        moveTween = congratZone.DOMove(_endPos, 1)
            .OnComplete(() =>
            {
                canClick = true;
                delayTween = DOVirtual.DelayedCall(5, () =>
                {
                    OnOffCongratulate();
                });
            });
    }
    void OnOffCongratulate()
    {
        canClick = false;
        blackPanel.gameObject.SetActive(false);
        rainbowFx.Stop();
        rainbowFx.transform.SetParent(canvasFx.transform);
        rainbowFx.transform.localScale = startRainbowScale;
   //     curTrans.SetParent(transform);
        EventDispatcher.Instance.Dispatch(new EventKey.OnClickItem { gui = this });
    }

    private void GetPlaygame(Panel parentPanel, PanelType nextMode)
    {
        curModeType = nextMode;
        parentPanel.Hide();
        switch (nextMode)
        {
            case PanelType.NumberMode:
                Instantiate(numberMode, canvas.transform);
                break;
            case PanelType.ShapeMode:
                Instantiate(shapeMode, canvas.transform);
                break;
            case PanelType.GotoSchoolMode:
                Instantiate(gotoSchoolMode, canvas.transform);
                break;
            case PanelType.AlphaLearningMode:
                Instantiate(alphaLearningMode, canvas.transform);
                break;
            case PanelType.FruitMode:
                Instantiate(fruitDecorMode, canvas.transform);
                break;
            case PanelType.ClayMode:
                Instantiate(clayMode, canvas.transform);
                break;
            case PanelType.GalaxyMode:
                Instantiate(galaxyMode, canvas.transform);
                break;
        }
    }

    public GameObject GetCurContent()
    {
        if(mainPanel != null && mainPanel.gameObject.activeSelf)
        {
            return mainPanel.ScrollRect.content.gameObject;
        }
        if(room1 != null && room1.gameObject.activeSelf)
        {
            return room1.ScrollRect.content.gameObject;
        }
        if(room2 != null && room2.gameObject.activeSelf)
        {
            return room2.ScrollRect.content.gameObject;
        }
        if(room3 != null && room3.gameObject.activeSelf)
        {
            return room3.ScrollRect.content.gameObject;
        }
        return null;
    }
    public GameObject GetCurGround()
    {
        if(mainPanel != null && mainPanel.gameObject.activeSelf)
        {
            return mainPanel.GroundTrans.gameObject;
        }
        if(room1 != null && room1.gameObject.activeSelf)
        {
            return room1.GroundTrans.gameObject;
        }
        if(room2 != null && room2.gameObject.activeSelf)
        {
            return room2.GroundTrans.gameObject;
        }
        if (room3 != null && room3.gameObject.activeSelf)
        {
            return room3.GroundTrans.gameObject;
        }
        return null;
    }

    public void ScaleImage(Image item, float width, float height)
    {
        item.SetNativeSize();
        if (item.rectTransform.rect.height > item.rectTransform.rect.width)
        {
            ratio = item.rectTransform.rect.height / item.rectTransform.rect.width; // Scale with Max Height
            width = height / ratio;
            item.rectTransform.sizeDelta = new Vector2(width, height);
        }
        else
        {
            ratio = item.rectTransform.rect.width / item.rectTransform.rect.height;  // Scale with Max Width
            height = width / ratio;
            item.rectTransform.sizeDelta = new Vector2(width, height);
        }
    }

    public void GetLoseGame(GameObject _gameObject, PanelType panelType)
    {
        var losePanel_ = Instantiate(losePanel, _gameObject.transform);
        delayTween = DOVirtual.DelayedCall(2, () =>
        {
            Destroy(_gameObject);
            Destroy(losePanel_);
            OpenPanel(panelType);
        });
    }

    void Onback(Panel _panel, PanelType panelType, bool isDestroy = true)
    {
        TutorialManager.Instance.Stop();
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Click);

        FirebaseManager.instance.LogWatchAds("Inters");
        if (isDestroy)
        {
            Destroy(_panel.gameObject);
        }
        else
        {
            _panel.Hide();
        }
        OpenPanel(panelType);
    }

    void OnEndgame(System.Action OnComplete = null)
    {
        isEndgame = true;
       // SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Hooray, CharacterAnimation.SexType.Boy);
        blackPanel.DOFade(0.3f, 0f);
        PlayBalloon();
        WolfooRunToScreen();
        confettiFx1.gameObject.SetActive(true);
        confettiFx2.gameObject.SetActive(true);
        confettiFx1.transform.SetAsLastSibling();
        confettiFx2.transform.SetAsLastSibling();
        confettiFx1.Play();
        confettiFx2.Play();

        DOVirtual.DelayedCall(5, () =>
        {
            blackPanel.gameObject.SetActive(false);
            foreach (var item in characters)
            {
                item.transform.position = new Vector2(startWolfooZone.position.x, item.transform.position.y);
            }
            OnComplete?.Invoke();
        });
    }
    public void OnPutItem()
    {
        punchTween.Kill();
    }

    public void PlayEndgame(GameObject _gameobject, PanelType panelType, bool isDestroy, GameObject item = null)
    {
        blackPanel.transform.SetAsLastSibling();
        blackPanel.gameObject.SetActive(true);

        curMode = _gameobject;
        curItem = item;

        TutorialManager.Instance.Stop();

        FirebaseManager.instance.LogEndMode(_gameobject.name);
        OnEndgame(() =>
        {
            FirebaseManager.instance.LogWatchAds("Inters");
            OpenPanel(panelType);
            Destroy(_gameobject);
        });
    }

    void OpenPausePanel(bool isAtHome)
    {
        if (isAtHome || !isEndgame) return;

        isEndgame = false;
    }

    public void PlayLoading(System.Action OnComplete = null)
    {
        loadingPanel.Open(null, OnComplete);
    }

    public void PlayLighting(Transform _endTrans)
    {
        lightingFx.transform.position = _endTrans.position;
        lightingFx.Play();
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
    }
    public void PLayStar(Transform _endTrans)
    {
        starExplore.transform.position = _endTrans.position;
        starExplore.Play();
    }
    public void PLayStar2(Transform _endTrans)
    {
        starExplore2.transform.position = _endTrans.position;
        starExplore2.Play();
    }

    void InitWolfooAndFriends()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].transform.position = new Vector2(startWolfooZone.position.x, characters[i].transform.position.y);
            characters[i].PlayIdle();
        }
    }
    void WolfooRunToScreen()
    {
        wolfooZone.transform.SetAsLastSibling();
        for (int i = 0; i < characters.Count; i++)
        {
            var index = i;
            characters[index].transform.rotation = Quaternion.Euler(Vector3.up * 180);
            characters[index].PlayMove();
            characters[index].transform.DOMoveX(wolfooZone.GetChild(0).GetChild(index).position.x, 2)
            .OnComplete(() =>
            {
                characters[index].PlaySpecial();
            });
        }
    }

    public void WolfooRunToSad(System.Action OnComplete = null)
    {
        wolfooZone.transform.SetAsLastSibling();
        int rd = Random.Range(0, characters.Count);
        characters[rd].transform.position = Vector3.up * -3;
        characters[rd].PlaySad();

        DOVirtual.DelayedCall(3, () =>
        {
            OnComplete?.Invoke();
            characters[rd].transform.position = wolfooZone.GetChild(0).position;
        });
    }
    void InitBalloon()
    {
        float width = balloonPb.GetComponent<RectTransform>().rect.width;
        float xPos;

        if (ballSprites.Count != 0)
        {
            rdNoRp = new RandomNoRepeat<Sprite>(ballSprites);
        }
        for (int i = 0; i < totalBalloon; i++)
        {
            xPos = Random.Range(-balloonZone.rect.width / 2 + width / 2, balloonZone.rect.width / 2 - width / 2);
            var balloon = Instantiate(balloonPb, balloonZone.transform);
            balloon.transform.localPosition = new Vector2(xPos, balloonZone.rect.y);

            if (ballSprites.Count != 0)
            {
                balloon.ImageBall.sprite = rdNoRp.Random();
                ScaleImage(balloon.ImageBall,
                    balloonPb.ImageBall.rectTransform.rect.width,
                    balloonPb.ImageBall.rectTransform.rect.height);
                balloons.Add(balloon);
            }
        }
    }

    public void PlayBalloon(System.Action action = null)
    {
        balloonZone.SetAsLastSibling();
        foreach (var item in balloons)
        {
            item.transform.localPosition = Vector2.zero;
            item.OnAnimation();
            item.gameObject.SetActive(true);
        }
    }

    public void PlaySuccessDrag(Transform trans, System.Action action = null)
    {
        var star = Instantiate(starExplore, trans.parent);

        star.transform.position = trans.position;
        star.gameObject.SetActive(true);
      //  star.Play();

        if (action != null)
        {
            DOVirtual.DelayedCall(star.main.duration/2, () =>
            {
                action?.Invoke();
            });
        }
    }

    public void SetLayerHUD(int order)
    {
        if(canvasHUD == null)
            canvasHUD = canvas.GetComponent<Canvas>();
        canvasHUD.sortingOrder = order;
    }

    public void OpenPanel(PanelType panelType)
    {
        switch (panelType)
        {
            case PanelType.Ads:
                FirebaseManager.instance.LogOpenPanel("AD");
                HUDSystem.Instance.Show<AdsPanel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                break;
            case PanelType.Gift:
                // FirebaseManager.instance.LogOpenPanel("Gift");
                break;
            case PanelType.Back:
                break;
            case PanelType.Setting:
                FirebaseManager.instance.LogOpenPanel("Cai_dat");
                var settingPanel = HUDSystem.Instance.Show<SettingPanel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                settingPanel.transform.SetAsLastSibling();
                break;
            case PanelType.Sticker:
                FirebaseManager.instance.LogOpenPanel("Sticker");
                break;
            case PanelType.Intro:
                FirebaseManager.instance.LogOpenPanel("Intro");
                Instantiate(introPanel, canvas.transform);
                break;
            case PanelType.NoAds:
                FirebaseManager.instance.LogOpenPanel("No_AD");
                var noAdsPanel = HUDSystem.Instance.Show<NoAdsPanel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                noAdsPanel.transform.SetAsLastSibling();
                break;
            case PanelType.IAP:
                FirebaseManager.instance.LogOpenPanel("IAP");
                HUDSystem.Instance.Show<IAPPanel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                break;
            case PanelType.Lose:
                FirebaseManager.instance.LogOpenPanel("Lose");
                if(losePanel == null)
                    losePanel = HUDSystem.Instance.Show<LosePanel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                break;
            case PanelType.Room1:
                FirebaseManager.instance.LogOpenPanel("Room1");
                if (room1 == null)
                    room1 = HUDSystem.Instance.Show<Room1>(null, UIPanels<HUDSystem>.ShowType.PauseCurrent).GetComponent<Room1>();
                else 
                    HUDSystem.Instance.Show<Room1>(null, UIPanels<HUDSystem>.ShowType.PauseCurrent).GetComponent<Room1>();
                break;
            case PanelType.Main:
                FirebaseManager.instance.LogOpenPanel("Main");
                if(mainPanel == null)
                {
                    mainPanel = HUDSystem.Instance.Show<MainPanel>(null, UIPanels<HUDSystem>.ShowType.DissmissCurrent).GetComponent<MainPanel>();
                    if (DataMainManager.instance.LocalDataStorage.FirstOpen)
                    {
                        EventManager.OnPlaygame?.Invoke(mainPanel, PanelType.GotoSchoolMode);
                        DataMainManager.instance.LocalDataStorage.FirstOpen = false;
                        DataMainManager.instance.SaveItem(DataMainManager.StorageKey.Data);
                    }
                }
                else
                    HUDSystem.Instance.Show<MainPanel>(null, UIPanels<HUDSystem>.ShowType.DissmissCurrent).GetComponent<MainPanel>();
                break;
            case PanelType.Room2:
                FirebaseManager.instance.LogOpenPanel("Room2");
                if(room2 == null)
                    room2 = HUDSystem.Instance.Show<Room2>(null, UIPanels<HUDSystem>.ShowType.PauseCurrent).GetComponent<Room2>();
                else
                    HUDSystem.Instance.Show<Room2>(null, UIPanels<HUDSystem>.ShowType.PauseCurrent).GetComponent<Room2>();
                break;
            case PanelType.Room3:
                FirebaseManager.instance.LogOpenPanel("Room3");
                if(room3 == null)
                    room3 = HUDSystem.Instance.Show<Room3>(null, UIPanels<HUDSystem>.ShowType.PauseCurrent).GetComponent<Room3>();
                else
                    HUDSystem.Instance.Show<Room3>(null, UIPanels<HUDSystem>.ShowType.PauseCurrent).GetComponent<Room3>();
                break;
            case PanelType.PLayer:
                HUDSystem.Instance.Show<PlayerPanel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                break;
            case PanelType.BuyFailPanel:
                FirebaseManager.instance.LogOpenPanel("Khong_du_Coin");
                HUDSystem.Instance.Show<BuyFailPanel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                break;
            case PanelType.CommingSoon:
                FirebaseManager.instance.LogOpenPanel("Comming_Soon");
                HUDSystem.Instance.Show<CommingSoonPanel>(null, UIPanels<HUDSystem>.ShowType.KeepCurrent);
                break;
        }
    }
}

public enum PanelType
{
    None,
    Ads,
    Gift,
    Back,
    Sticker,
    Setting,
    Intro,
    NoAds,
    Home,
    IAP,
    Lose,
    Tutorial,
    Room1,
    Main,
    Room2,
    Room3,
    NumberMode,
    ShapeMode,
    GotoSchoolMode,
    AlphaLearningMode,
    SessionChooseShape,
    SessionCutItem,
    GalaxyMode,
    FruitMode,
    PLayer,
    BuyFailPanel,
    CommingSoon,
    ClayMode,
}