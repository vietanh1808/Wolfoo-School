using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] Image loadingImg;
    [SerializeField] GameObject wolfooImg;
    [SerializeField] Image backgroundImg;
    [SerializeField] Transform destination;
    [SerializeField] float timeLoading;

    Vector2 startPos;

    private void Awake()
    {
        startPos = wolfooImg.transform.position;
        loadingImg.fillAmount = 0;
        backgroundImg.DOFade(0, 0);
        gameObject.SetActive(false);
    }

    public void Open(System.Action action1 = null, System.Action action2 = null)
    {
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
        backgroundImg.DOFade(1, 0.5f).OnComplete(() =>
        {
            action1?.Invoke();
            wolfooImg.transform.DOMoveX(destination.position.x, timeLoading);
            loadingImg.DOFillAmount(1, timeLoading).OnComplete(() =>
            {
                action2?.Invoke();
                loadingImg.fillAmount = 0;
                backgroundImg.DOFade(0, 0);
                gameObject.SetActive(false);
                wolfooImg.transform.position = startPos;
            });
        });
    }
}
