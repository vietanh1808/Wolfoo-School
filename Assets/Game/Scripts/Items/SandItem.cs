using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SandItem : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] LineRenderer linePb;
    [SerializeField] Transform limitLeft;
    [SerializeField] Transform limitRight;
    [SerializeField] Transform limitUp;
    [SerializeField] Transform limitDown;

    int currentIndex = 0;
    Vector3 lastPos;
    LineRenderer curLine;
    private Vector3 currentPos;
    List<LineRenderer> lines = new List<LineRenderer>();
    bool isEraseMode;

    private void Start()
    {
        EventManager.OnClickAlpha += GetClickAlpha;
        EventManager.OnUpdateStartPosAlpha += GetUpdatePos;
        EventManager.OnBeginDragDrake += GetBeginDragDrake;
        EventManager.OnEndDragDrake += GetEndDragDrake;

        curLine = Instantiate(linePb, transform);
        lines.Add(curLine);
    }
    private void OnDestroy()
    {
        EventManager.OnClickAlpha -= GetClickAlpha;
        EventManager.OnUpdateStartPosAlpha -= GetUpdatePos;
        EventManager.OnBeginDragDrake -= GetBeginDragDrake;
        EventManager.OnEndDragDrake -= GetEndDragDrake;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            GetScratch();
        }
    }

    private void GetBeginDragDrake()
    {
        Debug.Log("Begin Drag");
        isEraseMode = true;
    }

    private void GetEndDragDrake()
    {
        Debug.Log("End Drag");
        isEraseMode = false;
    }

    private void GetUpdatePos(Vector3 pos)
    {
        currentPos = pos;
        currentPos.z = 0;
        curLine = Instantiate(linePb, transform);
        lines.Add(curLine);
    }

    private void GetClickAlpha(int arg1, bool arg2)
    {
        foreach (var line in lines)
        {
            line.sortingOrder = 0;
        }
        lines.Clear();
    }

    private void GetScratch()
    {
        if (isEraseMode) return;

        Debug.Log("Scratching");
        currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPos.z = 0;

        if (currentPos.x < limitLeft.position.x || currentPos.x > limitRight.position.x) return;
        if (currentPos.y < limitDown.position.y || currentPos.y > limitUp.position.y) return;

        if (Vector2.Distance(lastPos, currentPos) >= 0.01f)
        {
            UpdateLine();

            if (!SoundManager.instance.Sfx.isPlaying)
                SoundManager.instance.PlayOtherSfx(SfxOtherType.Draw);
        }
    }

    void UpdateLine()
    {
        currentIndex++;
        lastPos = currentPos;
        curLine.positionCount = currentIndex;
        curLine.SetPosition(currentIndex - 1, currentPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentIndex = 0;
        curLine = Instantiate(linePb, transform);
        lines.Add(curLine);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
