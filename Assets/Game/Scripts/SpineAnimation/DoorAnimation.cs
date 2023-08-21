using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class DoorAnimation : MonoBehaviour
{
    [Header("<======== SPINE =======>")]
    [SerializeField] private SkeletonGraphic skeletonAnim;
    [Header("OpenIdle")]
    [SerializeField, SpineAnimation] private string openIdleAnim;
    [Header("Close")]
    [SerializeField, SpineAnimation] private string closeAnim;
    [Header("OpenAnim")]
    [SerializeField, SpineAnimation] private string openAnim;
    private AnimState animState;
    private Tween delayTween;

    public SkeletonGraphic SkeletonAnim { get => skeletonAnim; set => skeletonAnim = value; }

    private void Start()
    {
    }

    #region Anim by Spine
    public void CloseAnim()
    {
        if (delayTween != null && delayTween.IsActive()) return;
        if (animState == AnimState.Close)
            return;
        animState = AnimState.Close;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, closeAnim, true);
    }
    void OpenIdleAnim()
    {
        if (animState == AnimState.Open)
            return;
        animState = AnimState.Open;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, openIdleAnim, true);
    }
    public void OpenAnim(System.Action OnComplete = null)
    {
        if (animState == AnimState.Open)
            return;
        animState = AnimState.Open;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, openAnim, false);
        delayTween = DOVirtual.DelayedCall(GetTimeAnimation(animState) + 0.1f, () =>
        {
            OpenIdleAnim();
            OnComplete?.Invoke();
        });
    }
    public float GetTimeAnimation(AnimState animState)
    {
        var myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(closeAnim);
        switch (animState)
        {
            case AnimState.OpenIdle:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(openIdleAnim);
                break;
            case AnimState.Close:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(closeAnim);
                break;
            case AnimState.Open:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(openAnim);
                break;
        }

        float animLength = myAnimation.Duration;
        return animLength;
    }

    public enum AnimState
    {
        None,
        Idle,
        Play,
        Open,
        Close,
        OpenIdle,
    }
    #endregion

}
