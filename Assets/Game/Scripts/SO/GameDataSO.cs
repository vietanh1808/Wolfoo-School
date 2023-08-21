using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using SCN.ActionLib;

[CreateAssetMenu(menuName = "HardData/" + Const.GAME_DATA_SO, fileName = Const.GAME_DATA_SO)]
public class GameDataSO : ScriptableObject
{
    public Sprite[] GiftToysSpr;
    public GiftBox GiftBoxs;
    public Sprite[] Balloons;
    public List<Sprite> Food;
    public List<Sprite> Dessert;
    public List<Sprite> Bibs;
    public List<Sprite> StickerBibs;
    public List<PuzzleData> PuzzleDatas;
    public List<TracingData> Tracing;
    public List<ArrageData> ArrageDatas;
    public List<SkeletonDataAsset> Characters;
    public List<DragFollowPath> LevelsTracing2;
    public List<DataLevelColoring> DataLevelColorings;
    public List<Sprite> plates;
    public JigsawData jigsawData;

    [System.Serializable] public struct GiftBox
    {
        public List<Sprite> FullBox;
        public List<Sprite> BottomBox;
        public List<Sprite> LidBox;
    }
    [System.Serializable]
    public struct DataLevelColoring
    {
        public List<Sprite> spr;
    //    public List<LevelColoring> levels;
    }
    [System.Serializable]
    public struct PuzzleData
    {
        public List<Sprite> BottomPuzzle;
        public List<Sprite> DragPuzzle;
        public List<AudioClip> sound;
    }
    [System.Serializable]
    public struct JigsawData
    {
        public List<Sprite> Alphabet;
        public List<Sprite> Picture;
    }
    [System.Serializable]
    public struct TracingData
    {
        public List<Shape> Shapes;
        public List<Sprite> IconLevel;
        public List<CompoundShape> Names;
    }
    [System.Serializable]
    public struct ArrageData
    {
        public Sprite Cabinet;
        public List<Sprite> itemArrange;
        public Sprite Hand;
    }

}
