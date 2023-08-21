using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetupManager : MonoBehaviour
{
    public Transform center;
    public Transform outsideLeft;
    public Transform outsideDown;
    public Transform endMoveShapeFlowerZone;
    public Transform[] endMoveShapeItemZones;
    public Transform addPetalZone;
    public Transform addTrunkZone;
    public Transform potLastZone;
    public Transform coinZone;

    private static UISetupManager instance;

    public static UISetupManager Instance { get => instance; }

    private void Awake()
    {
        if(instance == null) instance = this;

    }
}
