using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN;

public class SessionJigsaw : MonoBehaviour
{
    [Header("Content Board")]
    [SerializeField] Transform board;
    [SerializeField] Transform posBoard;
    [SerializeField] private GameObject selectLevelGO;
    [SerializeField] private ButtonBase ButtonJigsawAlphabet;
    [SerializeField] private ButtonBase ButtonJigsawPicture;
    [SerializeField] private ButtonBase ButtonCloseJigsaw;
    [SerializeField] private ButtonBase ButtonCloseJigsawLevels;
    [SerializeField] private LevelManager levelJigsawPrefab;
    [Header("Content PickLevel")]
    [SerializeField] private GameObject groupPickLevel;
    [SerializeField] private GameObject groupLevels;
    [SerializeField] private Transform contentLevels;
    [SerializeField] private ItemLevelJigsaw itemLevelJigsawPrefab;
    [SerializeField] private List<ItemLevelJigsaw> listLevels;
    private LevelManager levelJigsaw;
    private Vector3 startPosBoard;
    private List<int> listLockAdsAlphabet = new List<int>();
    private List<int> listLockAdsPicture = new List<int>();
    private List<int> listOwnedAlphabet = new List<int>();
    private List<int> listOwnedPicture = new List<int>();
    GameDataSO gameDataSO;
    Tween tweenBoard;
    private void Start()
    {
        GameManager.instance.GetDataSO(DataSOType.GameDataSO);
        gameDataSO = GameManager.instance.GameDataSO;

        startPosBoard = board.transform.position;
        ButtonJigsawAlphabet.onClick.AddListener(() => PlayJigsaw(JigsawType.Alphabet));
        ButtonJigsawPicture.onClick.AddListener(() => PlayJigsaw(JigsawType.Picture));
        ButtonCloseJigsaw.onClick.AddListener(OnCloseJigsaw);
        ButtonCloseJigsawLevels.onClick.AddListener(OnCloseJigsawLevels);
        listLevels = new List<ItemLevelJigsaw>();
        EventDispatcher.Instance.RegisterListener<EventKey.PlayLevelJigsaw>(PlayLevelJigsaw);
        EventDispatcher.Instance.RegisterListener<EventKey.OnCloseLevelJigsaw>(OnCloseLevelJigsaw);
        //listLockAdsAlphabet = gameDataSO.ConfigReward[Config.JIGSAW][Config.JIGSAW_ALPHABET].AsListInt;
        //listLockAdsPicture = gameDataSO.ConfigReward[Config.JIGSAW][Config.JIGSAW_PICTURE].AsListInt;
        //listOwnedAlphabet = gameDataSO.CurrentOwned[Config.JIGSAW][Config.JIGSAW_ALPHABET].AsListInt;
        //listOwnedPicture = gameDataSO.CurrentOwned[Config.JIGSAW][Config.JIGSAW_PICTURE].AsListInt;
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveListener<EventKey.PlayLevelJigsaw>(PlayLevelJigsaw);
        EventDispatcher.Instance.RemoveListener<EventKey.OnCloseLevelJigsaw>(OnCloseLevelJigsaw);
    }
    private void OnCloseLevelJigsaw() {
        selectLevelGO.SetActive(true);
        board.transform.position = startPosBoard;
        BoardMoveIn(() =>
        {
           
        });
    }
    private void OnCloseJigsaw()
    {
        BoardMoveOut(() =>
        {
            gameObject.SetActive(false);
        });
    }
    private void OnCloseJigsawLevels()
    {
        BoardMoveOut(() =>
        {
            OnPlayJigsaw();
        });
    }
    public void OnPlayJigsaw()
    {
        selectLevelGO.SetActive(true);
        groupPickLevel.SetActive(true);
        groupLevels.SetActive(false);
        DOVirtual.DelayedCall(.1f, () =>
        {
            ButtonJigsawAlphabet.transform.localScale = Vector3.zero;
            ButtonJigsawPicture.transform.localScale = Vector3.zero;
        });
        BoardMoveIn(() =>
        {
            ButtonJigsawAlphabet.transform.DOScale(Vector3.one, .25f);
            ButtonJigsawPicture.transform.DOScale(Vector3.one, .25f);
        });
    }
    private void PlayJigsaw(JigsawType jigsawType)
    {
        BoardMoveOut(() =>
        {
            groupPickLevel.SetActive(false);
            groupLevels.SetActive(true);
            SpawnLevels(jigsawType);
            BoardMoveIn(() =>
            {

            });
        });
        //GameplayGO.SetActive(true);
        //Debug.Log(jigsawType);
        //if (jigsawType == JigsawType.Alphabet)
        //{

        //    levelJigsaw.OnLevelStart(gameDataSO.jigsawData.Alphabet[0]);
        //}
        //else
        //{
        //    levelJigsaw.OnLevelStart(gameDataSO.jigsawData.Picture[0]);
        //}
    }
    private void PlayLevelJigsaw(EventKey.PlayLevelJigsaw data)
    {
        selectLevelGO.SetActive(false);
        if (levelJigsaw != null) Destroy(levelJigsaw.gameObject);
        if (data.jigsawType == JigsawType.Alphabet)
        {
            levelJigsaw = Instantiate(levelJigsawPrefab, transform);
            //levelJigsaw.OnLevelStart(gameDataSO.jigsawData.Alphabet[data.index]);
          //  FirebaseManager.Instance.PlayJigsawAlphabet(data.index);
        }
        else
        {
            levelJigsaw = Instantiate(levelJigsawPrefab, transform);
            //levelJigsaw.OnLevelStart(gameDataSO.jigsawData.Picture[data.index]);
           // FirebaseManager.Instance.PlayJigsawPicture(data.index);
        }
       // FirebaseManager.Instance.PlayStep("Jigsaw");
    }
    private void SpawnLevels(JigsawType jigsawType)
    {
        foreach (var item in listLevels)
        {
            item.gameObject.SetActive(false);
        }
        if (jigsawType == JigsawType.Alphabet)
        {
            for (int i = 0; i < gameDataSO.jigsawData.Alphabet.Count; i++)
            {
            //    var isLockAds = gameDataSO.ConfigReward[Config.IS_CONFIG].AsBool ? listLockAdsAlphabet.Contains(i) && listOwnedAlphabet.Contains(i) : false;
                var item = i >= listLevels.Count ? Instantiate(itemLevelJigsawPrefab, contentLevels) : listLevels[i];
                item.InitItem(i, gameDataSO.jigsawData.Alphabet[i], jigsawType);
            //    item.SetLockAds(isLockAds);
                item.gameObject.SetActive(true);
                if (i >= listLevels.Count) listLevels.Add(item);
            }
        }
        else
        {
            for (int i = 0; i < gameDataSO.jigsawData.Picture.Count; i++)
            {
            //    var isLockAds = gameDataSO.ConfigReward[Config.IS_CONFIG].AsBool ? listLockAdsPicture.Contains(i) && listOwnedPicture.Contains(i) : false;
                var item = i >= listLevels.Count ? Instantiate(itemLevelJigsawPrefab, contentLevels) : listLevels[i];
                item.InitItem(i, gameDataSO.jigsawData.Picture[i], jigsawType);
            //    item.SetLockAds(isLockAds);
                item.gameObject.SetActive(true);
                if (i >= listLevels.Count) listLevels.Add(item);
            }
        }
    }

    private void BoardMoveIn(Action onComplete)
    {
        tweenBoard?.Kill();
        tweenBoard = board.DOMove(posBoard.position, 0.75f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
    private void BoardMoveOut(Action onComplete)
    {
        tweenBoard?.Kill();
        tweenBoard = board.DOMove(startPosBoard, 0.75f).SetEase(Ease.InBack).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
