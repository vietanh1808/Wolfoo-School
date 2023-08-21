using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeBoardFlower : ItemMove
{
    [SerializeField] Transform itemZone;
    [SerializeField] FlowerBox flowerBoxPb;
    private ClayModeDataSO data;

    private void Start()
    {
        GameManager.instance.GetDataSO(DataSOType.ClayMode);
        data = GameManager.instance.ClayDataSO;

        var length = itemZone.childCount;
        for (int i = 0; i < length; i++)
        {
            var flowerBox = Instantiate(flowerBoxPb, itemZone.GetChild(i));
            flowerBox.AssignItem(i);
            flowerBox.AssignItem(data.traySprites[i], data.lidSprites[i], data.shapeFlowerItems[i].sprites);
            flowerBox.transform.localPosition = Vector3.zero;
        }
    }

}
