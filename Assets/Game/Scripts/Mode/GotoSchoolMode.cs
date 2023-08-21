using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SCN.Tutorial;
using DG.Tweening;

public class GotoSchoolMode : Panel, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] BackgroundMoveItem background;
    [SerializeField] CharacterAnimation wolfoo;
    [SerializeField] Obstacle obstaclePb;
    [SerializeField] Transform touchTrans;
    [SerializeField] List<Transform> roadlanes;
    [SerializeField] Vector2 showXRange;
    [SerializeField] Bot botCharacter;
    [SerializeField] Image loadingBarImg;
    [SerializeField] ParticleSystem starFx;
    [SerializeField] Transform tutTrans;
    [SerializeField] GameObject tutPanelPb;
    [SerializeField] Button backBtn;

    GameObject tutPanel;

    bool isEndgame;
    private Vector3 initTouch;
    private float xMoved;
    private float yMoved;
    private float distance;
    private bool swipedLeft;
    private bool isSwiped;
    int curIdxRoadlane = 2;
    private int lastIdx;
    private ItemsDataSO data;
    float totalBar;
    float curBar;
    Image barFillImg;
    Image iconImg;
    bool isPlayTut;
    private float distance2;
    bool isPaused = true;
    private bool isHinted;

    private void Start()
    {
            FirebaseManager.instance.LogBeginMode(gameObject.name);
        EventManager.OnCollisionObstacle += GetCollision;
        EventManager.OnCompletedRace += OnEndgame;
        EventManager.OnClickTutorial += OnCloseTut;

        backBtn.onClick.AddListener(OnBack);
        GameManager.instance.GetDataSO(DataSOType.Items);
        data = GameManager.instance.ItemDataSO;
        InitData();

        wolfoo.transform.position = roadlanes[curIdxRoadlane].position;
        lastIdx = curIdxRoadlane;
        wolfoo.PlayIdle();

        barFillImg.fillAmount = 0;
        DOVirtual.DelayedCall(1, () =>
        {
            Playgame();
        });
    }

    private void OnBack()
    {
        EventManager.OnBackPanel?.Invoke(this, PanelType.Main, true);
    }

    private void OnDestroy()
    {
        EventManager.OnCollisionObstacle -= GetCollision;
        EventManager.OnCompletedRace -= OnEndgame;
        EventManager.OnClickTutorial -= OnCloseTut;
    }


    private void Update()
    {
        if (isEndgame || isPaused) return;

        // PLay Tutorial
        if (!isPlayTut || !isHinted)
        {
            distance2 = Vector2.Distance(tutTrans.position, wolfoo.transform.position);
            if (distance2 < 4)
            {
                isHinted = true;
                PlayTut();
            }
        }

        barFillImg.fillAmount = 1 - background.GetCurDistance() / background.GetTotalDistance();
        iconImg.transform.localPosition = new Vector3(
            (totalBar * barFillImg.fillAmount) - totalBar / 2,
            iconImg.transform.localPosition.y,
            0);
    }
    private void OnCloseTut()
    {
        Debug.Log("OnClose");
        Playgame();
        TutorialManager.Instance.StopAllCoroutines();
        TutorialManager.Instance.Stop();
    }

    void Playgame()
    {
        isPaused = false;
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Whistle);
        background.MapMove();
        wolfoo.PlayMove();
    }

    void PlayTut()
    {
        isPaused = true;
        tutPanel = Instantiate(tutPanelPb, transform);
        tutPanel.transform.position = wolfoo.transform.position + Vector3.one * 3;
        tutPanel.transform.DOScale(20, 0.5f);
        background.MapPause();
        TutorialManager.Instance.NoReactTime = 1;
        TutorialManager.Instance.StartPointer(wolfoo.transform.position, wolfoo.transform.position + Vector3.up * 3);
    }

    void InitData()
    {
        var rdNoRpSprite = new RandomNoRepeat<Sprite>(data.obstacleSprites);
        var rdNoRpTrans = new RandomNoRepeat<Transform>(roadlanes);
        int i = 0;
        var nextPos = new Vector3(Random.Range(showXRange.x, showXRange.y), roadlanes[curIdxRoadlane].position.y, transform.position.z);
        tutTrans.position = nextPos;

        while (nextPos.x < background.DesinationTrans.position.x - 2)
        {
            i++;
            var rdXPos = Random.Range(showXRange.x, showXRange.y);
            var obstacle = Instantiate(obstaclePb, background.ObjstacleZone);
            obstacle.AssignItem(rdNoRpSprite.Random(), nextPos);
            nextPos = new Vector3(rdXPos + i * 9, rdNoRpTrans.Random().position.y, transform.position.z);
        }

        barFillImg = loadingBarImg.transform.Find("Fill bar").GetComponent<Image>();
        iconImg = loadingBarImg.transform.Find("Icon Player").GetComponent<Image>();
        totalBar = loadingBarImg.rectTransform.rect.width;
        curBar = -totalBar / 2;

        if(botCharacter != null)
            botCharacter.AssignLane(roadlanes);
    }

    void GetCollision(bool isBot)
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Incorrect);
        background.MapPause();
        wolfoo.PlayDizzy();
        wolfoo.transform.position = roadlanes[lastIdx].position;
        if(lastIdx == 0 || lastIdx == 1)
        {
            TutorialManager.Instance.StartPointer(wolfoo.transform.position, wolfoo.transform.position + Vector3.down * 3);
        }else
        {
            TutorialManager.Instance.StartPointer(wolfoo.transform.position, wolfoo.transform.position + Vector3.up * 3);
        }
    }

    private void OnEndgame()
    {
        isEndgame = true;
        starFx.transform.position = iconImg.transform.position;
        starFx.Play();
        background.MapPause();
        wolfoo.PlayHappy();

        SoundManager.instance.PlayOtherSfx(SfxOtherType.Congratulation);
        EventManager.OnEndgame?.Invoke(gameObject, PanelType.Main, true, null);
      //  UnityEditor.EditorApplication.isPlaying = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEndgame) return;
        GameManager.instance.GetCurrentPosition(touchTrans);
        initTouch = touchTrans.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEndgame) return;
        GameManager.instance.GetCurrentPosition(touchTrans);

        if (isSwiped) return;

        xMoved = initTouch.x - touchTrans.position.x;
        yMoved = initTouch.y - touchTrans.position.y;
        distance = Vector2.Distance(initTouch, touchTrans.position);
        swipedLeft = Mathf.Abs(xMoved) > Mathf.Abs(yMoved);

        if(distance > 1)
        {
            isSwiped = true;
            // Swipe Left
            if (swipedLeft && xMoved > 0)
            {
            }
            // Swipe Right
            else if (swipedLeft && xMoved < 0)
            {
            }
            // Swipe Up
            else if (!swipedLeft && yMoved < 0)
            {
                curIdxRoadlane--;
                if (curIdxRoadlane < 0)
                {
                    curIdxRoadlane++;
                    return;
                }
                wolfoo.transform.position = roadlanes[curIdxRoadlane].position;
                wolfoo.PlayMove();
                background.MapMove();

                if (tutPanel != null)
                    tutPanel.SetActive(false);
            }
            // Swipe Down
            else if (!swipedLeft && yMoved > 0)
            {
                curIdxRoadlane++;
                if (curIdxRoadlane == roadlanes.Count)
                {
                    curIdxRoadlane--;
                    return;
                }
                wolfoo.transform.position = roadlanes[curIdxRoadlane].position;
                wolfoo.PlayMove();
                background.MapMove();

                if (tutPanel != null)
                    tutPanel.SetActive(false);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isEndgame) return;
        isSwiped = false;
        lastIdx = curIdxRoadlane;

        if(!background.IsMoved && !isPlayTut)
        {
            background.MapMove();
        }
        isPaused = false;
        isPlayTut = true;
        TutorialManager.Instance.Stop();
    }
}
