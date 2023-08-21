using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class PeelerAnimation : MonoBehaviour
{
    [Header("<======== SPINE =======>")]
    [SerializeField] private SkeletonGraphic skeletonAnim;
    [Header("Close")]
    [SerializeField, SpineAnimation] private string closeAnim;
    [Header("ExcuteAnim")]
    [SerializeField, SpineAnimation] private string excuteAnim;
    private AnimState animState;
    private Tween delayTween;

    public SkeletonGraphic SkeletonAnim { get => skeletonAnim; set => skeletonAnim = value; }

    private void Start()
    {
    }

    #region Anim by Spine
    public void IdleAnim()
    {
        if (delayTween != null && delayTween.IsActive()) return;
        if (animState == AnimState.Close)
            return;
        animState = AnimState.Close;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, closeAnim, true);
    }
    public void ExcuteAnim(System.Action OnComplete = null)
    {
        if (animState == AnimState.Excute)
            return;
        animState = AnimState.Excute;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, excuteAnim, true);
    }
    public float GetTimeAnimation(AnimState animState)
    {
        var myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(closeAnim);
        switch (animState)
        {
            case AnimState.Close:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(closeAnim);
                break;
            case AnimState.Excute:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(excuteAnim);
                break;
        }

        float animLength = myAnimation.Duration;
        return animLength;
    }

    public enum AnimState
    {
        None,
        Excute,
        Close,
    }
    #endregion

}
