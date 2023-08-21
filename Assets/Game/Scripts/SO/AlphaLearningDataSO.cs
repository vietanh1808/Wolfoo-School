using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN.ActionLib;

[CreateAssetMenu(fileName = Const.ALPHA_LEARNING_DATA_SO, menuName = "Scriptable Objects/" + Const.ALPHA_LEARNING_DATA_SO)]
public class AlphaLearningDataSO : ScriptableObject
{
    public List<Sprite> uppercaseSprites;
    public List<Sprite> lowercaseSprites;
    public List<GameObject> uppercaseObjs;
    public List<GameObject> lowercaseObjs;
}
