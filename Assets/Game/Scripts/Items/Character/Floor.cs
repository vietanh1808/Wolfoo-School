using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Floor : BackItem
{
    protected override void Start()
    {
        base.Start();
        skinType = SkinBackItemType.Floor;
    }


}
