using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class WoodHorseAnimation : MonoBehaviour
{
    [Header("<======== SPINE =======>")]
    [SerializeField] private SkeletonGraphic skeletonAnim;
    [Header("Idle")]
    [SerializeField, SpineAnimation] private string idleAnim;
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
    public void PlayAnim(bool isLoop = false, System.Action OnComplete = null)
    {
        if (animState == AnimState.Play)
            return;
        animState = AnimState.Play;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, playAnim, isLoop);

        if (delayTween != null) delayTween?.Kill();
        if(!isLoop)
        {
            delayTween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () =>
            {
                PlayIdle();
                OnComplete?.Invoke();
            });
        }
    }
    public float GetTimeAnimation(AnimState animState)
    {
        var myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim);
        switch (animState)
        {
            case AnimState.Idle:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim);
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
        Play,
    }
    #endregion

}
