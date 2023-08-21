using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LosePanel : Panel, IPointerClickHandler
{
    [SerializeField] Image loseImg;
    [SerializeField] Transform endLogoTrans;
    [SerializeField] CharacterAnimation wolfooAnim;
    private Vector3 startPos;

    private void OnEnable()
    {
       // SoundManager.instance.PlayWolfooSfx(SfxWolfooType.Sad, CharacterAnimation.SexType.Boy);
    }
    private void Start()
    {
        startPos = loseImg.transform.position;
        loseImg.transform.DOMoveY(endLogoTrans.position.y, 1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            loseImg.transform.DOPunchScale(new Vector3(-0.2f, 0.2f, 0), 0.5f, 2);
            wolfooAnim.PlayDizzy();
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        EventManager.OnClickPanel?.Invoke(PanelType.Lose);
    }
}
