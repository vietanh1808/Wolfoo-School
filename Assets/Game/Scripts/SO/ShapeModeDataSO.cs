using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN.ActionLib;

[CreateAssetMenu(fileName = Const.SHAPE_DATA_SO, menuName = "Scriptable Objects/" + Const.SHAPE_DATA_SO)]
public class ShapeModeDataSO : ScriptableObject
{
    public List<Sprite> emptyBlockSprites;
    public List<Sprite> emptySliceSprites;
    public List<Sprite> paperColorSprites;
    public List<ShapeColor> shapeColors;
    public List<DragFollowPath> shapeCuts;
}
[System.Serializable]
public struct ShapeColor
{
    public List<Sprite> blockSprites;
    public List<Sprite> sliceSprites;
    public List<Sprite> remainSliceSprites;
}
