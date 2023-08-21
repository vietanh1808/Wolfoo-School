using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Helper;
using Spine.Unity;
public class GiftAnimation : MonoBehaviour
{
    [Header("<======== SPINE =======>")]
    [SerializeField] private SkeletonGraphic skeletonAnim;
    [Header("Idle")]
    [SerializeField, SpineAnimation] private string idleAnim;
    [Header("OpenIdle")]
    [SerializeField, SpineAnimation] private string openIdleAnim;
    [Header("Open")]
    [SerializeField, SpineAnimation] private string openAnim;
    [Header("Close")]
    [SerializeField, SpineAnimation] private string closeAnim;
    private AnimState animState;
    public SkeletonGraphic SkeletonAnim { get => skeletonAnim; set => skeletonAnim = value; }

    [Header("Skin")]
    [SerializeField, SpineSkin] string[] skinList;

    private bool canClick = true;

    public enum ColorType
    {
        Red,
        RedDecor,
        Purple,
        Green,
        Blue,
    }

    public void ChangeSkin(ColorType colorType)
    {
        skeletonAnim.Skeleton.SetSkin(skinList[(int)colorType]);
        skeletonAnim.Skeleton.SetSlotsToSetupPose();
    }
    public void ChangeSkin()
    {
        skeletonAnim.Skeleton.SetSkin(skinList[Random.Range(0, skinList.Length)]);
        skeletonAnim.Skeleton.SetSlotsToSetupPose();
    }
    public void PlayPutItemIntoGift(GameObject obj, System.Action OnComplete = null)
    {
        obj.transform.SetParent(transform.parent);
        obj.transform.SetAsLastSibling();
        obj.transform.DOScale(Vector3.zero, 1.5f);
        obj.transform.DOJump(Vector3.down * 1, 5, 1, 1.5f).OnComplete(() =>
        {
            obj.transform.SetParent(GUIManager.instance.CurMode.transform);
            if (animState == AnimState.Close) return;
            animState = AnimState.Close;
            AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, closeAnim, false);
            DOVirtual.DelayedCall(GetTimeAnimation(animState) + 2, () =>
            {
                OnComplete?.Invoke();
            });
        });
    }
    public void PlayOpenAnim(System.Action action = null)
    {
        gameObject.SetActive(true);
        float time = 0.5f;
        canClick = false;
        PlayOpen();
        DOVirtual.DelayedCall(GetTimeAnimation(AnimState.Open) - time, () =>
        {
            SoundManager.instance.PlayOtherSfx(SfxOtherType.Lighting);
            action?.Invoke();
            DOVirtual.DelayedCall(time, () =>
            {
                PlayIdle();
                gameObject.SetActive(false);
            });
        });
    }
    #region Anim by Spine
    public void PlayOpen()
    {
        if (animState == AnimState.Open) return;
        animState = AnimState.Open;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, openAnim, false);
    }
    public void Stop()
    {
        PlayIdle();
    }
    public void PlayIdle()
    {
        if (animState == AnimState.Idle)
            return;
        animState = AnimState.Idle;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, idleAnim, true);
    }
    public void PlayOpenIdle()
    {
        if (animState == AnimState.OpenIdle)
            return;
        animState = AnimState.Idle;
        AnimationHelper.PlayAnimation(SkeletonAnim.AnimationState, openIdleAnim, true);
    }
    public float GetTimeAnimation(AnimState animState)
    {
        var myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim);
        switch (animState)
        {
            case AnimState.Idle:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(idleAnim);
                break;
            case AnimState.Open:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(openAnim);
                break;
            case AnimState.OpenIdle:
                myAnimation = SkeletonAnim.Skeleton.Data.FindAnimation(openIdleAnim);
                break;
        }

        float animLength = myAnimation.Duration;
        return animLength;
    }

    public enum AnimState
    {
        None,
        Idle,
        Open,
        Close,
        OpenIdle
    }
    #endregion

}
