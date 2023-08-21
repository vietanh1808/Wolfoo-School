using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
public class Balloon : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private Image imageBall;
    [SerializeField] private ParticleSystem effectPop;
    [SerializeField] private AudioSource soundPop;
    Tween tweenMove = null;

    public Image ImageBall { get => imageBall; set => imageBall = value; }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tweenMove != null) tweenMove.Kill();
        ImageBall.enabled = false;
        effectPop.Play();
        soundPop.Play();
        DOVirtual.DelayedCall(1f, () => {
            gameObject.SetActive(false);
        });
    }
    public void OnAnimation()
    {
        if (tweenMove != null) tweenMove.Kill();
        ImageBall.enabled = true;
        transform.localPosition = new Vector3(Random.Range(-800f, 800f), 0, 0);
        tweenMove= transform.DOLocalMoveY(Random.Range(2000f, 2500f), Random.Range(5f, 8f)).SetEase(Ease.Linear).OnComplete(() => {
            gameObject.SetActive(false); 
        });
    }
}
