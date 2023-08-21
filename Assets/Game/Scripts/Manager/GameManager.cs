using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int curFlipIdx;
    public bool isCompare;
    public int curChooseItemIdx;
    public int curIdxMode;
    public GameObject curMode;
    private float ratio;
    public int curShapeIdx;
    public int curIdxColor;
    public DataSOType curModeType;
    public int curIdxSession = 0;
    public int curAlphaIdx;
    public int totalStar;
    public CaculateType curCaculate;
    public int curRange;
    public int lastSiblingIndex;
    public int countId = 0;
    public MainPanel mainPanel;
    public bool isChooseCharacerOpen;
    public Transform ChooseCharacterZone;

    List<int> idxCakeItems = new List<int>();

    private Vector3 newpos;
    private Vector3 pos;
    private Vector3 curPosition;
    private ItemsDataSO itemDataSO;
    private ShapeModeDataSO shapeDataSO;
    private AlphaLearningDataSO alphaLearningDataSO;
    private GameDataSO gameDataSO;
    private ClayModeDataSO clayDataSO;

    public ItemsDataSO ItemDataSO { get => itemDataSO; }
    public ShapeModeDataSO ShapeDataSO { get => shapeDataSO; }
    public AlphaLearningDataSO AlphaLearningDataSO { get => alphaLearningDataSO; }
    public GameDataSO GameDataSO { get => gameDataSO; }
    public ClayModeDataSO ClayDataSO { get => clayDataSO; }

    private void Awake()
    {
        if(instance == null)
        instance = this;
    }
    public void GetDataSO(DataSOType type)
    {
        curModeType = type;
        switch (type)
        {
            case DataSOType.Items:
                if (itemDataSO == null)
                    itemDataSO = Resources.Load<ItemsDataSO>(Const.ITEM_DATA_SO);
                break;
            case DataSOType.Shape:
                if (shapeDataSO == null)
                    shapeDataSO = Resources.Load<ShapeModeDataSO>(Const.SHAPE_DATA_SO);
                break;
            case DataSOType.AlphaLearning:
                if (alphaLearningDataSO == null)
                    alphaLearningDataSO = Resources.Load<AlphaLearningDataSO>(Const.ALPHA_LEARNING_DATA_SO);
                break;
            case DataSOType.GameDataSO:
                if (gameDataSO == null)
                    gameDataSO = Resources.Load<GameDataSO>(Const.GAME_DATA_SO);
                break;
            case DataSOType.ClayMode:
                if (clayDataSO == null)
                    clayDataSO = Resources.Load<ClayModeDataSO>(Const.CLAY_DATA_SO);
                break;
        }
    }
    private void Start()
    {
        //  StartCoroutine(IAPManager.Instance.IEStart());
        Input.multiTouchEnabled = false;
    }
    private void OnDestroy()
    {
    }

    public int GetCoin(SkinBackItemType skinType)
    {
        if (itemDataSO == null) GetDataSO(DataSOType.Items);

        for (int i = 0; i < itemDataSO.charactersPrice.Count; i++)
        {
            if (itemDataSO.charactersPrice[i].skinType == skinType)
            {
                return itemDataSO.charactersPrice[i].price;
            }
        }

        return 0;
    }
    public int GetCoin(int idx)
    {
        if (itemDataSO == null) GetDataSO(DataSOType.Items);

        return itemDataSO.charactersPrice[idx].price;
    }
    public void GetCurrentPosition(Transform _transform)
    {
        curPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        curPosition.z = 0;
        _transform.position = curPosition;
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
    public void CheckPos(Transform transform)
    {
        pos = Camera.main.WorldToScreenPoint(transform.position);
        if (pos.x > (Screen.safeArea.xMax))
        {
            newpos = new Vector3(Screen.safeArea.xMax, pos.y, pos.z);
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(newpos).x, transform.position.y, transform.position.z);
        }
        if (pos.x < Screen.safeArea.xMin)
        {
            newpos = new Vector3(Screen.safeArea.xMin, pos.y, pos.z);
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(newpos).x, transform.position.y, transform.position.z);
        }
        if (pos.y > Screen.safeArea.yMax)
        {
            newpos = new Vector3(pos.x, Screen.safeArea.yMax, pos.z);
            transform.position = new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(newpos).y, transform.position.z);
        }
        if (pos.y < Screen.safeArea.yMin)
        {
            newpos = new Vector3(pos.x, Screen.safeArea.yMin, pos.z);
            transform.position = new Vector3(transform.position.x, Camera.main.ScreenToWorldPoint(newpos).y + 0.5f, transform.position.z);
        }
    }
}
public enum DataSOType
{
    Items,
    Shape,
    AlphaLearning,
    ClayMode,
    GameDataSO,
}
public enum SkinBackItemType
{
    None,
    Bag,
    Door,
    Wall,
    Carpet,
    Floor,
    Character
}

[System.Serializable]
public struct PriceItem
{
    public SkinBackItemType skinType;
    public int price;
}

