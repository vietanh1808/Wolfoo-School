using DG.Tweening;
using SCN;
using SCN.ActionLib;
using SCN.Tutorial;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pistil : ItemMove
{
    [SerializeField] DragFollowPath dragFollowPath;
    [SerializeField] CustomPath[] customPaths;
    [SerializeField] Image[] customPathImgs;
    [SerializeField] Image[] tutorialLineImgs;

    private int countLine = -1;
    private Vector3 offsetKnife;
    private LineKnife knife;
    private Tweener fadeTween;
    private Tween delayItemTween;
    private Tweener moveItemTween;
    private Tweener scaleItemTween;

    private void Start()
    {
        itemImg = GetComponent<Image>();    
        if(dragFollowPath != null)
        {
            for (int i = 0; i < customPathImgs.Length; i++)
            {
                customPathImgs[i].fillAmount = 0;
                tutorialLineImgs[i].DOFade(0, 0);
            }
        }
    }
    private void OnDestroy()
    {
        if (moveItemTween != null) moveItemTween?.Kill();
        if (scaleItemTween != null) scaleItemTween?.Kill();
        if (fadeTween != null) fadeTween?.Kill();
        if (delayItemTween != null) delayItemTween?.Kill();
    }
    private void OnEnable()
    {
        if(dragFollowPath != null)
        dragFollowPath.OnProgress += GetDragProgress;
    }
    private void OnDisable()
    {
        if(dragFollowPath != null)
        dragFollowPath.OnProgress -= GetDragProgress;
    }
    public void AssignItem(Sprite sprite)
    {
        itemImg.sprite = sprite;
        itemImg.SetNativeSize();
    }
    public void AssignSprite(Sprite sprite)
    {
        itemImg.sprite = sprite;
    }

    private void GetDragProgress(float progressValue)
    {
        if (countLine >= customPathImgs.Length) return;
        if (progressValue == 1)
        {
            TutorialManager.Instance.Stop();
            GUIManager.instance.PlayLighting(dragFollowPath.DragObject.transform);
            AssignLine(knife, () =>
            {
                EventDispatcher.Instance.Dispatch(new EventKey.OnCompleteAll { pistil = this });
            });
            return;
        }

        SoundManager.instance.PlayOtherSfx(SfxOtherType.Draw);
        offsetKnife = knife.transform.position - knife.CompareTrans.position;
        knife.transform.position = dragFollowPath.DragObject.transform.position + offsetKnife;
        customPathImgs[countLine].fillAmount = progressValue;
    }

    public void AssignLine(LineKnife _knife, System.Action OnCompleteAll)
    {
        knife = _knife;
        countLine++;
        if (countLine > customPaths.Length) return;
        if (countLine == customPaths.Length)
        {
            if (fadeTween != null)
            {
                fadeTween?.Kill();
                tutorialLineImgs[countLine - 1].DOFade(0, 0);
            }
            OnCompleteAll?.Invoke();
            return;
        }

        if(countLine == customPaths.Length/2)
        {
            knife.OnChangeDirection(LineKnife.Direction.Vertical);
        }
        if (fadeTween != null)
        {
            fadeTween?.Kill();
            fadeTween = tutorialLineImgs[countLine - 1].DOFade(0, 0);
        }

        dragFollowPath.MovePath = customPaths[countLine];
        offsetKnife = knife.transform.position - knife.CompareTrans.position;
        knife.AssignEndPos(dragFollowPath.MovePath.GetPoint(0).position + offsetKnife, transform);
        knife.MoveToEndPos(() =>
        {
            fadeTween = tutorialLineImgs[countLine]
            .DOFade(1, 0.5f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);

            if(countLine == 0)
            {
                dragFollowPath.Init();
            }
            else
            {
                dragFollowPath.Reset();
            }

            TutorialManager.Instance.StartPointer(
                dragFollowPath.MovePath.GetPoint(0).position,
                dragFollowPath.MovePath.GetPoint(1).position);
        });

    }
}
