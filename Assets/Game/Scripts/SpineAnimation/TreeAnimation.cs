using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class TreeAnimation : MonoBehaviour
{
    [Header("<======== SPINE =======>")]
    [SerializeField] private SkeletonGraphic skeletonAnim;
    [Header("Idle")]
    [SerializeField, SpineAnimation] private string idleAnim;
    [Header("Idle2")]
    [SerializeField, SpineAnimation] private string idleAnim2;
    [Header("PlayAnim")]
    [SerializeField, SpineAnimation] private string playAnim;
    private AnimState animState;
    private Tween delayTween;

    public SkeletonGraphic SkeletonAnim { get => skeletonAnim; set => skeletonAnim = value; }

    #region Anim by Spine
    public void PlayIdle()
    {
        if (animState == AnimState.Idle)
            return;
        animState = AnimState.Idle;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, idleAnim, true);
    }
    public void PlayIdle2()
    {
        if (animState == AnimState.Idle2)
            return;
        animState = AnimState.Idle2;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, idleAnim2, true);
    }
    public void PlayAnim()
    {
        if (animState == AnimState.Play)
            return;
        animState = AnimState.Play;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, playAnim, false);
        if (delayTween != null) delayTween?.Kill();
        delayTween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () =>
        {
            PlayIdle2();
        });
    }
    public float GetTimeAnimation(AnimState animState)
    {
        var myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim);
        switch (animState)
        {
            case AnimState.Idle:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim);
                break;
            case AnimState.Idle2:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim2);
                break;
            case AnimState.Play:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(playAnim);
                break;
        }

        float animLength = myAnimation.Duration;
        return animLength;
    }

    public enum AnimState
    {
        None,
        Idle,
        Idle2,
        Play,
    }
    #endregion

}
