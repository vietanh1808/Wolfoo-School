using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class CharacterAnimation : MonoBehaviour
{
    [Header("<======== BACKBONE_ITEM =======>")]
    [SerializeField] Image snowBall;
    [SerializeField] Image muddyImg;
    [SerializeField] Image bubbleImg;
    [SerializeField] SexType sexType = SexType.Boy;   

    [Header("<======== SPINE =======>")]
    [SerializeField] private SkeletonGraphic skeletonAnim;
    [Header("Idle")]
    [SerializeField, SpineAnimation] private string idleAnim;
    [Header("Run")]
    [SerializeField, SpineAnimation] private string runAnim;
    [Header("Walk")]
    [SerializeField, SpineAnimation] private string walkAnim;
    [Header("Laugh")]
    [SerializeField, SpineAnimation] private string laughAnim;
    [Header("Jump")]
    [SerializeField, SpineAnimation] private string jumpAnim;
    [Header("Happy")]
    [SerializeField, SpineAnimation] private string happyAnim;
    [Header("Sad")]
    [SerializeField, SpineAnimation] private string sadAnim;
    [Header("Special")]
    [SerializeField, SpineAnimation] private string specialAnim;
    [Header("WaveHand")]
    [SerializeField, SpineAnimation] private string wavehandAnim;
    [Header("HandUp")]
    [SerializeField, SpineAnimation] private string HandUpAnim;
    [Header("HoldHand")]
    [SerializeField, SpineAnimation] private string HoldHandAnim;
    [Header("Throwing")]
    [SerializeField, SpineAnimation] private string ThrowingAnim;
    [Header("Eat")]
    [SerializeField, SpineAnimation] private string EatAnim;
    [Header("Disagree")]
    [SerializeField, SpineAnimation] private string DisagreeAnim;
    [Header("Dizzy")]
    [SerializeField, SpineAnimation] private string dizzyAnim;
    [Header("Sit")]
    [SerializeField, SpineAnimation] private string sitAnim;
    [Header("TakeAPhoto")]
    [SerializeField, SpineAnimation] private string takeAPhoto;
    private AnimState animState;
    private Tween timeTween;
    private Tween tweenDelay;

    public enum SexType
    {
        Boy,
        Girl,
    }

    public SkeletonGraphic SkeletonAnim { get => skeletonAnim; set => skeletonAnim = value; }
    public Image SnowBall { get => snowBall; }
    public Image MuddyImg { get => muddyImg; set => muddyImg = value; }
    public Image BubbleImg { get => bubbleImg; set => bubbleImg = value; }

    private void Awake()
    {
        if(bubbleImg != null)
        {
            bubbleImg.gameObject.SetActive(false);
        }
    }
    public void TurnOnBubble(float timeDelay)
    {
        tweenDelay = DOVirtual.DelayedCall(timeDelay, () =>
        {
            bubbleImg.gameObject.SetActive(true);
        });
    }
    public void TurnOffBubble()
    {
        tweenDelay?.Kill();
        bubbleImg.gameObject.SetActive(false);
    }
    public void StopMove()
    {
        if (animState != AnimState.Laugh || animState != AnimState.Idle)
            PlayIdle();
        PlayLaugh();
    }

    public void PlaySadAnim()
    {
        PlaySad();
        DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }
    public void PlayCarryAnim()
    {
        PLayTakeAPhoto();
        DOVirtual.DelayedCall(GetTimeAnimation(animState), () => PlayIdle());
    }

    #region Anim by Spine
    public void PlayMove()
    {
        if (animState == AnimState.Run) return;
        animState = AnimState.Run;
        if(SoundManager.instance != null)
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Walk, sexType);
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, runAnim, true);
    }
    public void PlayIdle()
    {
        if (animState == AnimState.Idle)
            return;
        animState = AnimState.Idle;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, idleAnim, true);
    }
    public void PlayDizzy()
    {
        if (animState == AnimState.Dizzy)
            return;
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Complain);
        animState = AnimState.Dizzy;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, dizzyAnim, true);
    }
    public void PLayTakeAPhoto()
    {
        if (animState == AnimState.TakeAPhoto)
            return;
        animState = AnimState.TakeAPhoto;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, takeAPhoto, true);


    }
    public void PlayJump()
    {
        if (animState == AnimState.Jump)
            return;
        animState = AnimState.Jump;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, jumpAnim, false);
    }
    public void PlayDisagree()
    {
        if (animState == AnimState.Disagree)
            return;
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Sad, sexType);
        animState = AnimState.Disagree;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, DisagreeAnim, false);
        DOVirtual.DelayedCall(GetTimeAnimation(animState) - 1f, () =>
        {
            PlayIdle();
        });
    }
    public void PlayEat()
    {
        if (animState == AnimState.Eat)
            return;
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Laugh, sexType);
        animState = AnimState.Eat;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, EatAnim, false);
        DOVirtual.DelayedCall(GetTimeAnimation(animState), () =>
        {
            PlayIdle();
        });
    }
    public void PlayHappy()
    {
        if (animState == AnimState.Happy)
            return;
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Hooray, sexType);
        animState = AnimState.Happy;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, happyAnim, false);
    }
    public void PlaySad()
    {
        if (animState == AnimState.Sad)
            return;
        animState = AnimState.Sad;
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Complain, sexType);
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, sadAnim, false);
    }
    public void PlaySit()
    {
        if (animState == AnimState.Sit)
            return;
        animState = AnimState.Sit;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, sitAnim, true);
    }
    public void PlayLaugh()
    {
        if (animState == AnimState.Laugh)
            return;
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Laugh, sexType);
        animState = AnimState.Laugh;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, laughAnim, false);

        if (timeTween != null && timeTween.IsActive())
        {
            timeTween?.Kill();
            return;
        }
        timeTween = DOVirtual.DelayedCall(GetTimeAnimation(animState), () =>
        {
            PlayIdle();
        });
    }
    public void PlaySpecial()
    {
        if (animState == AnimState.Special)
            return;
        SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Hoow, sexType);
        animState = AnimState.Special;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, specialAnim, false);

        DOVirtual.DelayedCall(GetTimeAnimation(animState), () =>
        {
            PlayIdle();
        });
    }
    public void PlayWaveHand()
    {
        if (animState == AnimState.WaveHand) return;
        if(SoundManager.instance != null)
            SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Hello, sexType);
        animState = AnimState.WaveHand;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, wavehandAnim, false);
        DOVirtual.DelayedCall(GetTimeAnimation(animState), () =>
        {
            PlayIdle();
        });
    }
    public void PlayWalk()
    {
        if (animState == AnimState.Walk) return;
        animState = AnimState.Walk;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, walkAnim, true);
    }
    public void PlayWalk(float rdX)
    {
        transform.Rotate(rdX < 0 ? Vector3.up * 180 : Vector3.zero);
    }
    public void PlayWalk(bool isRight)
    {
        //Debug.Log("Move " + isRight);
        transform.rotation = Quaternion.Euler(isRight ? new Vector3(0, 180, 0) : Vector3.zero);
    }
    public float GetTimeAnimation(AnimState animState)
    {
        var myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim);
        switch (animState)
        {
            case AnimState.Sad:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(sadAnim);
                break;
            case AnimState.Special:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(specialAnim);
                break;
            case AnimState.WaveHand:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(wavehandAnim);
                break;
            case AnimState.HoldHand:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(HoldHandAnim);
                break;
            case AnimState.HandUp:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(HandUpAnim);
                break;
            case AnimState.Throwing:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(ThrowingAnim);
                break;
            case AnimState.Laugh:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(laughAnim);
                break;
            case AnimState.Walk:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(walkAnim);
                break;
            case AnimState.Eat:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(EatAnim);
                break;
            case AnimState.Dizzy:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(dizzyAnim);
                break;
            case AnimState.Sit:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(sitAnim);
                break;
            case AnimState.TakeAPhoto:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(takeAPhoto);
                break;
        }
       
        float animLength = myAnimation.Duration;
        return animLength;
    }

    public enum AnimState
    {
        None,
        Idle,
        Run,
        Jump,
        Happy,
        Sad,
        Special,
        WaveHand,
        HandUp,
        Throwing,
        HoldHand,
        Laugh,
        Walk,
        Eat,
        Disagree,
        Dizzy,
        Sit,
        TakeAPhoto
    }
    #endregion
   
}
