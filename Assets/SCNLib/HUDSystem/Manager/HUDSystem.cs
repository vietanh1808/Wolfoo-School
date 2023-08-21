using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUDSystem : UIPanels<HUDSystem>
{
    private void Awake()
    {
    }
    private static bool isLock = false;
    public static bool IsLock
    {
        get => isLock;
        set => isLock = value;
    }
    protected virtual void Start()
    {
    }
}
