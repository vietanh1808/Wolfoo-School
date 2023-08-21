using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] int totalHeart;
    [SerializeField] Image whitePanel;
    [SerializeField] Transform heartZone;

    List<Image> heartImgs = new List<Image>();
    List<Image> fillImgs = new List<Image>();

    int curHeart;
    Image heartFillCenterImg;
    Image heartCenterImg;

    private void Start()
    {
        curHeart = totalHeart;
        heartCenterImg = whitePanel.transform.GetChild(0).GetComponent<Image>();
        heartFillCenterImg = whitePanel.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        heartFillCenterImg.transform.localScale = Vector3.zero;
        heartCenterImg.gameObject.SetActive(false);

        whitePanel.DOFade(0, 0);
        for (int i = 0; i < heartZone.childCount; i++)
        {
            var image = heartZone.GetChild(i).GetChild(0);
            heartImgs.Add(image.GetComponent<Image>());
            fillImgs.Add(image.GetChild(0).GetComponent<Image>());
        }

        foreach (var item in fillImgs)
        {
            item.fillAmount = 1;
        }
    }

    public void OnCountDown(System.Action OnComplete = null, System.Action OnFail = null)
    {
        curHeart--;

        heartCenterImg.gameObject.SetActive(true);
        // Indicate down
        int totalTime = 1;
        heartFillCenterImg.transform.localScale = Vector3.one;
        whitePanel.DOFade(1, totalTime / 4).OnComplete(() =>
        {
            whitePanel.DOFade(1/2, totalTime / 4).OnComplete(() =>
            {
                whitePanel.DOFade(1/3, totalTime / 4).OnComplete(() =>
                {
                    whitePanel.DOFade(0, totalTime / 4);
                });
            });
        });

        heartFillCenterImg.DOFillAmount(0, totalTime).OnComplete(() =>
        {
            fillImgs[curHeart].fillAmount = 0;
            heartFillCenterImg.transform.DOScale(0, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (curHeart == 0)
                {
                    OnFail?.Invoke();
                } 
                else
                {
                    OnComplete?.Invoke();
                }
                heartCenterImg.gameObject.SetActive(false);
                heartFillCenterImg.fillAmount = 1;
            });
        });
    }
}
