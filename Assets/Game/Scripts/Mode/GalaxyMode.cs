using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;
using SCN.Tutorial;

public class GalaxyMode : Panel, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ObstacleGalaxy obstaclePb;
    [SerializeField] List<Image> backgroundImgs;
    [SerializeField] RectTransform bgMove;
    [SerializeField] float velocity;
    [SerializeField] Image player;
    [SerializeField] float timeToPlay;
    [SerializeField] Text scoreTxt;
    [SerializeField] ScorePanel scorePanelPb;
    [SerializeField] ParticleSystem smokeFx;
    [SerializeField] ParticleSystem starFx;

    int mainIdx;
    float range;
    int count = 0;
    int nextIdx = 0;
    private int rdOtherIdx;
    private int rd;
    bool isPaused = true;
    int score = 0;
    float lastLocalXPos;
    float countTime = 0;

    ScorePanel scorePanel;
    ShapeModeDataSO data;
    private int curShapeIdx;
    List<ObstacleGalaxy> obstacles = new List<ObstacleGalaxy>();
    private int curIdxColor;
    private int countCollision = 0;
    private int rdColor;
    private Vector3 playerPos;
    bool canClick;

    private void Awake()
    {
        // GameManager.instance.spawnPlayerPos = player.transform.position;
        playerPos = player.transform.position;
        player.gameObject.SetActive(false);

        scorePanel = Instantiate(scorePanelPb, transform);
    }

    private void Start()
    {
            FirebaseManager.instance.LogBeginMode(gameObject.name);
        EventManager.OnCollisionObstacleGalaxy += GetCollision;
        EventManager.OnCompleteMove += GetCompleteMoveObject;

        GameManager.instance.GetDataSO(DataSOType.Shape);
        data = GameManager.instance.ShapeDataSO;
        curShapeIdx = GameManager.instance.curShapeIdx;
        curIdxColor = GameManager.instance.curIdxColor;
        lastLocalXPos = player.transform.localPosition.x;

        range = bgMove.rect.width;
        GetNextPos();
        InitObstacle();


        mainIdx = curShapeIdx;
        player.sprite = data.shapeColors[curShapeIdx].blockSprites[curIdxColor];
        player.SetNativeSize();

        TutorialManager.Instance.StartPointer(player.transform);
    }

    private void OnDestroy()
    {
        EventManager.OnCollisionObstacleGalaxy -= GetCollision;
        EventManager.OnCompleteMove -= GetCompleteMoveObject;
    }

    private void Update()
    {
        if (isPaused) return;

        countTime += Time.deltaTime;
        if (countTime >= timeToPlay)
        {
            isPaused = true;
            OnEndgame();
            return;
        }

        bgMove.position += Vector3.right * velocity * 0.01f;
        if(transform.position.x < backgroundImgs[nextIdx].transform.position.x)
        {
            GetNextPos();
        }
    }

    private void GetCompleteMoveObject(GameObject obj)
    {
        obj.transform.DOMove(playerPos, 1).OnComplete(() =>
        {
            obj.SetActive(false);
            player.gameObject.SetActive(true);
            isPaused = false;
            canClick = true;
        });
    }

    private void GetCollision(int idx, ObstacleGalaxy obstacle)
    {
        if (!canClick) return;
        if (isPaused) return;

        countCollision++;
        if(countCollision % obstacles.Count == 0)
        {
            SetupNextObstacle();
        }

        if(idx != mainIdx)
        {
            isPaused = true;
            OnFail();
            obstacle.OnFail();
            scorePanel.OnCountDown(() =>
            {
                SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Sad);
                isPaused = false;

                player.sprite = data.shapeColors[curShapeIdx].blockSprites[curIdxColor];
                player.SetNativeSize();
            }, 
            () =>
            {
               OnEndgame(true);
            });
        }
        else
        {
            OnSucess();
            obstacle.OnSucess();
            score++;
            scoreTxt.text = score + "";
        }
    }
    void OnSucess()
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Correct);
        starFx.Play();
    }
    void OnFail()
    {
        SoundManager.instance.PlayOtherSfx(SfxOtherType.Collision);
        smokeFx.Play();
        Vibration.Vibrate(500);
    }

    void OnEndgame(bool isLose = false)
    {
        TutorialManager.Instance.Stop();
        GameManager.instance.totalStar = score;
        if(isLose)
        {
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Disagree);
            EventManager.OnLose?.Invoke();
            EventManager.OnEndSession?.Invoke();
        }
        else
        {
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Congratulation);
            EventManager.OnEndSession?.Invoke();
        }
    }

    private void InitObstacle()
    {
        rdOtherIdx = UnityEngine.Random.Range(0, data.emptyBlockSprites.Count);
        while (rdOtherIdx == curShapeIdx)
        {
            rdOtherIdx = UnityEngine.Random.Range(0, data.emptyBlockSprites.Count);
        }

        rdColor = UnityEngine.Random.Range(0, data.shapeColors[0].blockSprites.Count);
        while (rdColor == curIdxColor)
        {
            rdColor = UnityEngine.Random.Range(0, data.shapeColors[0].blockSprites.Count);
        }
        // rdcolor = 5
        // curidxcolor = 1
        // curidx = 1
        // rdOtherIdx = 4
        // rd = 0
        for (int i = 0; i < 7; i++)
        {
            rd = UnityEngine.Random.Range(0, 2);
            var obstacle = Instantiate(obstaclePb, bgMove);
            if (rd == 0)
            {
                obstacle.AssignItem(rdOtherIdx,
                    data.shapeColors[rdOtherIdx].blockSprites[rdColor]);
            }
            else
            {
                obstacle.AssignItem(curShapeIdx,
                    data.shapeColors[curShapeIdx].blockSprites[curIdxColor]);
            }
            obstacle.transform.localPosition =
                new Vector3(lastLocalXPos, 0, 0);
            lastLocalXPos -= 2300;
            obstacles.Add(obstacle);
        }
    }

    void SetupNextObstacle()
    {
        for (int i = 0; i < obstacles.Count; i++)
        {
            rd = UnityEngine.Random.Range(0, 2);
            if (rd == 0)
            {
                obstacles[i].AssignItem(rdOtherIdx,
                    data.shapeColors[rdOtherIdx].blockSprites[rdColor]);
            }
            else
            {
                obstacles[i].AssignItem(curShapeIdx,
                    data.shapeColors[curShapeIdx].blockSprites[curIdxColor]);
            }
            obstacles[i].transform.localPosition =
                new Vector3(lastLocalXPos, 0, 0);
            lastLocalXPos -= 2500;
        }
    }

    void GetNextPos()
    {
        count++;
        nextIdx = nextIdx == 0 ? 1 : 0;
        if (count <= 1) return;
        backgroundImgs[nextIdx].transform.localPosition -= Vector3.right * range * 2;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canClick) return;
        if (isPaused) return;
        mainIdx = rdOtherIdx;
        player.sprite = data.shapeColors[rdOtherIdx].blockSprites[rdColor];
        player.SetNativeSize();
        TutorialManager.Instance.Stop();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canClick) return;
        if (isPaused) return;
        mainIdx = curShapeIdx;
        player.sprite = data.shapeColors[curShapeIdx].blockSprites[curIdxColor];
        player.SetNativeSize();
    }
}
