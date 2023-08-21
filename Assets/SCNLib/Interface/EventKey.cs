using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN;
public class EventKey : IEventParams
{
    public struct OnDragPiece : IEventParams
    {
        public Quaternion quaternion;
        public int idPiece;
    }
    public struct OnCloseLevelJigsaw : IEventParams
    {
    }
    public struct CheckProgress : IEventParams
    {
        public float Progress;
    }
    public struct DoneDragItemPiece : IEventParams
    {
        public int idPiece;
    }
    public struct PlayLevelJigsaw : IEventParams
    {
        public JigsawType jigsawType;
        public int index;

    }
    public struct ReturnItemPiece : IEventParams 
    {
        public int quaternion; 
        public int idPiece; 
        public int Id3;
    }
    public struct OnBeginDragItem : IEventParams
    {
        public Peeler peeler;
        public LineKnife knife;
        public Petal petal;
        public Trunk trunk;
        public Leaf leaf;
        public PotItem potItem;
    }
    public struct OnDragItem : IEventParams
    {
        public Pistil pistil;
        public float value;

        public Peeler peeler;
        public LineKnife knife;
        public Petal petal;
        public Trunk trunk;
        public Leaf leaf;
    }
    public struct OnEndDragItem : IEventParams
    {
        public Peeler peeler;
        public LineKnife knife;
        public Petal petal;
        public Trunk trunk;
        public Leaf leaf;
        public PotItem potItem;
    }
    public struct OnEndDragBackItem : IEventParams
    {
        public int id;
        public WaterBottle waterBottle;
        public PencilBackItem pencil;
        public Book book;
        public BackItem backItem;
        public Wolfoo wolfoo;
        public Trash trash;
        public CarryItem carryItem;
        public BagItem bag;
    }
    public struct OnBeginDragBackItem : IEventParams
    {
        public int id;
        public WaterBottle waterBottle;
        public PencilBackItem pencil;
        public BackItem backItem;
    }
    public struct OnDragBackItem : IEventParams
    {
        public int id;
        public WaterBottle waterBottle;
        public PencilBackItem pencil;
        public BackItem backItem;
    }
    public struct OnClickBackItem : IEventParams
    {
        public int id;
        public Door door;
        public Printer printer;
    }
    public struct OnClickItem : IEventParams
    {
        public ConfirmBtn confirmBtn;
        public FlowerBox flowerBox;
        public ShapeFlowerItem flowerItem;
        public MagicFan fan;
        public GUIManager gui;
        public int id;
    }
    public struct OnCompleteAll : IEventParams
    {
        public Pistil pistil;
    }
    public struct OnModeComplete : IEventParams
    {
        public Pot flowerPot;
    }
    public struct OnWatchAds : IEventParams
    {
        public int instanceID;
        public int idxItem;
    }
    public struct InitAdsPanel : IEventParams
    {
        public int instanceID;
        public int idxItem;
        public Sprite sprite;
        public string textStr;
    }
}
public enum JigsawType
{
    Alphabet,
    Picture
}
