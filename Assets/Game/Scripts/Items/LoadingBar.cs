using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : ItemMove
{
    [SerializeField] Image fillbarImg;

    private float totalTime;
    private float curTime;
    private bool isComplete;
    private Tweener punchRotateTween;

    public bool IsComplete { get => isComplete; }

    private void Start()
    {
        fillbarImg.fillAmount = 0;
        transform.localScale = Vector3.one * 0.6f;
    }
    private void OnDestroy()
    {
        if (punchRotateTween != null) punchRotateTween?.Kill();
    }

    public void AssignItem(float _totalTime)
    {
        isComplete = false;
        curTime = 0;
        totalTime = _totalTime;
        fillbarImg.fillAmount = 0;
    }

    public float OnProgress(System.Action OnComplete)
    {
        if (isComplete == true) return curTime;
        if (curTime >= totalTime)
        {
            isComplete = true;
            OnComplete?.Invoke();
     //       SoundManager.instance.PlayOtherSfx(SfxOtherType.OnComplete);
            punchRotateTween = transform.DOPunchRotation(Vector3.forward * -5, 0.25f, 1).OnComplete(() =>
            {
                punchRotateTween = transform.DOPunchRotation(Vector3.forward * 5, 0.25f, 1).OnComplete(() =>
                {
                    punchRotateTween = transform.DORotate(Vector3.zero, 0.25f).OnComplete(() =>
                    {
                        MoveOut(Direction.Left, 0.5f);
                    });
                });
            });
            return curTime;
        }
        curTime += Time.deltaTime;
        fillbarImg.fillAmount = curTime / totalTime;
        return curTime;
    }
    public void OnProgress(float progress, System.Action OnComplete = null)
    {
        if (isComplete == true) return;
        fillbarImg.fillAmount = progress;
        if (progress >= 1)
        {
            isComplete = true;
       //     SoundManager.instance.PlayOtherSfx(SfxOtherType.OnComplete);
            GUIManager.instance.starExplore.transform.position = transform.position + Vector3.up * 3;
            GUIManager.instance.starExplore.Play();
            punchRotateTween = transform.DOPunchRotation(Vector3.forward * -5, 0.25f, 1).OnComplete(() =>
            {
                punchRotateTween = transform.DOPunchRotation(Vector3.forward * 5, 0.25f, 1).OnComplete(() =>
                {
                    punchRotateTween = transform.DORotate(Vector3.zero, 0.25f).OnComplete(() =>
                    {
                        OnComplete?.Invoke();   
                        MoveOut(Direction.Left, 0.5f);
                    });
                });
            });
            return;
        }
    }
}
