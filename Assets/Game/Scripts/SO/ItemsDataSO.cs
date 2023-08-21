using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = Const.ITEM_DATA_SO, menuName = "Scriptable Objects/" + Const.ITEM_DATA_SO)]
public class ItemsDataSO : ScriptableObject
{
    public List<Sprite> obstacleSprites;
    public List<CharacterAnimation> characters;
    public List<Sprite> eyeSprites;
    public List<Sprite> handSprites;
    public List<Sprite> mouthSprites;
    public List<Sprite> fruitSprites;
    public List<GameObject> eyeAnimations;
    public List<Sprite> shapeBoards;
    public List<Sprite> shapeGlocery;
    public List<PriceItem> charactersPrice;
}
