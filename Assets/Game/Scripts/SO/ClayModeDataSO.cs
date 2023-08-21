using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN.ActionLib;
using UnityEngine.UI;

[CreateAssetMenu(fileName = Const.CLAY_DATA_SO, menuName = "Scriptable Objects/" + Const.CLAY_DATA_SO)]
public class ClayModeDataSO : ScriptableObject
{
    public Sprite[] traySprites;
    public Sprite[] lidSprites;
    public ShapeFlowerData[] shapeFlowerItems;
    public StampedClay[] stampedClayPbs;
    public FlowerData[] flowerItems;
    public Sprite[] potSprites;
    public Sprite[] potMaskSprites;
    public Vector2[] localMaskPos;
    public Vector2[] localFlowerPos;
}
[System.Serializable]
public struct ShapeFlowerData
{
    public Sprite[] sprites; 
}
[System.Serializable]
public struct FlowerData
{
    public Sprite[] trunkSprites; 
    public Sprite[] leafSprites; 
    public Sprite[] petalSprites;
    public Sprite pistilSprite;
    public Sprite[] colorPetalSprites;
    public Sprite colorPistilSprite;
    public Flower flowerPb;
}
