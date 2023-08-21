using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tray : ItemMove
{
    [SerializeField] Transform itemZone;

    public Transform ItemZone { get => itemZone; }
}
